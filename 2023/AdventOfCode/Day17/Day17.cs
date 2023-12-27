using Coord = (int X, int Y);

namespace AdventOfCode.Day17;

public sealed class Day17(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day17";

    private readonly string[] _example =
    [
        "2413432311323",
        "3215453535623",
        "3255245654254",
        "3446585845452",
        "4546657867536",
        "1438598798454",
        "4457876987766",
        "3637877979653",
        "4654967986887",
        "4564679986453",
        "1224686865563",
        "2546548887735",
        "4322674655533"
    ];

    [Fact]
    public void ExampleOne() => FindLeastHeatLoss(_example, 1, 3).Should().Be(102);

    [Fact]
    public void PartOne() => WriteOutput(FindLeastHeatLoss(Input, 1, 3));

    [Fact]
    public void ExampleTwo() => FindLeastHeatLoss(_example, 4, 10).Should().Be(94);

    [Fact]
    public void PartTwo() => WriteOutput(FindLeastHeatLoss(Input, 4, 10));

    private static int FindLeastHeatLoss(string[] input, int minStep, int maxStep)
    {
        var grid = new Dictionary<Coord, int>();
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
            {
                grid[(x, y)] = input[y][x] - '0';
            }

        var startRight = new Context((0, 0), ((1, 0), 0));
        var startDown = new Context((0, 0), ((0, 1), 0));
        var end = (input.Max(i => i.Length) - 1, input.Length - 1);
        var dists = new Dictionary<Context, int>()
        {
            [startRight] = 0,
            [startDown] = 0
        };

        var queue = new PriorityQueue<Context, int>([(startRight, 0), (startDown, 0)]);
        while (queue.TryDequeue(out var context, out _))
        {
            if (context.Position == end && context.Direction.Count >= minStep)
                return dists[context];

            var nextPositions = GetNextPositions(context, minStep, maxStep).Where(c => grid.ContainsKey(c.Position));
            foreach (var next in nextPositions)
            {
                var n = grid[next.Position];
                if (!dists.TryGetValue(next, out var nextDist) ||
                    dists[context] + n < nextDist)
                {
                    dists[next] = dists[context] + n;
                    queue.Enqueue(new Context(next.Position, next.Direction), dists[next]);
                }
            }
        }

        return -1;

        static IEnumerable<Context> GetNextPositions(Context context, int minStep, int maxStep)
        {
            Coord[] dirs = [(0, -1), (1, 0), (0, 1), (-1, 0)];
            int idx = Array.IndexOf(dirs, context.Direction.Dir);

            if (context.Direction.Count >= minStep)
            {
                var left = dirs[(idx + 1) % 4];
                yield return new Context(context.Position.Add(left), (left, 1));

                var right = dirs[(idx + 3) % 4];
                yield return new Context(context.Position.Add(right), (right, 1));
            }

            if (context.Direction.Count < maxStep)
                yield return new Context(context.Position.Add(context.Direction.Dir), (context.Direction.Dir, context.Direction.Count + 1));
        }
    }

    private record Context(Coord Position, (Coord Dir, int Count) Direction);
}

public static class CoordExtensions
{
    public static Coord Add(this Coord self, Coord other) =>
        (self.X + other.X, self.Y + other.Y);
}
