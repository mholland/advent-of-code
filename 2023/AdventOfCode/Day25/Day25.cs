namespace AdventOfCode.Day25;

public sealed class Day25(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day25";

    private readonly string[] _example =
    [
        "jqt: rhn xhk nvd",
        "rsh: frs pzl lsr",
        "xhk: hfx",
        "cmg: qnr nvd lhk bvb",
        "rhn: xhk bvb hfx",
        "bvb: xhk hfx",
        "pzl: lsr hfx nvd",
        "qnr: nvd",
        "ntq: jqt hfx bvb xhk",
        "nvd: lhk",
        "lsr: lhk",
        "rzs: qnr cmg lsr rsh",
        "frs: qnr lhk lsr"
    ];

    [Fact]
    public void ExampleOne() => CalculatePartitions(_example).Should().Be(54);

    [Fact]
    public void PartOne() => WriteOutput(CalculatePartitions(Input));

    private static int CalculatePartitions(string[] input)
    {
        var nodes = new Dictionary<string, HashSet<string>>();
        foreach (var line in input)
        {
            var parts = line.Split(':', StringSplitOptions.TrimEntries);
            var src = parts[0];
            var dests = parts[1].Split(' ');

            if (!nodes.TryGetValue(src, out _))
                nodes[src] = [];
            foreach (var dest in dests)
            {
                nodes[src].Add(dest);
                if (!nodes.TryGetValue(dest, out _))
                    nodes[dest] = [];
                nodes[dest].Add(src);
            }
        }

        var start = nodes.Keys.OrderBy(_ => Guid.NewGuid()).First();
        string? target = null;
        for (var x = 0; x < 3; x++)
        {
            var path = BFS(nodes, start, target);

            target = path[^1];
            for (var i = 0; i < path.Count - 1; i++)
                nodes[path[i]].Remove(path[i + 1]);
        }

        var visited = BFS(nodes, start, target);

        return (nodes.Count - visited.Count) * visited.Count;
    }

    private static List<string> BFS(Dictionary<string, HashSet<string>> nodes, string start, string? target = null)
    {
        var prev = new Dictionary<string, string?>
        {
            [start] = null
        };
        var tgt = start;
        var longestPath = 0;
        var visited = new HashSet<string>();
        var queue = new Queue<string>([start]);
        while (queue.TryDequeue(out var current))
        {
            visited.Add(current);
            if (current == target)
                break;
            foreach (var dest in nodes[current])
            {
                if (!prev.ContainsKey(dest))
                {
                    prev[dest] = current;
                    queue.Enqueue(dest);
                }
            }
        }

        if (target is not null && !prev.TryGetValue(target, out _))
            return [..visited];

        foreach (var node in prev.Keys)
        {
            var p = ConstructPath(node, prev);
            if (p.Count > longestPath)
            {
                tgt = node;
                longestPath = p.Count;
            }
        }

        return ConstructPath(tgt, prev);

        static List<string> ConstructPath(string target, Dictionary<string, string?> previous)
        {
            var tgt = target;
            var path = new List<string>();
            while (tgt != null)
            {
                path.Add(tgt);
                tgt = previous[tgt];
            }
            path.Reverse();

            return path;
        }
    }
}
