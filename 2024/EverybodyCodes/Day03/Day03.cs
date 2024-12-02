using Xunit.Abstractions;
using Coord = (int X, int Y);

namespace EverybodyCodes.Day03;

public class Day03(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => nameof(Day03);

    private readonly string[] _example =
    [
        "..........",
        "..###.##..",
        "...####...",
        "..######..",
        "..######..",
        "...####...",
        ".........."
    ];
    
    [Fact]
    public void ExampleOne() => RemovableBlocks(_example, _orthogonalNeighbours).Should().Be(35);

    [Fact]
    public void PartOne() => WriteOutput(RemovableBlocks(ReadFile("A.txt"), _orthogonalNeighbours));

    [Fact]
    public void PartTwo() => WriteOutput(RemovableBlocks(ReadFile("B.txt"), _orthogonalNeighbours));

    private readonly Coord[] _orthogonalNeighbours = [(1, 0), (-1, 0), (0, 1), (0, -1)];
    private readonly Coord[] _allNeighbours = [(0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0),  (-1, 1)];
    
    [Fact]
    public void ExampleThree() => RemovableBlocks(_example, _allNeighbours).Should().Be(29);
    
    [Fact]
    public void PartThree() => WriteOutput(RemovableBlocks(ReadFile("C.txt"), _allNeighbours));

    private static int RemovableBlocks(string[] input, Coord[] neighbourStrategy)
    {
        var land = new Dictionary<Coord, char>();
        var depths = new Dictionary<Coord, int>();
        for (var y = 0; y < input.Length; y++)
        {
            var line = input[y];
            for (var x = 0; x < line.Length; x++)
            {
                land.Add((x, y), line[x]);
                depths.Add((x, y), 0);
            }
        }

        var dug = 0;
        while (true)
        {
            var stillDigging = false;
            foreach (var (pos, _) in land.Where(b => b.Value == '#'))
            {
                var currentDepth = depths[pos];
                var neighbours = neighbourStrategy.Select(n => (X: n.X + pos.X, Y: n.Y + pos.Y));
                if (neighbours.Any(n => GetDepth(n) < currentDepth)) continue;
                
                depths[pos] += 1;
                dug += 1;
                stillDigging = true;
            }

            if (!stillDigging) break;
        }
        
        return dug;

        int GetDepth(Coord pos)
        {
            depths.TryGetValue(pos, out var depth);
            return depth;
        }
    }
}