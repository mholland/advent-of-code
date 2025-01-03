using ThreeNetwork = (string C1, string C2, string C3);

namespace AdventOfCode.Day23;

public sealed class Day23(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 23);

    private readonly string[] _example =
    [
        "kh-tc",
        "qp-kh",
        "de-cg",
        "ka-co",
        "yn-aq",
        "qp-ub",
        "cg-tb",
        "vc-aq",
        "tb-ka",
        "wh-tc",
        "yn-cg",
        "kh-ub",
        "ta-co",
        "de-co",
        "tc-td",
        "tb-wq",
        "wh-td",
        "ta-ka",
        "td-qp",
        "aq-cg",
        "wq-ub",
        "ub-vc",
        "de-ta",
        "wq-aq",
        "wq-vc",
        "wh-yn",
        "ka-de",
        "kh-ta",
        "co-tc",
        "wh-qp",
        "tb-vc",
        "td-yn",
    ];

    [Fact]
    public void ExampleOne() => CountInterconnected(_example).Should().Be(7);

    [Fact]
    public async Task PartOne() => WriteOutput(CountInterconnected(await ReadInputLines()));

    [Fact]
    public void ExampleTwo() => FindPassword(_example).Should().Be("co,de,ka,ta");

    [Fact]
    public async Task PartTwo() => WriteOutput(FindPassword(await ReadInputLines()));

    private static string FindPassword(string[] input)
    {
        var mapping = ParseGraph(input);
        var nodes = mapping.Keys.ToHashSet();

        var cliques = FindCliques(mapping, [], nodes, []);
        var max = cliques.MaxBy(c => c.Count) ?? [];

        return string.Join(",", max.OrderBy(x => x));
    }

    private static List<HashSet<string>> FindCliques(Dictionary<string, string[]> mapping, HashSet<string> potential, HashSet<string> remaining, HashSet<string> skip)
    {
        if (remaining.Count == 0 && skip.Count == 0)
            return [potential];

        List<HashSet<string>> cliques = [];
        HashSet<string> currentRemaining = [.. remaining];
        foreach (var node in remaining)
        {
            HashSet<string> newPotential = [.. potential, node];
            var newRemaining = currentRemaining.Where(r => mapping[node].Contains(r)).ToHashSet();
            var newSkip = skip.Where(r => mapping[node].Contains(r)).ToHashSet();
            cliques.AddRange(FindCliques(mapping, newPotential, newRemaining, newSkip));

            skip.Add(node);
            currentRemaining.Remove(node);
        }

        return cliques;
    }

    private static Dictionary<string, string[]> ParseGraph(string[] input)
    {
        var mapping = new Dictionary<string, List<string>>();
        foreach (var connection in input)
        {
            var split = connection.Split('-');
            var (c1, c2) = (split[0], split[1]);
            if (!mapping.TryGetValue(c1, out _))
                mapping.Add(c1, [c2]);
            mapping[c1].Add(c2);
            if (!mapping.TryGetValue(c2, out _))
                mapping.Add(c2, [c1]);
            mapping[c2].Add(c1);
        }

        return mapping.ToDictionary(m => m.Key, m => m.Value.Distinct().ToArray());
    }

    private static int CountInterconnected(string[] input)
    {
        var connected = new HashSet<ThreeNetwork>();
        var mapping = ParseGraph(input);

        foreach (var c0 in mapping.Keys)
        {
            if (mapping[c0].Length < 2)
                continue;
            var conn = mapping[c0];
            for (var i = 0; i < conn.Length; i++)
                for (var j = i + 1; j < conn.Length; j++)
                {
                    var c1 = conn[i];
                    var c2 = conn[j];
                    if (!mapping[c1].Contains(c2))
                        continue;
                    string[] computers = [c0, c1, c2];
                    Array.Sort(computers);
                    connected.Add((computers[0], computers[1], computers[2]));
                }
        }

        return connected.Count(x => x.C1.StartsWith('t') || x.C2.StartsWith('t') || x.C3.StartsWith('t'));
    }
}
