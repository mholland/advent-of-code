using System.ComponentModel;
using Pos3 = (int X, int Y, int Z);

namespace AdventOfCode.Day08;

public sealed class Day08(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 08);

    private readonly string[] _example =
    [
        "162,817,812",
        "57,618,57",
        "906,360,560",
        "592,479,940",
        "352,342,300",
        "466,668,158",
        "542,29,236",
        "431,825,988",
        "739,650,466",
        "52,470,668",
        "216,146,977",
        "819,987,18",
        "117,168,530",
        "805,96,715",
        "346,949,466",
        "970,615,88",
        "941,993,340",
        "862,61,35",
        "984,92,344",
        "425,690,689"
    ];

    [Fact]
    public void ExampleOne() => FindLargestCircuits(_example, 10).Should().Be(40);

    [Fact]
    public async Task PartOne() => WriteOutput(FindLargestCircuits(await ReadInputLines(), 1000));

    [Fact]
    public void ExampleTwo() => FindLastBoxes(_example).Should().Be(25272);

    [Fact]
    public async Task PartTwo() => WriteOutput(FindLastBoxes(await ReadInputLines()));

    private static long FindLastBoxes(string[] input)
    {
        var boxes = input.Select(Pos3.Parse).ToArray();
        var distances = CalculateDistances(boxes).OrderBy(d => d.Value).ToArray();
        var i = 0;
        var edges = new Dictionary<Pos3, List<Pos3>>();
        while (true)
        {
            var ((from, to), _) = distances[i];
            edges.TryAdd(from, []);
            edges.TryAdd(to, []);
            edges[from].Add(to);
            edges[to].Add(from);

            var seen = TraverseCircuit(edges, boxes[0]);
            if (seen.Count == boxes.Length)
                return Convert.ToInt64(from.X) * to.X;

            i += 1;
        }
    }

    private static Dictionary<(Pos3 From, Pos3 To), double> CalculateDistances(Pos3[] boxes)
    {
        var distances = new Dictionary<(Pos3 From, Pos3 To), double>();

        for (var i = 0; i < boxes.Length; i++)
        {
            var b1 = boxes[i];
            for (var j = i + 1; j < boxes.Length; j++)
            {
                var b2 = boxes[j];
                var dist = b1.Dist(b2);
                distances.Add((b1, b2), dist);
            }
        }

        return distances;
    }

    private static int FindLargestCircuits(string[] input, int connections)
    {
        var boxes = input.Select(Pos3.Parse).ToArray();
        var totalBoxes = boxes.Length;
        var distances = CalculateDistances(boxes);

        var shortest = distances.OrderBy(e => e.Value).Take(connections).ToArray();
        var edges = new Dictionary<Pos3, List<Pos3>>();
        foreach (var ((from, to), _) in shortest)
        {
            edges.TryAdd(from, []);
            edges.TryAdd(to, []);
            edges[from].Add(to);
            edges[to].Add(from);
        }

        var circuitSizes = new List<int>();
        var overallSeen = new HashSet<Pos3>();
        foreach (var box in boxes)
        {
            if (overallSeen.Contains(box))
                continue;

            var seen = TraverseCircuit(edges, box);

            circuitSizes.Add(seen.Count);
            overallSeen.UnionWith(seen);
        }

        return circuitSizes.OrderByDescending(c => c).Take(3).Aggregate(1, (acc, cur) => acc * cur);
    }

    private static HashSet<Pos3> TraverseCircuit(Dictionary<Pos3, List<Pos3>> edges, Pos3 box)
    {
        var queue = new Queue<Pos3>([box]);
        var seen = new HashSet<Pos3>();
        while (queue.TryDequeue(out var current))
        {
            if (!seen.Add(current) || !edges.TryGetValue(current, out var connected))
                continue;

            foreach (var adj in connected)
                queue.Enqueue(adj);
        }

        return seen;
    }
}

public static class Pos3Extensions
{
    extension(Pos3 p)
    {
        public static Pos3 Parse(string input)
        {
            var split = input.Split(',');
            return (int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
        }

        public double Dist(Pos3 other) =>
            Math.Sqrt(
                Math.Pow(other.X - p.X, 2) +
                Math.Pow(other.Y - p.Y, 2) +
                Math.Pow(other.Z - p.Z, 2)
            );
    }
}
