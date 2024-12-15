namespace AdventOfCode.Day12;

public sealed class Day12(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 12);

    private static readonly string[] ExampleOne =
    [
        "AAAA",
        "BBCD",
        "BBCC",
        "EEEC"
    ];

    private static readonly string[] ExampleTwo =
    [
        "OOOOO",
        "OXOXO",
        "OOOOO",
        "OXOXO",
        "OOOOO"
    ];

    private static readonly string[] ExampleThree =
    [
        "RRRRIICCFF",
        "RRRRIICCCF",
        "VVRRRCCFFF",
        "VVRCCCJFFF",
        "VVVVCJJCFE",
        "VVIVCCJJEE",
        "VVIIICJJEE",
        "MIIIIIJJEE",
        "MIIISIJEEE",
        "MMMISSJEEE"
    ];

    public static TheoryData<string[], int> ExamplesPartOne => new()
    {
        { ExampleOne, 140 },
        { ExampleTwo, 772 },
        { ExampleThree, 1930 }
    };

    [Theory]
    [MemberData(nameof(ExamplesPartOne))]
    public void ExampleOnePrices(string[] input, int price) => CalculateFencingPrice(input).Should().Be(price);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculateFencingPrice(await ReadInputLines()));

    private static readonly string[] ExampleFour =
    [
        "EEEEE",
        "EXXXX",
        "EEEEE",
        "EXXXX",
        "EEEEE"
    ];

    private static readonly string[] ExampleFive =
    [
        "AAAAAA",
        "AAABBA",
        "AAABBA",
        "ABBAAA",
        "ABBAAA",
        "AAAAAA"
    ];

    public static TheoryData<string[], int> ExamplesPartTwo() => new()
    {
        { ExampleOne, 80 },
        { ExampleTwo, 436 },
        { ExampleThree, 1206 },
        { ExampleFour, 236 },
        { ExampleFive, 368 }
    };

    [Theory]
    [MemberData(nameof(ExamplesPartTwo))]
    public void ExampleTwoPrices(string[] input, int price) => CalculateFencingPrice(input, true).Should().Be(price);

    [Fact]
    public async Task PartTwo() => WriteOutput(CalculateFencingPrice(await ReadInputLines(), true));

    private static int CalculateFencingPrice(string[] input, bool discounted = false)
    {
        Dictionary<Pos, char> grid = [];
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
            {
                grid[new Pos(x, y)] = input[y][x];
            }

        var globalSeen = new HashSet<Pos>();
        var price = 0;
        foreach (var (pos, plant) in grid)
        {
            if (globalSeen.Contains(pos))
                continue;
            var queue = new Queue<Pos>([pos]);
            var seen = new HashSet<Pos>();
            var edges = new HashSet<(Pos Pos, Pos Dir)>();
            while (queue.TryDequeue(out var current))
            {
                if (!seen.Add(current))
                    continue;
                foreach (var (neighbour, dir) in current.Neighbours())
                {
                    if (!grid.TryGetValue(neighbour, out var n) || n != plant)
                    {
                        if (dir == Pos.Right)
                            edges.Add((neighbour, Pos.Left));
                        if (dir == Pos.Left)
                            edges.Add((current, Pos.Right));
                        if (dir == Pos.Up)
                            edges.Add((current, Pos.Down));
                        if (dir == Pos.Down)
                            edges.Add((neighbour, Pos.Up));

                        continue;
                    }
                    queue.Enqueue(neighbour);
                }
            }
            price += (discounted ? CountSides(edges) : edges.Count) * seen.Count;
            globalSeen.UnionWith(seen);
        }

        return price;
    }

    private static int CountSides(HashSet<(Pos Pos, Pos Direction)> edges)
    {
        var sides = 0;
        // Get all edges that have the region above or below it
        var horizontal = edges
            .Where(e => e.Direction == Pos.Up || e.Direction == Pos.Down)
            .GroupBy(h => h.Pos.Y);
        foreach (var group in horizontal)
        {
            // For each of the horizontal lines, order them by X position and iterate through incrementing sides
            // when an edge segment doesn't come immediately after the previous one.
            var ordered = group.OrderBy(g => g.Pos.X).ToArray();
            var prev = ordered.First();
            foreach (var (p, d) in ordered.Skip(1))
            {
                if (Math.Abs(prev.Pos.X - p.X) != 1 || prev.Direction != d)
                    sides += 1;
                prev = (p, d);
            }
            sides += 1;
        }
        var vertical = edges
            .Where(e => e.Direction == Pos.Left || e.Direction == Pos.Right)
            .GroupBy(v => v.Pos.X);
        foreach (var group in vertical)
        {
            var ordered = group.OrderBy(g => g.Pos.Y).ToArray();
            var prev = ordered.First();
            foreach (var (p, d) in ordered.Skip(1))
            {
                if (Math.Abs(prev.Pos.Y - p.Y) != 1 || prev.Direction != d)
                    sides += 1;
                prev = (p, d);
            }
            sides += 1;
        }
        return sides;
    }

    private record Pos(int X, int Y)
    {
        public static readonly Pos Up = new(0, -1);
        public static readonly Pos Right = new(1, 0);
        public static readonly Pos Down = new(0, 1);
        public static readonly Pos Left = new(-1, 0);
        public IEnumerable<(Pos Pos, Pos Dir)> Neighbours()
        {
            yield return (this with { X = X + 1 }, Right);
            yield return (this with { Y = Y + 1 }, Down);
            yield return (this with { X = X - 1 }, Left);
            yield return (this with { Y = Y - 1 }, Up);
        }
    }
}
