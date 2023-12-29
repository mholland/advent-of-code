using System.Runtime.InteropServices.Marshalling;
using Coord = (int X, int Y);

namespace AdventOfCode.Day21;

public sealed class Day21(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day21";

    private readonly string[] _example =
    [
        "...........",
        ".....###.#.",
        ".###.##..#.",
        "..#.#...#..",
        "....#.#....",
        ".##..S####.",
        ".##..#...#.",
        ".......##..",
        ".##.#.####.",
        ".##..##.##.",
        "..........."
    ];

    [Fact]
    public void ExampleOne() => CountReachableLocations(_example, [6]).First().Should().Be(16);
    
    [Fact]
    public void PartOne() => WriteOutput(CountReachableLocations(Input, [64]).First());

    [Fact]
    public void PartTwo() => WriteOutput(CountReachableLocationsFurther(Input));

    private static long CountReachableLocationsFurther(string[] input)
    {
        const int steps = 26501365;
        // 26501365 - 65 =
        //      26501300
        // / 131 (width/heigh of grid)
        //        202300
        var height = input.Length;
        var yValues = new List<int>(3);
        yValues.AddRange(CountReachableLocations(
            input,
            Enumerable.Range(0, 3).Select(i => height / 2 + i * height)
            .ToArray()));

        // y = ax^2 + bx + c
        // yValues_0 = a0^2 + b0 + c = c
        // yValues_1 = a1^2 + b1 + c = a + b + c
        // yValues_2 = a2^2 + b2 + c = 4a + 2b + c
        long c = yValues[0];
        // yV_1 - c = a + b
        // yV_2 - c = 4a + 2b
        // 2(yV_1 - c) = 2a + 2b
        // (yV_2 - c) - 2(yV_1 - c) == 4a + 2b - 2a - 2b = 2a
        // a = ((yV_2 - c) - 2(yV_1 - c)) / 2
        long a = (yValues[2] - c - 2 * (yValues[1] - c)) / 2;
        long b = yValues[1] - a - c;

        var x = (steps - (height / 2)) / height;
        
        return a * x * x + b * x + c;
    }

    private static IEnumerable<int> CountReachableLocations(string[] input, int[] steps)
    {
        var garden = new Dictionary<Coord, char>();
        Coord start = default;
        var height = input.Length;
        var width = input.Max(i => i.Length);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < input[y].Length; x++)
            {
                var tile = input[y][x];
                if (tile == 'S')
                {
                    start = (x, y);
                    tile = '.';
                }

                garden.Add((x, y), tile);
            }
        }
        Coord[] dirs = [(0, -1), (1, 0), (0, 1), (-1, 0)];

        var locations = new List<Coord>([start]);
        var newLocations = new List<Coord>();
        var reached = new List<int>(steps.Length);
        for (var i = 0; i < steps[^1]; i++)
        {
            newLocations.Clear();
            foreach (var (cx, cy) in locations)
            foreach (var (px, py) in dirs)
            {
                var next = (cx + px, cy + py);
                if (GetLocation(garden, width, height, next) == '.')
                    newLocations.Add(next);
            }
            locations = [..newLocations.Distinct()];
            if (steps.Contains(i + 1))
                reached.Add(locations.Count);
        }

        return reached;

        static char GetLocation(Dictionary<Coord, char> garden, int width, int height, Coord loc) =>
            garden[(((loc.X % width) + width) % width, ((loc.Y % height) + height) % height)];

    }
}

