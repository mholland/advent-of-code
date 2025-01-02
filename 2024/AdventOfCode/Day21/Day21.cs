namespace AdventOfCode.Day21;

public sealed class Day21(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 21);

    private readonly string[] _example =
    [
        "029A",
        "980A",
        "179A",
        "456A",
        "379A"
    ];

    private static readonly Grid DirectionPad = new()
    {
        [(1, 0)] = '^',
        [(2, 0)] = 'A',
        [(0, 1)] = '<',
        [(1, 1)] = 'v',
        [(2, 1)] = '>'
    };

    private static readonly Dictionary<char, Pos> DirectionPadInv = 
        DirectionPad.ToDictionary(x => x.Value, x => x.Key);

    // +---+---+---+
    // | 7 | 8 | 9 |
    // +---+---+---+
    // | 4 | 5 | 6 |
    // +---+---+---+
    // | 1 | 2 | 3 |
    // +---+---+---+
    //     | 0 | A |
    //     +---+---+
    private static readonly Grid NumberPad = new()
    {
        [(0, 0)] = '7',
        [(1, 0)] = '8',
        [(2, 0)] = '9',
        [(0, 1)] = '4',
        [(1, 1)] = '5',
        [(2, 1)] = '6',
        [(0, 2)] = '1',
        [(1, 2)] = '2',
        [(2, 2)] = '3',

        [(1, 3)] = '0',
        [(2, 3)] = 'A'
    };

    private static readonly Dictionary<char, Pos> NumberPadInv =
        NumberPad.ToDictionary(x => x.Value, x => x.Key);
    
    [Fact]
    public void ExampleOne() => ComplexitySum(_example, 2).Should().Be(126384);

    [Fact]
    public async Task PartOne() => WriteOutput(ComplexitySum(await ReadInputLines(), 2));

    [Fact]
    public async Task PartTwo() => WriteOutput(ComplexitySum(await ReadInputLines(), 25));

    private static long ComplexitySum(string[] input, int robots)
    {
        // We can, initially, press any button we want at a cost of 1 as we're pressing buttons not a robot.
        var costs = new Dictionary<(Pos From, Pos To), long>();
        foreach (var (from, to) in Pairs(DirectionPad))
            costs.Add((from, to), 1);

        var paths = GetPairPaths();

        costs = Enumerable.Range(0, robots).Aggregate(costs, (agg, cur) =>
            GetCosts(DirectionPad, agg, paths));

        costs = GetCosts(NumberPad, costs, paths);

        return input.Aggregate(0L, (agg, code) => agg + GetCost(code, costs, NumberPad) * int.Parse(code[..^1]));
    }

    private static Dictionary<(Grid Pad, Pos From, Pos To), List<string>> GetPairPaths()
    {
        Dictionary<(Grid Pad, Pos From, Pos To), List<string>> result = [];
        foreach (var (from, to) in Pairs(DirectionPad))
            result.Add((DirectionPad, from, to), FindPaths(DirectionPad, from, to));
        foreach (var (from, to) in Pairs(NumberPad))
            result.Add((NumberPad, from, to), FindPaths(NumberPad, from, to));
        return result;
    }

    private static Dictionary<(Pos From, Pos To), long> GetCosts(Grid pad, Dictionary<(Pos From, Pos To), long> previousCosts, Dictionary<(Grid Pad, Pos From, Pos To), List<string>> pairPaths)
    {
        var newCosts = new Dictionary<(Pos From, Pos To), long>();
        foreach (var (from, to) in Pairs(pad))
        {
            var minCost = pairPaths[(pad, from, to)].Min(p => GetCost(p, previousCosts, DirectionPad));
            newCosts.Add((from, to), minCost);
        }
        return newCosts;
    }

    private static IEnumerable<(Pos From, Pos To)> Pairs(Grid grid)
    {
        foreach (var from in grid.Keys)
        foreach (var to in grid.Keys)
            yield return (from, to);
    }

    private static long GetCost(string path, Dictionary<(Pos From, Pos To), long> costs, Grid pad)
    {
        var lookup = pad.ToDictionary(x => x.Value, x => x.Key);
        var from = lookup['A'];
        var cost = 0L;

        foreach (var move in path)
        {
            var to = lookup[move];
            cost += costs[(from, to)];
            from = to;
        }

        return cost;
    }

    private static List<string> FindPaths(Grid grid, Pos from, Pos to)
    {
        var paths = new List<string>();
        var queue = new Queue<(Pos Pos, string Path)>([(from, string.Empty)]);
        while (queue.TryDequeue(out var current))
        {
            var (pos, path) = current;
            if (pos == to)
            {
                paths.Add(path + 'A');
                continue;
            }

            var xDiff = Math.Sign(to.X - pos.X);
            var yDiff = Math.Sign(to.Y - pos.Y);
            var nextX = (pos.X + xDiff, pos.Y);
            var nextY = (pos.X, pos.Y + yDiff);
            if (xDiff != 0 && grid.ContainsKey(nextX))
            {
                var d = xDiff > 0 ? '>' : '<';
                queue.Enqueue((nextX, path + d));
            }
            if (yDiff != 0 && grid.ContainsKey(nextY))
            {
                var d = yDiff > 0 ? 'v' : '^';
                queue.Enqueue((nextY, path + d));
            }
        }

        return paths;
    }
}
