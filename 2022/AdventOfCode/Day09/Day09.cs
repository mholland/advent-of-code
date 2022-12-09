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
        var tail = (X: 0, Y: 0);
        var head = (X: 0, Y: 0);
        visited.Add(tail);
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
            for (var a = 0; a < dist; a++)
            {
                var newHead = (X: head.X + delta.X, Y: head.Y + delta.Y);
                if (Math.Abs(newHead.X - tail.X) > 1 || Math.Abs(newHead.Y - tail.Y) > 1)
                {
                    tail = (X: head.X, Y: head.Y);
                    visited.Add(tail);
                }
                head = newHead;
            }
        }

        return visited.Count();
    }

    private IEnumerable<(string Direction, int Distance)> ParseMovements(string[] input) =>
        input.Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (x[0], int.Parse(x[1])));
}