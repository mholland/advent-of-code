using Pos = (int X, int Y);

namespace AdventOfCode.Day10;

public sealed class Day10(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 10);

    private readonly string[] _example =
    [
        "89010123",
        "78121874",
        "87430965",
        "96549874",
        "45678903",
        "32019012",
        "01329801",
        "10456732"
    ];

    [Fact]
    public void ExampleOne() => TrailheadAnalysis(_example).Scores.Should().Be(36);

    [Fact]
    public async Task PartOne() => WriteOutput(TrailheadAnalysis(await ReadInputLines()).Scores);

    [Fact]
    public void ExampleTwo() => TrailheadAnalysis(_example).Ratings.Should().Be(81);

    [Fact]
    public async Task PartTwo() => WriteOutput(TrailheadAnalysis(await ReadInputLines()).Ratings);

    private static (int Scores, int Ratings) TrailheadAnalysis(string[] input)
    {
        Dictionary<Pos, int> grid = [];
        List<Pos> trailheads = [];
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            grid[(x, y)] = input[y][x] - '0';
            if (grid[(x, y)] == 0)
                trailheads.Add((x, y));
        }

        var scores = 0;
        var ratings = 0;
        foreach (var trailhead in trailheads)
        {
            var stack = new Stack<Pos>([trailhead]);
            List<Pos> ends = [];
            while (stack.TryPop(out var pos))
            {
                if (grid[pos] == 9)
                {
                    ends.Add(pos);
                    continue;
                }

                Pos[] directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];
                foreach (var dir in directions)
                {
                    var neighbour = pos.Add(dir);
                    if (!grid.TryGetValue(neighbour, out var n) || n != grid[pos] + 1)
                        continue;
                    stack.Push(neighbour);
                }
            }

            scores += ends.Distinct().Count();
            ratings += ends.Count;
        }

        return (scores, ratings);
    }
}

public static class Extensions
{
    public static Pos Add(this Pos self, Pos dir) =>
        (self.X + dir.X, self.Y + dir.Y);
}