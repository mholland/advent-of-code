namespace AdventOfCode.Day12;

public sealed class Day12 : TestBase
{
    protected override string Day => "Day12";
    
    public Day12(ITestOutputHelper output) 
        : base(output)
    {
    }

    [Fact]
    public void ExampleOne() => CalculateShortestDistance(new[]
    {
        "Sabqponm",
        "abcryxxl",
        "accszExk",
        "acctuvwj",
        "abdefghi"
    }).Should().Be(31);

    [Fact]
    public void PartOne() => Output.WriteLine($"Shortest: {CalculateShortestDistance(Input)}");

    [Fact]
    public void ExampleTwo() => CalculateOverallShortestDistance(new[]
    {
        "Sabqponm",
        "abcryxxl",
        "accszExk",
        "acctuvwj",
        "abdefghi"
    }).Should().Be(29);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Shortest: {CalculateOverallShortestDistance(Input)}");

    private (IDictionary<(int X, int Y), int> Grid, (int X, int Y) Start, (int X, int Y) End) ParseGrid(string[] input)
    {
                var grid = new Dictionary<(int X, int Y), int>();
        (int X, int Y) start = default;
        (int X, int Y) end = default;
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input.Max(l => l.Length); x++)
        {
            var height = input[y][x];
            var coord = (x, y);
            if (height == 'S')
            {
                start = coord;
                height = 'a';
            }
            if (height == 'E')
            {
                end = coord;
                height = 'z';
            }
            grid.Add(coord, height - 'a' + 1);
        }

        return (grid, start, end);
    }

    private int CalculateOverallShortestDistance(string[] input)
    {
        var (grid, _, end) = ParseGrid(input);
        var starts = grid.Where(x => x.Value == 1).Select(x => x.Key).ToArray();

        return ShortestDistance(grid, starts, end);
    }

    private int CalculateShortestDistance(string[] input)
    {
        var (grid, start, end) = ParseGrid(input);

        return ShortestDistance(grid, new[] { start }, end);
    }

    private int ShortestDistance(IDictionary<(int X, int Y), int> grid, (int X, int Y)[] starts, (int X, int Y) end)
    {
        var queue = new Queue<(int X, int Y, int Dist, int Height)>();
        foreach (var start in starts)
            queue.Enqueue((start.X, start.Y, 0, 1));
        var visited = new HashSet<(int X, int Y)>();
        var dirs = new(int X, int Y)[]{ (1, 0), (-1, 0), (0, 1), (0, -1) };

        while (queue.TryDequeue(out var current))
        {
            if ((current.X, current.Y) == end)
                return current.Dist;
            foreach (var dir in dirs)
            {
                var potentialPath = (X: current.X + dir.X, Y: current.Y + dir.Y);
                if (grid.TryGetValue(potentialPath, out var dest) && !visited.Contains(potentialPath) && dest <= current.Height + 1)
                {
                    queue.Enqueue((potentialPath.X, potentialPath.Y, current.Dist + 1, dest));
                    visited.Add((potentialPath.X, potentialPath.Y));
                }
            }
        }

        return -1;
    }
}