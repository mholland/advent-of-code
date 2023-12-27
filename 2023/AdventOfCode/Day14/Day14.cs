using Grid = char[][];

namespace AdventOfCode.Day14;

public sealed class Day14(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day14";

    private readonly string[] _example =
    [
        "O....#....", // 10
        "O.OO#....#", // 9
        ".....##...", // 8
        "OO.#O....O", // 7
        ".O.....O#.", // 6
        "O.#..O.#.#", // 5
        "..O..#O..O", // 4
        ".......O..", // 3
        "#....###..", // 2
        "#OO..#...."  // 1
    ];

    [Fact]
    public void ExampleOne() => CalculateInitialLoadSum(_example).Should().Be(136);

    [Fact]
    public void PartOne() => WriteOutput(CalculateInitialLoadSum(Input));

    [Fact]
    public void ExampleTwo() => StressLoad(_example).Should().Be(64);

    [Fact]
    public void PartTwo() => WriteOutput(StressLoad(Input));

    private static int StressLoad(string[] input)
    {
        var states = new Dictionary<string, int>();
        var grid = input.Select(i => i.ToCharArray()).ToArray();
        var maxCycles = 1_000_000_000;
        var cycle = 1;
        while (cycle <= maxCycles)
        {
            for (var i = 0; i < 4; i++)
            {
                grid = Tilt(grid);
                grid = Rotate(grid);
            }
            if (states.TryGetValue(grid.Hash(), out var g))
            {
                var cycleLength = cycle - g;
                var numCycles = (maxCycles - g) / cycleLength;
                cycle = g + (numCycles * cycleLength);
            }
            states[grid.Hash()] = cycle;
            cycle++;
        }

        return CalculateLoad(grid);
    }

    private int CalculateInitialLoadSum(string[] input)
    {
        var grid = input.Select(i => i.ToCharArray()).ToArray();
        grid = Tilt(grid);
        return CalculateLoad(grid);
    }

    public static int CalculateLoad(Grid grid)
    {
        var load = 0;
        for (var y = 0; y < grid.Length; y++)
            for (var x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] == 'O')
                    load += grid.Length - y;
            }

        return load;
    }

    public static Grid Rotate(Grid grid)
    {
        var newGrid = Enumerable.Range(0, grid.Length)
            .Select(y => new char[grid[y].Length])
            .ToArray();
        var yy = grid.Length;
        for (var y = 0; y < grid.Length; y++)
        {
            var xx = grid[y].Length;
            for (var x = 0; x < xx; x++)
            {
                newGrid[x][yy - y - 1] = grid[y][x];
            }
        }

        return newGrid;
    }

    public static Grid Tilt(Grid grid)
    {
        for (var y = 0; y < grid.Length; y++)
            for (var x = 0; x < grid[y].Length; x++)
            {
                if (grid[y][x] != 'O') continue;
                var yy = y;
                while (yy > 0 && grid[yy - 1][x] == '.')
                {
                    grid[yy][x] = '.';
                    grid[yy - 1][x] = 'O';
                    yy--;
                }
            }

        return grid;
    }
}

public static class GridExtensions
{
    public static string Hash(this Grid grid) =>
        string.Join(Environment.NewLine, grid.Select(g => string.Join("", g)));
}