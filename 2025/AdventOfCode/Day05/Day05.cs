using System.Diagnostics;
using Range = (long From, long To);

namespace AdventOfCode.Day05;

public sealed class Day05(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 05);

    private readonly string[] _example =
    [
        "3-5",
        "10-14",
        "16-20",
        "12-18",
        "",
        "1",
        "5",
        "8",
        "11",
        "17",
        "32"
    ];

    [Fact]
    public void ExampleOne() => CountFreshIngredients(_example).Should().Be(3);

    [Fact]
    public void ExampleTwo() => CountFreshIngredientIds(_example).Should().Be(14);

    [Fact]
    public async Task PartOne() => WriteOutput(CountFreshIngredients(await ReadInputLines()));

    [Fact]
    public async Task PartTwo() => WriteOutput(CountFreshIngredientIds(await ReadInputLines()));

    public static long CountFreshIngredientIds(string[] input)
    {
        var separator = Array.IndexOf(input, string.Empty);
        var rawRanges = input[..separator];

        // u1             u2
        // |--------------|
        //        |-------------|
        //        v1            v2
        //
        // |---------|    v1        v2
        // u1        u2   |---------|
        // 
        // u1                    u2
        // |---------------------|
        //       |----------|
        //       v1         v2
        // max(u1, v1) < min(u2, v2)

        var ranges = rawRanges.Select(r => r.Split('-'))
            .Select(r => (From: long.Parse(r[0]), To: long.Parse(r[1])))
            .Distinct()
            .OrderBy(r => r.From)
            .ToList();

        var ids = 0L;
        var current = ranges[0];
        for (var i = 1; i < ranges.Count; i++)
        {
            if (current.Intersects(ranges[i]))
            {
                current = current.Merge(ranges[i]);
                continue;
            }

            ids += current.To - current.From + 1;
            current = ranges[i];
        }

        ids += current.To - current.From + 1;

        return ids;
    }

    private static int CountFreshIngredients(string[] input)
    {
        var separator = Array.IndexOf(input, string.Empty);
        var rawRanges = input[..separator];
        var ingredients = input[(separator + 1)..].Select(long.Parse);

        var ranges = rawRanges.Select(r => r.Split('-'))
            .Select(r => (From: long.Parse(r[0]), To: long.Parse(r[1])))
            .ToArray();

        return ingredients.Count(i => ranges.Any(r => i >= r.From && i <= r.To));
    }
}

public static class RangeExtensions
{
    extension(Range r)
    {
        public bool Intersects(Range other) =>
            Math.Max(r.From, other.From) <= Math.Min(r.To, other.To);

        public Range Merge(Range other) =>
            (Math.Min(r.From, other.From), Math.Max(r.To, other.To));
    }
}