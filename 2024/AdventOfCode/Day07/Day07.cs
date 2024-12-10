namespace AdventOfCode.Day07;

public sealed class Day07(ITestOutputHelper output) : TestBase(output)
{
    private static readonly Func<long, long, long>[] PartOneOperators = 
    [
        (x, y) => x + y,
        (x, y) => x * y,
    ];

    private static readonly Func<long, long, long>[] PartTwoOperators =
    [
        ..PartOneOperators,
        (x, y) => long.Parse(x + y.ToString())
    ];

    protected override DateOnly Date => new(2024, 12, 07);

    private readonly string[] _example =
    [
        "190: 10 19",
        "3267: 81 40 27",
        "83: 17 5",
        "156: 15 6",
        "7290: 6 8 6 15",
        "161011: 16 10 13",
        "192: 17 8 14",
        "21037: 9 7 18 13",
        "292: 11 6 16 20"
    ];

    [Fact]
    public void ExampleOne() => CalibrationResult(_example, PartOneOperators).Should().Be(3749);

    [Fact]
    public async Task PartOne() => WriteOutput(CalibrationResult(await ReadInputLines(), PartOneOperators));
    
    [Fact]
    public void ExampleTwo() => CalibrationResult(_example, PartTwoOperators).Should().Be(11387);
    
    [Fact]
    public async Task PartTwo() => WriteOutput(CalibrationResult(await ReadInputLines(), PartTwoOperators));

    private static long CalibrationResult(string[] input, Func<long, long, long>[] operators)
    {
        var calibration = 0L;
        foreach (var line in input)
        {
            var split = line.Split(':');
            var result = long.Parse(split[0]);
            var operands = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
            var totals = new Stack<(long Total, int OpIdx)>();
            totals.Push((operands[0], 0));
            while (totals.TryPop(out var t))
            {
                if (t.OpIdx == operands.Length - 1)
                {
                    if (t.Total == result)
                    {
                        calibration += t.Total;
                        break;
                    }

                    continue;
                }

                foreach (var op in operators)
                {
                    var newTotal = op.Invoke(t.Total, operands[t.OpIdx + 1]);
                    if (newTotal > result) continue;
                    totals.Push((newTotal, t.OpIdx + 1));
                }
            }
        }
        
        return calibration;
    }
}
