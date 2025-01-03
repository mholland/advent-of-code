namespace AdventOfCode.Day11;

public sealed class Day11(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 11);

    [Theory]
    [InlineData("0 1 10 99 999", 1, 7)]
    [InlineData("125 17", 25, 55312)]
    public void ExampleOne(string input, int blinks, int count) => StonesByCounts(input, blinks).Should().Be(count);

    [Fact]
    public async Task PartOne() => WriteOutput(StonesByCounts(await ReadInputFile(), 25));

    [Fact]
    public async Task PartTwo() => WriteOutput(StonesByCounts(await ReadInputFile(), 75));

    private static long StonesByCounts(string input, int blinks)
    {
        var stones = input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();
        Dictionary<long, long> stoneCounts = [];

        foreach (var stone in stones)
            stoneCounts.IncrementBy(stone, 1);

        var i = 0;
        while (i++ < blinks)
        {
            var newCounts = new Dictionary<long, long>(stoneCounts);
            foreach (var (stone, count) in stoneCounts)
            {
                if (stone == 0)
                {
                    newCounts.DecrementBy(stone, count);
                    newCounts.IncrementBy(1, count);
                    continue;
                }

                var val = stone.ToString();
                if (val.Length % 2 == 0)
                {
                    newCounts.IncrementBy(long.Parse(val[..(val.Length / 2)]), count);
                    newCounts.IncrementBy(long.Parse(val[(val.Length / 2)..]), count);
                    newCounts.DecrementBy(stone, count);
                    continue;
                }

                newCounts.DecrementBy(stone, count);
                newCounts.IncrementBy(stone * 2024, count);
            }

            stoneCounts = newCounts.Where(k => k.Value != 0).ToDictionary(x => x.Key, x => x.Value);
        }

        return stoneCounts.Aggregate(0L, (sum, kvp) => sum + kvp.Value);
    }

    // Too resource intensive for part 2!
    private static long CountStonesMechanically(string input, int blinks)
    {
        var stones = new LinkedList<long>(input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(long.Parse));
        for (var i = 0; i < blinks; i++)
        {
            var cur = stones.Last;
            while (cur is not null)
            {
                var val = cur.Value.ToString();
                if (cur.Value == 0)
                {
                    cur.Value = 1;
                }
                else if (val.Length % 2 == 0)
                {
                    cur.Value = long.Parse(val[..(val.Length / 2)]);
                    stones.AddLast(long.Parse(val[(val.Length / 2)..]));
                }
                else
                {
                    cur.Value *= 2024;
                }

                cur = cur.Previous;
            }
        }

        return stones.Count;
    }
}

public static class Extensions
{
    public static void IncrementBy(this Dictionary<long, long> self, long stone, long value)
    {
        if (self.TryGetValue(stone, out var count))
            self[stone] = count + value;
        else self.Add(stone, value);
    }

    public static void DecrementBy(this Dictionary<long, long> self, long stone, long value) =>
        self[stone] -= value;
}