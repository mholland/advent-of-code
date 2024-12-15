using Vec = (int X, int Y);

namespace AdventOfCode.Day14;

public sealed class Day14(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 14);

    [Fact]
    public void RobotWrapAround()
    {
        var robot = new Robot("4", "4", "2", "2");
        robot.Move(5, 5);
        robot.Pos.Should().Be((1, 1));

        var robotNeg = new Robot("0", "0", "-2", "-2");
        robotNeg.Move(5, 5);
        robotNeg.Pos.Should().Be((3, 3));
    }

    private readonly string[] _example =
    [
        "p=0,4 v=3,-3",
        "p=6,3 v=-1,-3",
        "p=10,3 v=-1,2",
        "p=2,0 v=2,-1",
        "p=0,0 v=1,3",
        "p=3,0 v=-2,-2",
        "p=7,6 v=-1,-3",
        "p=3,0 v=-1,-2",
        "p=9,3 v=2,3",
        "p=7,3 v=-1,2",
        "p=2,4 v=2,-3",
        "p=9,5 v=-3,-3"
    ];

    [Fact]
    public void ExampleOne() => SafetyFactor(_example, 11, 7).Should().Be(12);

    [Fact]
    public async Task PartOne() => WriteOutput(SafetyFactor(await ReadInputLines(), 101, 103));
    
    [Fact]
    public async Task PartTwo() => WriteOutput(EasterEgg(await ReadInputLines()));

    private int EasterEgg(string[] input)
    {
        var robots = ParseInput(input);
        for (var i = 1; i <= 10000; i++)
        {
            foreach (var robot in robots)
                robot.Move(101, 103);
            var positions = robots.Select(r => r.Pos).ToHashSet();
            var globalSeen = new HashSet<Vec>();
            var bestConnected = 0;
            foreach (var position in positions)
            {
                if (globalSeen.Contains(position))
                    continue;
                var seen = new HashSet<Vec>();
                var queue = new Queue<Vec>([position]);
                while (queue.TryDequeue(out var current))
                {
                    if (!seen.Add(current))
                        continue;
                    Vec[] dirs = [(1, 0), (0, 1), (-1, 0), (0, -1)];
                    foreach (var (dx, dy) in dirs)
                    {
                        var next = (current.X + dx, current.Y + dy);
                        if (positions.Contains(next))
                            queue.Enqueue(next);
                    }
                }
                bestConnected = Math.Max(bestConnected, seen.Count);
                globalSeen.UnionWith(seen);
            }
            if (bestConnected >= 20) 
            {
                PrintGrid(positions);
                return i;
            }
        }

        return 0;
    }

    private void PrintGrid(HashSet<Vec> positions)
    {
        var output = "\n";
        for (var y = 0; y < 103; y++)
        {
            var line = "";
            for (var x = 0; x < 101; x++)
            {
                line += positions.Contains((x, y)) ? '#' : '.';
            }
            output += line + "\n";
        }
        WriteOutput(output);
    }

    private static int SafetyFactor(string[] input, int w, int h)
    {
        var robots = ParseInput(input);
        for (var i = 0; i < 100; i++)
            foreach (var robot in robots)
                robot.Move(w, h);

        var topLeft = robots.Count(r => r.Pos.X >= 0 && r.Pos.X < (w - 1) / 2 && r.Pos.Y >= 0 && r.Pos.Y < (h - 1) / 2);
        var topRight = robots.Count(r => r.Pos.X >= (w + 1) / 2 && r.Pos.X < w && r.Pos.Y >= 0 && r.Pos.Y < (h - 1) / 2);
        var bottomLeft = robots.Count(r => r.Pos.X >= 0 && r.Pos.X < (w - 1) / 2 && r.Pos.Y >= (h + 1) / 2 && r.Pos.Y < h);
        var bottomRight = robots.Count(r => r.Pos.X >= (w + 1) / 2 && r.Pos.X < w && r.Pos.Y >= (h + 1) / 2 && r.Pos.Y < h);

        return topLeft * topRight * bottomLeft * bottomRight;
    }

    private static Robot[] ParseInput(string[] input)
    {
        return input
            .Select(x => x.Split(' '))
            .Select(x => (P: x[0][2..].Split(','), V: x[1][2..].Split(',')))
            .Select(x => new Robot(x.P[0], x.P[1], x.V[0], x.V[1]))
            .ToArray();
    }

    private record Robot
    {
        public Vec Pos { get; private set; }
        public Vec Vel { get; }

        public Robot(string px, string py, string vx, string vy)
        {
            Pos = (int.Parse(px), int.Parse(py));
            Vel = (int.Parse(vx), int.Parse(vy));
        }

        public void Move(int w, int h) =>
            Pos = ((((Pos.X + Vel.X) % w) + w) % w, (((Pos.Y + Vel.Y) % h) + h) % h);
    }
}
