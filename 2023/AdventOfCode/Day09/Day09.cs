using Steps = System.Collections.Generic.List<System.Collections.Generic.List<int>>;
namespace AdventOfCode.Day09;

public sealed class Day09(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day09";

    private readonly string[] _example =
    [
        "0 3 6 9 12 15",
        "1 3 6 10 15 21",
        "10 13 16 21 30 45",
    ];

    [Fact]
    public void ExampleOne() => NextValueSum(_example).Right.Should().Be(114);

    [Fact]
    public void ExampleTwo() => NextValueSum(_example[2..]).Left.Should().Be(5);

    [Fact]
    public void PartOne() =>
        WriteOutput("Right: " + NextValueSum(Input).Right);

    [Fact]
    public void PartTwo() =>
        WriteOutput("Left: " + NextValueSum(Input).Left);

    private static (int Left, int Right) NextValueSum(string[] input)
    {
        var sets = input.Select(i => new Steps() { i.Split(' ').Select(int.Parse).ToList() }).ToArray();
        foreach (var set in sets)
        {
            var current = set.First();

            while (!current.All(c => c == 0))
            {
                var newStep = new List<int>();
                for (var i = 1; i < current.Count; i++)
                {
                    newStep.Add(current[i] - current[i - 1]);
                }
                current = newStep;
                set.Add(current);
            }

            for (var i = set.Count - 2; i >= 0; i--)
            {
                set[i].Add(set[i + 1][^1] + set[i][^1]);
                set[i].Insert(0, set[i][0] - set[i + 1][0]);
            }
        }

        return (Left: sets.Sum(s => s[0][0]), Right: sets.Sum(s => s[0][^1]));
    }
}
