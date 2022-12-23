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
    public void ExampleOne() => FindCorrectIndices(Example).Should().Be(13);

    private int FindCorrectIndices(string[] input)
    {
        var chunks = Enumerable.Chunk(input.Where(x => !string.IsNullOrWhiteSpace(x)), 2);
        var packets = chunks.Select(x => (First: JsonArray.Parse(x[0])!, Second: JsonArray.Parse(x[1])!));
        return 0;
    }
}
