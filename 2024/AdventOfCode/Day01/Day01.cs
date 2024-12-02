namespace AdventOfCode.Day01;

public class Day01(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 01);

    private readonly string[] _example =
    [
        "3   4",
        "4   3",
        "2   5",
        "1   3",
        "3   9",
        "3   3"
    ];

    [Fact]
    public void ExampleOne() => CalculateScores(_example).Diff.Should().Be(11);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculateScores(await ReadInputLines()).Diff);

    [Fact]
    public void ExampleTwo() => CalculateScores(_example).Similarity.Should().Be(31);
    
    [Fact]
    public async Task PartTwo() => WriteOutput(CalculateScores(await ReadInputLines()).Similarity);

    private static (int Diff, int Similarity) CalculateScores(string[] input)
    {
        List<int> left = [];
        List<int> right = [];

        foreach (var line in input)
        {
            var split = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            left.Add(int.Parse(split[0]));
            right.Add(int.Parse(split[1]));
        }

        var rightFreq = right.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

        var diff = left.Order().Zip(right.Order()).Sum(x => Math.Abs(x.First - x.Second));
        var similarity = left.Where(right.Contains).Sum(x => x * rightFreq[x]);
        return (diff, similarity);
    }
}