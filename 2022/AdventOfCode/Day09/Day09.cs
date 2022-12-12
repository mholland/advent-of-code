namespace AdventOfCode.Day09;

public sealed class Day09 : TestBase
{
    public Day09(ITestOutputHelper output) 
        : base(output)
    {
    }

    protected override string Day => "Day09";

    private string[] Example = new[]
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

    [Fact]
    public void ExampleOne() => CountPositionsVisited(Example).Should().Be(13);

    [Fact]
    public void PartOne() => Output.WriteLine($"Visited: {CountPositionsVisited(Input)}");

    private int CountPositionsVisited(string[] input)
    {
        var visited = new HashSet<(int X, int Y)>();
        var knots = new LinkedList<(int X, int Y)>();
        knots.AddLast((X: 0, Y: 0));
        knots.AddLast((X: 0, Y: 0));
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
                var prev = (X: head.Value.X, Y: head.Value.Y);
                head.Value = (X: head.Value.X + delta.X, Y: head.Value.Y + delta.Y);
                var cur = head.Next;
                while (cur is not null)
                {
                    var ahead = cur.Previous!.Value;
                    if (Math.Abs(ahead.X - cur.Value.X) > 1 || Math.Abs(ahead.Y - cur.Value.Y) > 1)
                    {
                        cur.Value = (X: prev.X, Y: prev.Y);
                        if (cur.Next is null)
                            visited.Add(cur.Value);
                    }
                    prev = cur.Value;
                    cur = cur.Next;
                }
            }
        }

        return visited.Count();
    }

    private IEnumerable<(string Direction, int Distance)> ParseMovements(string[] input) =>
        input.Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (x[0], int.Parse(x[1])));
}