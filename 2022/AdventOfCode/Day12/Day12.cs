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

    private int CalculateShortestDistance(string[] input)
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

        var queue = new Queue<(int X, int Y, int Dist, int Height)>();
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