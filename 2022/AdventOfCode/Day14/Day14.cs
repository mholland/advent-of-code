namespace AdventOfCode.Day14;

public sealed class Day14 : TestBase
{
    protected override string Day => "Day14";

    public Day14(ITestOutputHelper output)
        : base(output)
    {
    }

    private readonly string[] _example = 
    {
        "498,4 -> 498,6 -> 496,6",
        "503,4 -> 502,4 -> 502,9 -> 494,9"
    };

    [Fact]
    public void ExampleOne() => CalculateCollectedSand(_example, false).Should().Be(24);

    [Fact]
    public void PartOne() => Output.WriteLine($"Sand: {CalculateCollectedSand(Input, false)}");
    
    [Fact]
    public void ExampleTwo() => CalculateCollectedSand(_example, true).Should().Be(93);
    
    [Fact]
    public void PartTwo() => Output.WriteLine($"Sand: {CalculateCollectedSand(Input, true)}");

    private static HashSet<(int X, int Y)> ReadCave(string[] input)
    {
        var filled = new HashSet<(int X, int Y)>();
        foreach (var vein in input)
        {
            var coords = vein
                .Split(" -> ", StringSplitOptions.RemoveEmptyEntries)
                .Select(l => l.Split(','))
                .Select(l => (X: int.Parse(l[0]), Y: int.Parse(l[1])))
                .ToArray();
            var from = coords.First();
            filled.Add(from);
            foreach (var coord in coords.Skip(1))
            {
                var diff = (X: coord.X - from.X, Y: coord.Y - from.Y);
                var dir = (X: Math.Sign(diff.X), Y: Math.Sign(diff.Y));
                var dist = Math.Max(Math.Abs(diff.X), Math.Abs(diff.Y));
                for (var i = 0; i < dist; i++)
                {
                    var newFill = (from.X + dir.X, from.Y + dir.Y);
                    filled.Add(newFill);
                    from = newFill;
                }
            }
        }

        return filled;
    }

    private int CalculateCollectedSand(string[] input, bool hasFloor)
    {
        var cave = ReadCave(input);
        var maxDepth = cave.Max(x => x.Y) + 2;

        var dest = new[] { (X: 0, Y: 1), (X: -1, Y: 1), (X: 1, Y: 1) };
        var sand = 0;
        while (true)
        {
            var cur = (X: 500, Y: 0);
            while (true)
            {
                var moved = false;
                for (var i = 0; i < dest.Length; i++)
                {
                    var poss = (X: cur.X + dest[i].X, Y: cur.Y + dest[i].Y);
                    if (cave.Contains(poss) || poss.Y == maxDepth) continue;
                    cur = poss;
                    moved = true;
                    break;
                }
                if (!moved)
                {
                    cave.Add(cur);
                    sand++;
                    if (hasFloor && cave.Contains((500, 0)))
                        return sand;
                    break;
                }
                if (!hasFloor && cur.Y + 1 >= maxDepth)
                    return sand;
            }
        }
    }
}
