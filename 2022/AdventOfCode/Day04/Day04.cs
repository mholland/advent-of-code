namespace AdventOfCode.Day04;

public sealed class Day04 : TestBase
{
    protected override string Day => "Day04";
    public Day04(ITestOutputHelper output)
        : base(output)
    {
    }

    private readonly string[] _example = 
    {
        "2-4,6-8",
        "2-3,4-5",
        "5-7,7-9",
        "2-8,3-7",
        "6-6,4-6",
        "2-6,4-8"
    };

    [Fact]
    public void ExampleOne() => CalculateEncompassingRanges(_example).Should().Be(2);

    [Fact]
    public void PartOne() => Output.WriteLine($"Contained within: {CalculateEncompassingRanges(Input)}");

    [Fact]
    public void ExampleTwo() => CalculateOverlappingRanges(_example).Should().Be(4);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Overlapping: {CalculateOverlappingRanges(Input)}");

    private static IEnumerable<(Range First, Range Second)> ParseRanges(string[] input)
    {
        return input.Select(l => l.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(r => (First: ParseRange(r[0]), Second: ParseRange(r[1])));

        Range ParseRange(string rng)
        {
            var split = rng.Split('-');
            return new Range(int.Parse(split[0]), int.Parse(split[1]));
        }
    }

    private static int CalculateEncompassingRanges(string[] input) => 
        ParseRanges(input)
            .Count(r => r.First.Contains(r.Second) || r.Second.Contains(r.First));

    private static int CalculateOverlappingRanges(string[] input) =>
        ParseRanges(input)
            .Count(r => r.First.Overlaps(r.Second));
}

public static class RangeExtensions
{
    public static bool Contains(this Range range, Range other) =>
        other.Start.Value >= range.Start.Value && other.End.Value <= range.End.Value;

    public static bool Overlaps(this Range range, Range other) =>
        range.Start.Value <= other.End.Value && range.End.Value >= other.Start.Value;
}