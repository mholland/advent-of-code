namespace AdventOfCode.Day19;

public sealed class Day19(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 19);

    private readonly string[] _example =
    [
        "r, wr, b, g, bwu, rb, gb, br",
        "",
        "brwrr",
        "bggr",
        "gbbr",
        "rrbgbr",
        "ubwu",
        "bwurrg",
        "brgr",
        "bbrgwb",
    ];

    [Fact]
    public void ExampleOne() => CountConstructableDesigns(_example).Total.Should().Be(6);

    [Fact]
    public async Task PartOne() => WriteOutput(CountConstructableDesigns(await ReadInputLines()).Total);

    [Fact]
    public void ExampleTwo() => CountConstructableDesigns(_example).Ways.Should().Be(16);

    [Fact]
    public async Task PartTwo() => WriteOutput(CountConstructableDesigns(await ReadInputLines()).Ways);

    private (int Total, long Ways) CountConstructableDesigns(string[] input)
    {
        var patterns = input[0].Split(", ", StringSplitOptions.RemoveEmptyEntries);
        var designs = input[2..];
        var cache = new Dictionary<string, long>();
        var constructable = designs.Select(IsConstructable).ToArray();
        var total = constructable.Count(d => d > 0);
        var ways = constructable.Sum();

        return (total, ways);

        long IsConstructable(string design)
        {
            if (cache.TryGetValue(design, out var value))
                return value;
                
            if (design == string.Empty)
                return 1;

            var constr = patterns.Where(design.StartsWith).Sum(x => IsConstructable(design[x.Length..]));
            cache.Add(design, constr);
            return constr;
        }
    }
}
