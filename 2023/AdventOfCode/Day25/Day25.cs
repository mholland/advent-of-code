using Graph = System.Collections.Generic.Dictionary<string, AdventOfCode.Day25.Day25.Node>;
using Edges = System.Collections.Generic.List<(string From, string To)>;
using System.Diagnostics;

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

    private readonly string[] _exampleThree =
    [
        "A: B C D E",
        "B: C D E",
        "C: D E",
        "D: E",

        "E: J",
        "D: F",
        "C: J",

        "F: G H I J",
        "G: H I J",
        "H: I J",
        "I: J",
    ];

    [Fact]
    public void ExampleOne() => CalculateGroupSizes(_example).Should().Be(54);

    [Fact]
    public void ExampleThree() => CalculateGroupSizes(_exampleThree).Should().Be(25);

    [Fact]
    public void PartOne() => WriteOutput(CalculateGroupSizes(Input)); // 582590

    public int CalculateGroupSizes(string[] input)
    {
        var (graph, edges) = ParseInput(input);
        var cuts = -1;
        var i = 0;
        var n = Math.Pow(graph.Count, 2);
        
        while (cuts != 3 && i < n)
        {
            (graph, edges) = KargerCut(ParseInput(input));
            cuts = edges.Count;
            i++;
        }

        Output.WriteLine("Iterations: " + i);

        return graph.Aggregate(1, (agg, cur) => agg *= cur.Value.Names.Count);
    }

    private static (Graph Graph, Edges Edges) ParseInput(string[] input)
    {
        var graph = new Graph();
        var edges = new Edges();
        foreach (var line in input)
        {
            var split = line.Split(':');
            var from = split[0];
            var to = split[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (!graph.TryGetValue(from, out _))
                graph.Add(from, new Node(from));
            foreach (var t in to)
            {
                if (!graph.TryGetValue(t, out _))
                    graph.Add(t, new Node(t));
                edges.Add((from, t));
            }
        }

        return (graph, edges);
    }

    private static (Graph Graph, Edges Edges) KargerCut((Graph Graph, Edges Edges) input)
    {
        var (graph, edges) = input;
        var random = new Random();
        while (graph.Count > 2)
        {
            var (f, t) = edges[random.Next(0, edges.Count - 1)];
            var from = graph[f];
            var to = graph[t];
            from.Merge(to);
            for (var i = 0; i < edges.Count; i++)
            {
                var edge = edges[i];
                if (edge.From == t)
                    edge.From = f;
                if (edge.To == t)
                    edge.To = f;
                edges[i] = edge;
            }
            edges.RemoveAll(e => e.From == e.To);
            graph.Remove(t);
        }

        return (graph, edges);
    }

    internal record Node(string Name)
    {
        public List<string> Names { get; } = [Name];

        public void Merge(Node node) => Names.AddRange(node.Names);
    }
}
