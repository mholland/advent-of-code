namespace AdventOfCode.Day06;

public sealed class Day06(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day06";

    private readonly string[] _example =
    [
        "Time:      7  15   30",
        "Distance:  9  40  200"
    ];

    [Fact]
    public void ExampleOne() => CalculateBeatableRuns(_example).Should().Be(288);

    [Fact]
    public void PartOne() =>
        WriteOutput("Beatable runs: " + CalculateBeatableRuns(Input));

    [Fact]
    public void ExampleTwo() => CalculateBeatableRunsKerned(_example).Should().Be(71503);

    [Fact]
    public void PartTwo() =>
        WriteOutput("Beatable runs after kerning: " + CalculateBeatableRunsKerned(Input));

    private static int CalculateBeatableRuns(string[] input)
    {
        var times = ParseInput(input[0]);
        var distances = ParseInput(input[1]);

        return CalculateBeatableRuns(times, distances);

        static long[] ParseInput(string val) =>
            val.Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
    }

    private static int CalculateBeatableRunsKerned(string[] input)
    {
        var time = ParseInput(input[0]);
        var distance = ParseInput(input[1]);

        return CalculateBeatableRuns([time], [distance]);

        static long ParseInput(string val) =>
            long.Parse(val.Split(":")[1].Replace(" ", ""));
    }

    private static int CalculateBeatableRuns(long[] times, long[] distances)
    {
        var pairs = times.Zip(distances);
        var result = 1;
        foreach (var (time, distance) in pairs)
        {
            var beatRecord = 0;
            for (var t = 0; t <= time; t++)
            {
                if (t * (time - t) > distance)
                    beatRecord++;
            }
            result *= beatRecord;
        }

        return result;
    }
}
