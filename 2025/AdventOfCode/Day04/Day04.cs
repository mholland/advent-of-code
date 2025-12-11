namespace AdventOfCode.Day04;

public sealed class Day04(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 04);

    private readonly string[] _example =
    [
        "..@@.@@@@.",
        "@@@.@.@.@@",
        "@@@@@.@.@@",
        "@.@@@@..@.",
        "@@.@@@@.@@",
        ".@@@@@@@.@",
        ".@.@.@.@@@",
        "@.@@@.@@@@",
        ".@@@@@@@@.",
        "@.@.@@@.@."
    ];

    [Fact]
    public void ExampleOne() => CountReachableRolls(_example).Should().Be(13);

    
    [Fact]
    public void ExampleTwo() => RemoveableRolls(_example).Should().Be(43);

    [Fact]
    public async Task PartOne() => WriteOutput(CountReachableRolls(await ReadInputLines()));

    [Fact]
    public async Task PartTwo() => WriteOutput(RemoveableRolls(await ReadInputLines()));

    private static Grid ParseGrid(string[] input)
    {
        var grid = new Grid();
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
                grid[(x, y)] = input[y][x];
        
        return grid;
    }

    private static int RemoveableRolls(string[] input)
    {
        var grid = ParseGrid(input);

        var removed = 0;
        var currentGrid = grid;
        while (true)
        {
            var step = RemoveReachableRolls(currentGrid);
            var stepRolls = step.Count(g => g.Value == '@');
            var beforeRolls = currentGrid.Count(g => g.Value == '@');
            if (stepRolls == beforeRolls)
                break;
            removed += beforeRolls - stepRolls;
            currentGrid = step;
        }

        return removed;
    }

    private static int CountReachableRolls(string[] input)
    {
        var grid = ParseGrid(input);

        var before = grid.Count(g => g.Value == '@');
        var after = RemoveReachableRolls(grid);

        return before - after.Count(g => g.Value == '@');
    }

    private static Grid RemoveReachableRolls(Grid grid)
    {
        var newGrid = new Grid();
        foreach (var (pos, val) in grid)
        {
            if (val == '.')
            {
                newGrid.Add(pos, '.');
                continue;
            }
            var adjRolls = pos.Adjacent()
                .Count(p => grid.TryGetValue(p, out var pa) && pa == '@');

            newGrid[pos] = adjRolls < 4 ? '.' : '@';
        }

        return newGrid;
    }
}

public static class PosExtensions
{
    extension(Pos p)
    {
        public Pos Add(Pos other) => (p.X + other.X, p.Y + other.Y);
        public Pos[] Adjacent() => 
        [
            p.Add((-1, -1)), p.Add((0, -1)), p.Add((1, -1)),
            p.Add((-1, 0)), p.Add((1, 0)),
            p.Add((-1, 1)), p.Add((0, 1)), p.Add((1, 1))
        ];
    }
}
