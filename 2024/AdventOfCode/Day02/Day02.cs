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
    public void ExampleOne() => SafeReports(_example, 0).Should().Be(2);

    [Fact]
    public async Task PartOne() => WriteOutput(SafeReports(await ReadInputLines(), 0));

    private static int SafeReports(string[] input, int tolerance)
    {
        var safe = 0;
        foreach (var line in input)
        {
            var report = line.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            List<int> diffs = [];
            for (var i = 0; i < report.Length - 1; i++)
                diffs.Add(report[i] - report[i+1]);

            if (diffs.Select(x => Math.Sign(x)).Distinct().Count() == 1
                && diffs.Select(Math.Abs).All(x => x >= 1 && x <= 3))
                safe += 1;
        }

        return safe;
    }
}
