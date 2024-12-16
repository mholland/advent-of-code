using Pos = (int X, int Y);

namespace AdventOfCode.Day16;

public sealed class Day16(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 16);

    private readonly string[] _example =
    [
        "###############",
        "#.......#....E#",
        "#.#.###.#.###.#",
        "#.....#.#...#.#",
        "#.###.#####.#.#",
        "#.#.#.......#.#",
        "#.#.#####.###.#",
        "#...........#.#",
        "###.#.#####.#.#",
        "#...#.....#.#.#",
        "#.#.#.###.#.#.#",
        "#.....#...#.#.#",
        "#.###.#.#.#.#.#",
        "#S..#.....#...#",
        "###############"
    ];

    private readonly string[] _exampleTwo =
    [
        "#################",
        "#...#...#...#..E#",
        "#.#.#.#.#.#.#.#.#",
        "#.#.#.#...#...#.#",
        "#.#.#.#.###.#.#.#",
        "#...#.#.#.....#.#",
        "#.#.#.#.#.#####.#",
        "#.#...#.#.#.....#",
        "#.#.#####.#.###.#",
        "#.#.#.......#...#",
        "#.#.###.#####.###",
        "#.#.#...#.....#.#",
        "#.#.#.#####.###.#",
        "#.#.#.........#.#",
        "#.#.#.#########.#",
        "#S#.............#",
        "#################"
    ];

    [Fact]
    public void ExampleOne() => LowestScore(_example).Should().Be(7036);
    
    [Fact]
    public void ExampleTwo() => LowestScore(_exampleTwo).Should().Be(11048);

    [Fact]
    public async Task PartOne() => WriteOutput(LowestScore(await ReadInputLines()));

    private static int LowestScore(string[] input)
    {
        Dictionary<Pos, char> grid = [];
        var start = (-1, -1);
        
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            var c = input[y][x];
            grid[(x, y)] = c;
            if (c is not 'S') continue;
            
            start = (x, y);
            grid[(x, y)] = '.';
        }
        Pos[] dirs = [(0, -1), (1, 0), (0, 1), (-1, 0)];
        var lowest = int.MaxValue;
        var seen = new Dictionary<Pos, int>();
        var queue = new Queue<(Pos Pos, int Dir, int Cost)>([(start, 1, 0)]);
        while (queue.TryDequeue(out var current))
        {
            var (pos, dir, cost) = current;
            if (grid[pos] == 'E')
            {
                lowest = Math.Min(lowest, cost);
                continue;
            }
            if (seen.TryGetValue(pos, out var cst) && cst < cost)
                continue;
            seen[pos] = cost;

            var ahead = pos.To(dirs[dir]);
            if (grid.TryGetValue(ahead, out var a) && a is '.' or 'E')
                queue.Enqueue((ahead, dir, cost + 1));
            var ld = ((dir - 1) % dirs.Length + dirs.Length) % dirs.Length;
            var rd = ((dir + 1) % dirs.Length + dirs.Length) % dirs.Length;

            var left = pos.To(dirs[ld]);
            if (grid.TryGetValue(left, out var lc) && lc is not '#')
                queue.Enqueue((left, ld, cost + 1001));

            var right = pos.To(dirs[rd]);
            if (grid.TryGetValue(right, out var rc) && rc is not '#')
                queue.Enqueue((right, rd, cost + 1001));
        }
        return lowest;
    }
}

public static class Extensions
{
    public static Pos To(this Pos self, Pos dir) => (self.X + dir.X, self.Y + dir.Y);
}