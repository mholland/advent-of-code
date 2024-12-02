namespace AdventOfCode.Day02;

public sealed class Day02(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 02);

    private readonly string[] _example =
    [
        "7 6 4 2 1",
        "1 2 7 8 9",
        "9 7 6 2 1",
        "1 3 2 4 5",
        "8 6 4 4 1",
        "1 3 6 7 9"
    ];

    [Fact]
    public void ExampleOne() => SafeReports(_example).Should().Be(2);

    [Fact]
    public async Task PartOne() => WriteOutput(SafeReports(await ReadInputLines()));

    [Fact]
    public void IndexMusings()
    {
        int[] foo = [1, 2, 3, 4, 5];
        foo[..1].Length.Should().Be(1);
        foo[2..].Length.Should().Be(3);
        int[] bar = [..foo[..1], ..foo[2..]];
        bar.Length.Should().Be(4);
        bar.Should().BeEquivalentTo([1, 3, 4, 5]);
    }

    [Fact]
    public void ExampleTwo() => SafeReports(_example, true).Should().Be(4);

    [Fact]
    public async Task PartTwo() => WriteOutput(SafeReports(await ReadInputLines(), true));

    private static int SafeReports(string[] input, bool tolerant = false)
    {
        var reports = input
            .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse).ToArray())
            .ToArray();
        var safe = reports.Count(IsSafe);
        if (!tolerant) return safe;

        var @unsafe = reports.Where(r => !IsSafe(r));
        safe += @unsafe
            .Count(report =>
                report.Select((_, i) => report.Without(i)).Any(IsSafe));

        return safe;

        bool IsSafe(int[] report)
        {
            var direction = Math.Sign(report[0] - report[1]);
            for (var i = 0; i < report.Length - 1; i++)
            {
                var diff = Math.Abs(report[i] - report[i + 1]);
                if (diff is < 1 or > 3) return false;
                if (Math.Sign(report[i] - report[i + 1]) != direction) return false;
            }

            return true;
        }
    }
}

public static class Extensions
{
    public static int[] Without(this int[] self, int index) => [..self[..index], ..self[(index + 1)..]];
}