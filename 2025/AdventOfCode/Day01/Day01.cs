namespace AdventOfCode.Day01;

public sealed class Day01(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 01);

    private readonly string[] _example =
    [
        "L68",
        "L30",
        "R48",
        "L5",
        "R60",
        "L55",
        "L1",
        "L99",
        "R14",
        "L82"
    ];

    [Fact]
    public void ExampleOne() => CalculatePassword(_example).Older.Should().Be(3);

    [Fact]
    public void ExampleTwo() => CalculatePassword(_example).Newer.Should().Be(6);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculatePassword(await ReadInputLines()).Older);

    [Fact]
    public async Task PartTwo() => WriteOutput(CalculatePassword(await ReadInputLines()).Newer);

    private static (int Older, int Newer) CalculatePassword(string[] input)
    {
        var point = 50;
        var instructions = input.Select(i => (Direction: i[0], Times: int.Parse(i[1..]))).ToArray();
        var olderSecurity = 0;
        var newerSecurity = 0;
        foreach (var (dir, times) in instructions)
        {
            for (var i = 0; i < times; i++)
            {
                var t = dir == 'R' ? 1 : -1;
                point = (point + t + 100) % 100;
                if (point == 0)
                    newerSecurity += 1;
            }

            if (point == 0)
                olderSecurity += 1;
        }

        return (olderSecurity, newerSecurity);
    }
}
