namespace AdventOfCode.Day09;

public sealed class Day09 : TestBase
{
    public Day09(ITestOutputHelper output) 
        : base(output)
    {
    }

    protected override string Day => "Day09";

    private readonly string[] _example = 
    {
        "R 4",
        "U 4",
        "L 3",
        "D 1",
        "R 4",
        "D 1",
        "L 5",
        "R 2"
    };

    private readonly string[] _exTwo = 
    {
        "R 5",
        "U 8",
        "L 8",
        "D 3",
        "R 17",
        "D 10",
        "L 25",
        "U 20"
    };

    [Fact]
    public void ExampleOne() => CountPositionsVisited(_example, 2).Should().Be(13);

    [Fact]
    public void PartOne() => Output.WriteLine($"Visited: {CountPositionsVisited(Input, 2)}");

    [Fact]
    public void ExampleTwo() => CountPositionsVisited(_exTwo, 10).Should().Be(36);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Visited: {CountPositionsVisited(Input, 10)}");

    private int CountPositionsVisited(string[] input, int knotCount)
    {
        var visited = new HashSet<(int X, int Y, int Num)>();
        var knots = new LinkedList<(int X, int Y, int Num)>();
        for (var c = 0; c < knotCount; c++)
            knots.AddLast((X: 0, Y: 0, Num: c));
        visited.Add(knots.Last!.Value);
        var moves = ParseMovements(input);
        var deltas = new Dictionary<string, (int X, int Y)>
        {
            ["U"] = (0, -1),
            ["R"] = (1, 0),
            ["D"] = (0, 1),
            ["L"] = (-1, 0)
        };
        foreach (var (dir, dist) in moves)
        {
            var delta = deltas[dir];
            var head = knots.First!;
            for (var a = 0; a < dist; a++)
            {
                head.Value = (X: head.Value.X + delta.X, Y: head.Value.Y + delta.Y, Num: head.Value.Num);
                var cur = head.Next;
                while (cur is not null)
                {
                    var ahead = cur.Previous!.Value;
                    if (Math.Abs(ahead.X - cur.Value.X) > 1 || Math.Abs(ahead.Y - cur.Value.Y) > 1)
                    {
                        var c = cur.Value;
                        var sign = (X: Math.Sign(ahead.X - c.X), Y: Math.Sign(ahead.Y - c.Y));
                        cur.Value = (X: c.X + sign.X, Y: c.Y + sign.Y, cur.Value.Num);
                        if (cur.Next is null)
                            visited.Add(cur.Value);
                    }
                    cur = cur.Next;
                }
            }
        }

        return visited.Count;
    }

    private static IEnumerable<(string Direction, int Distance)> ParseMovements(string[] input) =>
        input.Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (x[0], int.Parse(x[1])));
}