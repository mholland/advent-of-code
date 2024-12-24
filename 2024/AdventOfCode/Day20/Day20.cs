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
    public void ExampleOne() => PicosecondSaves(_example);

    [Fact]
    public async Task PartOne() => WriteOutput(PicosecondSaves(await ReadInputLines()));

    private static int PicosecondSaves(string[] input)
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
        var bestPath = FindPath(grid, start, end, (-1, -1));
        List<int> paths = [];
        foreach (var (pos, _) in grid.Where(g => g.Value == '#'))
        {
            paths.Add(FindPath(grid, start, end, pos));
        };
        return paths.Count(x => bestPath - x >= 100);
    }

    private static int FindPath(Grid grid, Pos start, Pos end, Pos phaseThrough)
    {
        var dists = new Dictionary<Pos, int>
        {
            [start] = 0
        };
        var queue = new PriorityQueue<Pos, int>([(start, 0)]);
        while (queue.TryDequeue(out var pos, out var dist))
        {
            if (pos == end)
                return dist;

            Pos[] neighbours = [(0, -1), (1, 0), (0, 1), (-1, 0)];
            foreach (var (nx, ny) in neighbours)
            {
                var next = (pos.X + nx, pos.Y + ny);
                if (grid.TryGetValue(next, out var nextTile) && (nextTile == '.' || next == phaseThrough))
                {
                    if (!dists.TryGetValue(next, out var nd) || dist + 1 < nd)
                    {
                        queue.Enqueue(next, dist + 1);
                        dists[next] = dist + 1;
                    }
                }
            }
        }

        return -1;
    }
}
