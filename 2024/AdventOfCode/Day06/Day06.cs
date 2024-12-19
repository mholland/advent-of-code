namespace AdventOfCode.Day06;

public sealed class Day06(ITestOutputHelper output) : TestBase(output)
{
    private static readonly Dictionary<Pos, Pos> NextDirs = new()
    {
        [(0, -1)] = (1, 0),
        [(1, 0)] = (0, 1),
        [(0, 1)] = (-1, 0),
        [(-1, 0)] = (0, -1)
    };

    protected override DateOnly Date => new(2024, 12, 06);

    private readonly string[] _example =
    [
        "....#.....",
        ".........#",
        "..........",
        "..#.......",
        ".......#..",
        "..........",
        ".#..^.....",
        "........#.",
        "#.........",
        "......#..."
    ];

    private readonly string[] _corners =
    [
        ".............",
        "...#.........",
        ".........#...",
        "........#....",
        ".............",
        "...^.........",
        ".............",
        "..#..........",
        "........#....",
        "............."

    ];

    [Fact]
    public void ExampleOne() => CountPositions(_example).Should().Be(41);

    [Fact]
    public async Task PartOne() => WriteOutput(CountPositions(await ReadInputLines()));
    
    [Fact]
    public void ExampleTwo() => CountObstacles(_example).Should().Be(6);

    [Fact]
    public void Corners()
    {
        var (grid, start) = ParseGrid(_corners);
        var (_, inLoop) = MapPatrol(start, grid);

        inLoop.Should().BeFalse();
    }
    
    [Fact]
    public async Task PartTwo() => WriteOutput(CountObstacles(await ReadInputLines()));

    private static int CountPositions(string[] input)
    {
        var (grid, start) = ParseGrid(input);

        return MapPatrol(start, grid).Steps.Count();
    }

    private static int CountObstacles(string[] input)
    {
        var (grid, start) = ParseGrid(input);
        
        var potentialLocations = MapPatrol(start, grid).Steps;
        var obstacles = 0;
        foreach (var potential in potentialLocations.Except([start]))
        {
            var newGrid = grid.ToDictionary(x => x.Key, x => x.Value);
            newGrid[(potential.X, potential.Y)] = '#';
            var (_, inLoop) = MapPatrol(start, newGrid);
            if (!inLoop) continue;
            obstacles += 1;
        }
        
        return obstacles;
    }

    private static (IEnumerable<Pos> Steps, bool InLoop) MapPatrol(Pos start, Grid grid)
    {
        var pos = start;
        var dir = (0, -1);
        var seen = new HashSet<(Pos Pos, Pos Dir)>();
        while (true)
        {
            if (!seen.Add((pos, dir)))
                return (seen.Select(p => p.Pos).Distinct(), true);
            var nextPos = pos.Add(dir);
            if (!grid.TryGetValue(nextPos, out var n))
                return (seen.Select(p => p.Pos).Distinct(), false);
            if (n == '#')
            {
                dir = NextDirs[dir];
                continue;
            }

            pos = nextPos;
        }
    }

    private static (Grid grid, Pos pos) ParseGrid(string[] input)
    {
        Grid grid = [];
        Pos pos = default;
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            grid[(x, y)] = input[y][x];
            if (input[y][x] != '^') continue;
            pos = (x, y);
            grid[(x, y)] = '.';
        }

        return (grid, pos);
    }
}

public static class Extensions
{
    public static Pos Add(this Pos pos, Pos dir) =>
        (pos.X + dir.X, pos.Y + dir.Y);
}
