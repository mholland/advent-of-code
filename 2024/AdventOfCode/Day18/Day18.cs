namespace AdventOfCode.Day18;

public class Day18(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 18);

    private readonly string[] _example =
    [
        "5,4",
        "4,2",
        "4,5",
        "3,0",
        "2,1",
        "6,3",
        "2,4",
        "1,5",
        "0,6",
        "3,3",
        "2,6",
        "5,1",
        "1,2",
        "5,5",
        "2,5",
        "6,5",
        "1,4",
        "0,4",
        "6,4",
        "1,1",
        "6,1",
        "1,0",
        "0,5",
        "1,6",
        "2,0"
    ];

    [Fact]
    public void ExampleOne() => ShortestDistance(_example, 6, 6, 12).Should().Be(22);

    [Fact]
    public async Task PartOne() => WriteOutput(ShortestDistance(await ReadInputLines(), 70, 70, 1024));

    [Fact]
    public void ExampleTwo() => FirstBlockage(_example, 6, 6, 12).Should().Be("6,1");
    
    [Fact]
    public async Task PartTwo() => WriteOutput(FirstBlockage(await ReadInputLines(), 70, 70, 1024));

    private static string FirstBlockage(string[] input, int w, int h, int skip)
    {
        var bytes = ParseBytes(input);
        var grid = ConstructGrid(bytes, w, h, skip);
        var start = (0, 0);
        var end = (w, h);

        for (var s = skip; s < bytes.Length; s++)
        {
            var @byte = bytes[s];
            grid[@byte] = '#';
            var (reachable, _) = ExitReachable(grid, start, end);
            if (!reachable)
                return $"{@byte.X},{@byte.Y}";
        }

        return "Not found";
    }

    private static int ShortestDistance(string[] input, int w, int h, int toTake)
    {
        var bytes = ParseBytes(input).Take(toTake)
            .ToArray();
        var grid = ConstructGrid(bytes, w, h, toTake);
        var start = (0, 0);
        var end = (w, h);

        var (_, distance) = ExitReachable(grid, start, end);

        return distance;
    }

    private static Pos[] ParseBytes(string[] input) =>
        input.Select(i => i.Split(",")).Select(x => (X: int.Parse(x[0]), Y: int.Parse(x[1]))).ToArray();
    
    private static Grid ConstructGrid(Pos[] bytes, int w, int h, int toTake)
    {
        Grid grid = [];
        var subset = bytes.Take(toTake).ToArray();
        for (var y = 0; y < h + 1; y++)
        {
            for (var x = 0; x < w + 1; x++)
            {
                grid[(x, y)] = subset.Contains((x, y)) ? '#' : '.';
            }
        }

        return grid;
    }

    private static (bool Reachable, int Distance) ExitReachable(Grid grid, Pos start,
        Pos end)
    {
        var queue = new Queue<(Pos Pos, int Distance)>([(start, 0)]);
        var seen = new HashSet<Pos>();
        while (queue.TryDequeue(out var current))
        {
            var (pos, distance) = current;
            if (!seen.Add(pos))
                continue;
            if (pos == end)
            {
                return (true, distance);
            }

            Pos[] neighbours = [(0, -1), (1, 0), (0, 1), (-1, 0)];
            foreach (var neighbour in neighbours)
            {
                var next = (pos.X + neighbour.X, pos.Y + neighbour.Y);
                if (grid.TryGetValue(next, out var n) && n == '.')
                    queue.Enqueue((next, distance + 1));
            }
        }

        return (false, -1);
    }

    private void PrintGrid(Grid grid)
    {
        var output = "\n";
        for (var y = 0; y <= grid.Max(g => g.Key.Y); y++)
        {
            var line = "";
            for (var x = 0; x <= grid.Max(g => g.Key.X); x++)
            {
                line += grid[(x, y)];
            }

            output += line + "\n";
        }

        WriteOutput(output);
    }
}