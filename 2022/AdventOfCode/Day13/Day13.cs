using System.Text.Json.Nodes;

namespace AdventOfCode.Day13;

public sealed class Day13 : TestBase
{
    protected override string Day => "Day13";

    public Day13(ITestOutputHelper output)
        : base(output)
    {
    }

    private string[] Example = new[]
    {
        "[1,1,3,1,1]",
        "[1,1,5,1,1]",
        "",
        "[[1],[2,3,4]]",
        "[[1],4]",
        "",
        "[9]",
        "[[8,7,6]]",
        "",
        "[[4,4],4,4]",
        "[[4,4],4,4,4]",
        "",
        "[7,7,7,7]",
        "[7,7,7]",
        "",
        "[]",
        "[3]",
        "",
        "[[[]]]",
        "[[]]",
        "",
        "[1,[2,[3,[4,[5,6,7]]]],8,9]",
        "[1,[2,[3,[4,[5,6,0]]]],8,9]"
    };

    [Fact]
    public void ExampleOne() => FindCorrectSignals(Example).Should().Be(13);

    [Fact]
    public void PartOne() => Output.WriteLine($"Signals: {FindCorrectSignals(Input)}");

    [Fact]
    public void ExampleTwo() => FindDecoderKey(Example).Should().Be(140);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Decoder key: {FindDecoderKey(Input)}");

    private int FindDecoderKey(string[] input)
    {
        var packets = input.Where(i => !string.IsNullOrWhiteSpace(i))
            .Select(p => (JsonNode.Parse(p) as JsonArray)!)
            .ToList();

        var two = new JsonArray(new JsonArray(2));
        var six = new JsonArray(new JsonArray(6));
        packets.Add(two);
        packets.Add(six);

        var sorted = packets
            .OrderBy(x => x, new PacketComparer())
            .Select((x, i) => (Packet: x, Ord: i + 1));

        var twoOrd = sorted.FirstOrDefault(s => PacketComparer.CompareTo(s.Packet, two) == 0).Ord;
        var sixOrd = sorted.FirstOrDefault(s => PacketComparer.CompareTo(s.Packet, six) == 0).Ord;

        return twoOrd * sixOrd;
    }

    private int FindCorrectSignals(string[] input) =>
        input.Where(i => !string.IsNullOrWhiteSpace(i)).Chunk(2)
            .Select(i => (First: JsonNode.Parse(i[0]) as JsonArray, Second: JsonNode.Parse(i[1]) as JsonArray))
            .Select((x, i) => (Ord: i + 1, Comp: PacketComparer.CompareTo(x.First!, x.Second!)))
            .Aggregate(0, (acc, c) => acc += c.Comp == -1 ? c.Ord : 0);

    private sealed class PacketComparer : IComparer<JsonArray>
    {
        public int Compare(JsonArray? x, JsonArray? y) =>
            (x, y) switch
            {
                (null, null) => 0,
                (null, _) => -1,
                (_, null) => 1,
                _ => CompareTo(x, y)
            };

        public static int CompareTo(JsonNode x, JsonNode y)
        {
            return (x, y) switch
            {
                (JsonValue i1, JsonValue i2) => Math.Sign(i1.GetValue<int>() - i2.GetValue<int>()),
                (JsonValue i1, JsonArray j2) => CompareTo(new JsonArray(i1.GetValue<int>()), j2),
                (JsonArray j1, JsonValue i2) => CompareTo(j1, new JsonArray(i2.GetValue<int>())),
                (JsonArray j1, JsonArray j2) => Cmp(j1, j2),
                _ => throw new Exception("Unsupported value")
            };

            int Cmp(JsonArray j1, JsonArray j2) 
            {
                int i;
                for (i = 0; i < Math.Min(j1.Count, j2.Count); i++)
                {
                    var cmp = CompareTo(j1[i]!, j2[i]!);
                    if (cmp is -1 or 1) return cmp;
                }
                if (i == j1.Count && i < j2.Count) return -1;
                if (i == j2.Count && i < j1.Count) return 1;

                return 0;
            }
        }
    }
}
