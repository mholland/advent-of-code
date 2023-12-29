using System.Collections.Immutable;
using Coord = (int X, int Y);
using Grid = System.Collections.Generic.Dictionary<(int X, int Y), char>;

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
    public void ExampleOne() => FindLongestPath(_example, true).Should().Be(94);

    [Fact]
    public void PartOne() => WriteOutput(FindLongestPath(Input, true));

    [Fact]
    public void ExampleTwo() => FindLongestPath(_example, false).Should().Be(154);

    [Fact]
    public void PartTwo() => WriteOutput(FindLongestPath(Input, false));

    private static readonly Dictionary<char, Coord> slopeTiles = new()
    {
        ['^'] = (0, -1),
        ['>'] = (1, 0),
        ['v'] = (0, 1),
        ['<'] = (-1, 0)
    };

    private static int FindLongestPath(string[] input, bool slopes)
    {
        var (grid, start, end) = ParseGrid(input);
        var junctions = grid
            .Where(x => x.Value == '.')
            .Where(x => Neighbours(grid, x.Key, slopes).Count() != 2)
            .Select(x => x.Key)
            .ToArray();

        var adjacency = BuildAdjacencyGraph(grid, junctions, slopes);

        return DFS(adjacency, start, end);
    }

    public static int DFS(Dictionary<Coord, List<(Coord Pos, int Dist)>> adjacency, Coord start, Coord end)
    {
        var pathQueue = new Stack<(Coord Pos, int Dist, ImmutableHashSet<Coord> Visited)>([(start, 0, ImmutableHashSet<Coord>.Empty)]);
        var max = 0;
        var dists = new Dictionary<Coord, int>();
        while (pathQueue.TryPop(out var current))
        {
            var (pos, dist, visited) = current;
            if (pos == end)
            {
                max = Math.Max(max, dist);
                continue;
            }
            visited = visited.Add(pos);
            foreach (var (nPos, nDist) in adjacency[pos])
            {
                if (!visited.Contains(nPos))
                    pathQueue.Push((nPos, dist + nDist, visited));
            }
        }

        return max;
    }

    static IEnumerable<Coord> Neighbours(Grid grid, Coord pos, bool slopes)
    {
        if (slopes && slopeTiles.TryGetValue(grid[pos], out var d))
        {
            yield return pos.Add(d);
            yield break;
        }
        Coord[] dirs = [(1, 0), (-1, 0), (0, 1), (0, -1)];
        foreach (var dir in dirs)
        {
            if (grid.TryGetValue(pos.Add(dir), out var tile) && tile != '#')
                yield return pos.Add(dir);
        }
    }

    public static Dictionary<Coord, List<(Coord Pos, int Dist)>> BuildAdjacencyGraph(Grid grid, Coord[] junctions, bool slopes)
    {
        var adjacency = junctions
            .Select(j => (j, new List<(Coord Pos, int Dist)>()))
            .ToDictionary();

        foreach (var junction in junctions)
        {
            var queue = new Queue<(Coord Pos, int Dist)>([(junction, 0)]);
            var visited = new HashSet<Coord>();
            while (queue.TryDequeue(out var current))
            {
                var (pos, dist) = current;
                if (junctions.Contains(pos) && pos != junction)
                {
                    adjacency[junction].Add((pos, dist));
                    continue;
                }
                visited.Add(pos);
                foreach (var nbor in Neighbours(grid, pos, slopes))
                {
                    if (!visited.Contains(nbor))
                        queue.Enqueue((nbor, dist + 1));
                }
            }
        }

        return adjacency;
    }

    private static (Grid Grid, Coord Start, Coord End) ParseGrid(string[] input)
    {
        var grid = new Grid();
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
                grid[(x, y)] = tile;
            }

        return (grid, start, end);
    }

    private string PrintGrid(Grid grid, ImmutableHashSet<Coord> seen)
    {
        var maxX = grid.Keys.Max(g => g.X);
        var maxY = grid.Keys.Max(g => g.Y);
        var res = string.Empty;
        for (var y = 0; y <= maxY; y++)
        {
            var line = string.Empty;
            for (var x = 0; x <= maxX; x++)
            {
                line += seen.Contains((x, y)) && !new char[] { '^', '>', '<', 'v' }.Contains(grid[(x, y)]) ? 'o' : grid[(x, y)];
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