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
    public void ExampleOne() => LowestScore(_example).Score.Should().Be(7036);
    
    [Fact]
    public void ExampleTwo() => LowestScore(_exampleTwo).Score.Should().Be(11048);

    [Fact]
    public async Task PartOne() => WriteOutput(LowestScore(await ReadInputLines()).Score);

    [Fact]
    public void ExampleThree() => LowestScore(_example).Spots.Should().Be(45);
    
    [Fact]
    public void ExampleFour() => LowestScore(_exampleTwo).Spots.Should().Be(64);

    [Fact]
    public async Task PartTwo() => WriteOutput(LowestScore(await ReadInputLines()).Spots);

    private static (int Score, int Spots) LowestScore(string[] input)
    {
        Grid grid = [];
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
        var seen = new Dictionary<(Pos, int), int>();
        var routeCosts = new List<(int Cost, HashSet<Pos> Route)>();
        var queue = new Queue<(Pos Pos, int Dir, int Cost, HashSet<Pos> Seen)>([(start, 1, 0, [start])]);
        while (queue.TryDequeue(out var current))
        {
            var (pos, dir, cost, hs) = current;
            if (grid[pos] == 'E')
            {
                routeCosts.Add((cost, hs));
                lowest = Math.Min(lowest, cost);
                continue;
            }
            if (seen.TryGetValue((pos, dir), out var cst) && cst < cost || cost > lowest)
                continue;
            seen[(pos, dir)] = cost;

            var ahead = pos.To(dirs[dir]);
            if (grid.TryGetValue(ahead, out var a) && a is '.' or 'E')
                queue.Enqueue((ahead, dir, cost + 1, [..hs, ahead]));
            var ld = ((dir - 1) % dirs.Length + dirs.Length) % dirs.Length;
            var rd = ((dir + 1) % dirs.Length + dirs.Length) % dirs.Length;

            var left = pos.To(dirs[ld]);
            if (grid.TryGetValue(left, out var lc) && lc is not '#')
                queue.Enqueue((left, ld, cost + 1001, [..hs, left]));

            var right = pos.To(dirs[rd]);
            if (grid.TryGetValue(right, out var rc) && rc is not '#')
                queue.Enqueue((right, rd, cost + 1001, [..hs, right]));
        }

        var bests = routeCosts.Where(r => r.Cost == lowest);
        var positions = bests.Aggregate(new HashSet<Pos>(), (agg, cur) =>
        {
            agg.UnionWith(cur.Route);
            return agg;
        });
        
        return (lowest, positions.Count);
    }
}

public static class Extensions
{
    public static Pos To(this Pos self, Pos dir) => (self.X + dir.X, self.Y + dir.Y);
}