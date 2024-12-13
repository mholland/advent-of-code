namespace AdventOfCode.Day12;

public sealed class Day12(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 12);

    private static readonly string[] _exampleOne =
    [
        "AAAA",
        "BBCD",
        "BBCC",
        "EEEC"
    ];

    private static readonly string[] _exampleTwo =
    [
        "OOOOO",
        "OXOXO",
        "OOOOO",
        "OXOXO",
        "OOOOO"
    ];

    private static readonly string[] _exampleThree =
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

    public static TheoryData<string[], int> Examples => new()
    {
        { _exampleOne, 140 },
        { _exampleTwo, 772 },
        { _exampleThree, 1930 }
    };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ExamplePrices(string[] input, int price) => CalculateFencingPrice(input).Should().Be(price);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculateFencingPrice(await ReadInputLines()));

    private static int CalculateFencingPrice(string[] input)
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
            var start = pos;
            var queue = new Queue<Pos>([start]);
            var seen = new HashSet<Pos>();
            var edges = 0;
            while (queue.TryDequeue(out var current))
            {
                if (!seen.Add(current))
                    continue;
                foreach (var neighbour in current.Neighbours())
                {
                    if (!grid.TryGetValue(neighbour, out var n) || n != plant)
                    {
                        edges += 1;
                        continue;
                    }
                    queue.Enqueue(neighbour);
                }
            }

            price += edges * seen.Count;
            globalSeen.UnionWith(seen);
        }

        return price;
    }

    private record Pos(int X, int Y)
    {
        public IEnumerable<Pos> Neighbours()
        {
            yield return new Pos(X + 1, Y);
            yield return new Pos(X, Y + 1);
            yield return new Pos(X - 1, Y);
            yield return new Pos(X, Y - 1);
        }
    }
}
