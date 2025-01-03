namespace AdventOfCode.Day08;

public sealed class Day08(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 08);

    private readonly string[] _example =
    [
        "............",
        "........0...",
        ".....0......",
        ".......0....",
        "....0.......",
        "......A.....",
        "............",
        "............",
        "........A...",
        ".........A..",
        "............",
        "............"
    ];

    [Fact]
    public void ExampleOne() => CountAntinodes(_example).Should().Be(14);
    
    [Fact]
    public async Task PartOne() => WriteOutput(CountAntinodes(await ReadInputLines()));
    
    [Fact]
    public void ExampleTwo() => CountAntinodes(_example, true).Should().Be(34);

    [Fact]
    public async Task PartTwo() => WriteOutput(CountAntinodes(await ReadInputLines(), true));

    private int CountAntinodes(string[] input, bool resonantHarmonics = false)
    {
        Grid grid = [];
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            grid[(x, y)] = input[y][x];
        }

        var antinodes = new HashSet<Pos>();
        foreach (var (pos, val) in grid.Where(kvp => kvp.Value != '.'))
        {
            var others = grid.Where(kvp => kvp.Value == val && kvp.Key != pos);
            foreach (var (other, _) in others)
            {
                var diff = (X: pos.X - other.X, Y: pos.Y - other.Y);
                var antinode = (X: pos.X + diff.X, Y: pos.Y + diff.Y);
                if (grid.ContainsKey(antinode)) 
                    antinodes.Add(antinode);
                if (!resonantHarmonics) continue;
                antinodes.Add(pos);
                antinodes.Add(other);
                while (grid.ContainsKey(antinode))
                {
                    antinodes.Add(antinode);
                    antinode = (X: antinode.X + diff.X, Y: antinode.Y + diff.Y);
                }
            }
        }
        
        // PrintGrid(grid, antinodes);
        return antinodes.Count;
    }

    private void PrintGrid(Grid grid, HashSet<Pos> antinodes)
    {
        var output = "\n";
        for (var y = 0; y <= grid.Keys.Max(g => g.Y); y++)
        {
            var line = "";
            for (var x = 0; x <= grid.Keys.Max(g => g.X); x++)
            {
                line += antinodes.Contains((x, y)) ? '#' : grid[(x, y)];
            }
            output += line + "\n";
        }
        
        WriteOutput(output);
    }
}