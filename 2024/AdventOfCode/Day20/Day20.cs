namespace AdventOfCode.Day20;

public sealed class Day20(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 20);

    private readonly string[] _example =
    [
        "###############",
        "#...#...#.....#",
        "#.#.#.#.#.###.#",
        "#S#...#.#.#...#",
        "#######.#.#.###",
        "#######.#.#...#",
        "#######.#.###.#",
        "###..E#...#...#",
        "###.#######.###",
        "#...###...#...#",
        "#.#####.#.###.#",
        "#.#...#.#.#...#",
        "#.#.#.#.#.#.###",
        "#...#...#...###",
        "###############"
    ];

    [Fact]
    public void ExampleOne() => PicosecondSaves(_example, 2);

    [Fact]
    public async Task PartOne() => WriteOutput(PicosecondSaves(await ReadInputLines(), 2));

    [Fact]
    public async Task PartTwo() => WriteOutput(PicosecondSaves(await ReadInputLines(), 20));

    private static int PicosecondSaves(string[] input, int allowedPhase)
    {
        Grid grid = [];
        var start = (X: -1, Y: -1);
        var end = (X: -1, Y: -1);
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
            {
                var c = input[y][x];
                if (c is 'S')
                    start = (x, y);
                if (c is 'E')
                    end = (x, y);
                grid[(x, y)] = c is '#' ? '#' : '.';
            }
        var dists = FindPath(grid, start, end);
        var path = dists.Keys;
        return dists
            .Sum(d => path
                .Where(p => Manhattan(d.Key, p) <= allowedPhase)
                .Count(x => dists[x] - dists[d.Key] - Manhattan(d.Key, x) >= 100));

        static int Manhattan(Pos p1, Pos p2) => Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }

    private static Dictionary<Pos, int> FindPath(Grid grid, Pos start, Pos end)
    {
        var dists = new Dictionary<Pos, int>
        {
            [start] = 0
        };
        var queue = new PriorityQueue<Pos, int>([(start, 0)]);
        while (queue.TryDequeue(out var pos, out var dist))
        {
            if (pos == end)
                break;

            Pos[] neighbours = [(0, -1), (1, 0), (0, 1), (-1, 0)];
            foreach (var (nx, ny) in neighbours)
            {
                var next = (pos.X + nx, pos.Y + ny);
                if (grid.TryGetValue(next, out var nextTile) && (nextTile == '.'))
                {
                    if (!dists.TryGetValue(next, out var nd) || dist + 1 < nd)
                    {
                        queue.Enqueue(next, dist + 1);
                        dists[next] = dist + 1;
                    }
                }
            }
        }

        return dists;
    }
}
