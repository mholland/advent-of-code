namespace AdventOfCode.Day14;

public sealed class Day14 : TestBase
{
    protected override string Day => "Day14";

    public Day14(ITestOutputHelper output)
        : base(output)
    {
    }

    private string[] Example = new[]
    {
        "498,4 -> 498,6 -> 496,6",
        "503,4 -> 502,4 -> 502,9 -> 494,9"
    };

    [Fact]
    public void ExampleOne() => CalculateCollectedSand(Example).Should().Be(24);

    [Fact]
    public void PartOne() => Output.WriteLine($"Sand: {CalculateCollectedSand(Input)}");

    private int CalculateCollectedSand(string[] input)
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
        var abyss = filled.Max(x => x.Y) + 1;

        var dest = new[] { (X: 0, Y: 1), (X: -1, Y: 1), (X: 1, Y: 1) };
        var abyssReached = false;
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
                    if (!filled.Contains(poss))
                    {
                        cur = poss;
                        moved = true;
                        break;
                    }
                }
                if (!moved)
                {
                    filled.Add(cur);
                    sand++;
                    break;
                }
                if (cur.Y >= abyss)
                {
                    abyssReached = true;
                    break;
                }
            }
            if (abyssReached)
                break;
        }

        return sand;
    }
}
