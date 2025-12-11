namespace AdventOfCode.Day03;

public sealed class Day03(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 03);

    private readonly string[] _example =
    [
        "987654321111111",
        "811111111111119",
        "234234234234278",
        "818181911112111"
    ];

    [Fact]
    public void ExampleOne() => MaximumUnrestrictedJoltage(_example, 2).Should().Be(357);

    [Fact]
    public async Task PartOne() => WriteOutput(MaximumUnrestrictedJoltage(await ReadInputLines(), 2));

    [Fact]
    public async Task PartTwo() => WriteOutput(MaximumUnrestrictedJoltage(await ReadInputLines(), 12));

    [Fact]
    public void ExampleTwo() =>
        MaximumUnrestrictedJoltage(_example, 12).Should().Be(3121910778619);

    private static long MaximumUnrestrictedJoltage(string[] input, int batterySize)
    {
        var banks = input.Select(i => i.Select(ii => ii - '0').ToArray()).ToArray();

        var maxJoltage = 0L;
        foreach (var bank in banks)
        {
            var chosen = new List<int>(batterySize);
            var b = bank;

            var mustLeave = batterySize - 1;
            var checkUpTo = banks.Max(x => x.Length) - mustLeave;
            var from = 0;
            while (chosen.Count < batterySize)
            {
                var max = b[from..checkUpTo].Max();
                chosen.Add(max);
                var index = Array.IndexOf(b[from..], max);
                from = from + index + 1;
                checkUpTo += 1;
            }

            var mul = 1L;
            var joltage = 0L;
            for (var i = chosen.Count - 1; i >= 0; i--)
            {
                joltage += chosen[i] * mul;
                mul *= 10;
            }
            maxJoltage += joltage;
        }

        return maxJoltage;
    }
}
