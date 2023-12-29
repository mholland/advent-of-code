using System.Collections.Immutable;
using Coord = (int X, int Y);

namespace AdventOfCode.Day23;

public sealed class Day23(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day23";

    private readonly string[] _example =
    [
        "#.#####################",
        "#.......#########...###",
        "#######.#########.#.###",
        "###.....#.>.>.###.#.###",
        "###v#####.#v#.###.#.###",
        "###.>...#.#.#.....#...#",
        "###v###.#.#.#########.#",
        "###...#.#.#.......#...#",
        "#####.#.#.#######.#.###",
        "#.....#.#.#.......#...#",
        "#.#####.#.#.#########v#",
        "#.#...#...#...###...>.#",
        "#.#.#v#######v###.###v#",
        "#...#.>.#...>.>.#.###.#",
        "#####v#.#.###v#.#.###.#",
        "#.....#...#...#.#.#...#",
        "#.#########.###.#.#.###",
        "#...###...#...#...#.###",
        "###.###.#.###v#####v###",
        "#...#...#.#.>.>.#.>.###",
        "#.###.###.#.###.#.#v###",
        "#.....###...###...#...#",
        "#####################.#"
    ];

    [Fact]
    public void ExampleOne() => FindLongestPath(_example, false).Should().Be(94);

    [Fact]
    public void PartOne() => WriteOutput(FindLongestPath(Input, false));

    [Fact]
    public void ExampleTwo() => FindLongestPath(_example, true).Should().Be(154);

    [Fact]
    public void PartTwo() => WriteOutput(FindLongestPath(Input, true));

    private static int FindLongestPath(string[] input, bool ignoreSlopes)
    {
        var grid = new Dictionary<Coord, char>();
        var start = default(Coord);
        var end = default(Coord);
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
            {
                var tile = input[y][x];
                if (y == 0 && tile == '.')
                    start = (x, y);
                if (y == input.Length - 1 && tile == '.')
                    end = (x, y);
                grid[(x, y)] = ignoreSlopes ? tile != '#' ? '.' : '#' : tile;
            }

        var queue = new Queue<(Coord Pos, int Dist, ImmutableHashSet<Coord> Seen)>([(start, 0, ImmutableHashSet<Coord>.Empty)]);
        var dists = new List<int>();
        Coord[] dirs = [(0, -1), (1, 0), (0, 1), (-1, 0)];
        var slopes = new Dictionary<char, Coord>
        {
            ['^'] = (0, -1),
            ['>'] = (1, 0),
            ['v'] = (0, 1),
            ['<'] = (-1, 0)
        };
        var greatestDistances = new Dictionary<Coord, int>();
        while (queue.TryDequeue(out var current))
        {
            var (position, distance, seen) = current;
            if (greatestDistances.TryGetValue(position, out var d) && d > distance)
                continue;
            greatestDistances[position] = distance;
            var currentTile = grid[position];
            if (position == end)
            {
                dists.Add(current.Dist);
                continue;
            }
            seen = seen.Add(position);

            if (slopes.TryGetValue(currentTile, out var nextDir))
            {
                var nextPos = position.Add(nextDir);
                if (!seen.Contains(nextPos) && grid[nextPos] != '#')
                    queue.Enqueue((nextPos, distance + 1, seen));
                continue;
            }

            foreach (var dir in dirs)
            {
                var n = position.Add(dir);

                if (seen.Contains(n) || !grid.TryGetValue(n, out var nxt) || nxt == '#')
                    continue;
                queue.Enqueue((n, distance + 1, seen));
            }
        }

        return dists.Max();
    }

    private string PrintGrid(Dictionary<Coord, char> grid, ImmutableHashSet<Coord> seen)
    {
        var maxX = grid.Keys.Max(g => g.X);
        var maxY = grid.Keys.Max(g => g.Y);
        var res = string.Empty;
        for (var y = 0; y <= maxY; y++)
        {
            var line = string.Empty;
            for (var x = 0; x <= maxX; x++)
            {
                line += seen.Contains((x, y)) && !new char[] { '^', '>', '<', 'v'}.Contains(grid[(x, y)]) ? 'o' : grid[(x, y)];
            }
            res += line + Environment.NewLine;
        }
        Output.WriteLine(res);

        return res;
    }
}

public static class CoordExtensions
{
    public static Coord Add(this Coord self, Coord other) =>
        (self.X + other.X, self.Y + other.Y);
}