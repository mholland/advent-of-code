using System.Globalization;
using Vec = (long X, long Y);

namespace AdventOfCode.Day18;

public sealed class Day18(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day18";

    private readonly string[] _example =
    [
        "R 6 (#70c710)",
        "D 5 (#0dc571)",
        "L 2 (#5713f0)",
        "D 2 (#d2c081)",
        "R 2 (#59c680)",
        "D 2 (#411b91)",
        "L 5 (#8ceee2)",
        "U 2 (#caa173)",
        "L 1 (#1b58a2)",
        "U 2 (#caa171)",
        "R 2 (#7807d2)",
        "U 3 (#a77fa3)",
        "L 2 (#015232)",
        "U 2 (#7a21e3)"
    ];

    [Fact]
    public void ExampleOne() => CalculateLavaFill(_example).Should().Be(62);

    [Fact]
    public void ExampleTwo() => CalculateLavaFillHex(_example).Should().Be(952408144115);

    [Fact]
    public void PartOne() => WriteOutput(CalculateLavaFill(Input).ToString());

    [Fact]
    public void PartTwo() => WriteOutput(CalculateLavaFillHex(Input).ToString());

    private static long CalculateLavaFillHex(string[] input)
    {
        var moves = input.Select(i => i.Split(' '))
            .Select(i => i[2])
            .Select(i => i[2..^1])
            .Select(ParseHex)
            .ToArray();

        return CalculateArea(moves);

        static (Vec Dir, int Dist) ParseHex(string val)
        {
            var dirs = new Dictionary<char, Vec>
            {
                //0 means R, 1 means D, 2 means L, and 3 means U.
                ['3'] = (0, -1),
                ['0'] = (1, 0),
                ['1'] = (0, 1),
                ['2'] = (-1, 0),
            };
            var dist = int.Parse(val[0..^1], NumberStyles.HexNumber);
            return (dirs[val[^1]], dist);
        }
    }

    private static long CalculateArea((Vec Dir, int Dist)[] moves)
    {
        var current = (0L, 0L);
        var ll = new LinkedList<Vec>();
        ll.AddLast(current);
        var perimeter = moves.Sum(m => m.Dist);
        foreach (var (dir, dist) in moves[0..^1])
        {
            var next = current.Add(dir.Mult(dist));
            ll.AddLast(next);
            current = next;
        }

        var first = ll.First;
        var cur = first;
        long sumOne = default;
        long sumTwo = default;
        while (cur != null && cur.Prev() != null && cur.Prev() != first)
        {
            sumOne += cur.Value.X * cur.Prev()!.Value.Y;
            sumTwo += cur.Value.Y * cur.Prev()!.Value.X;
            cur = cur.Prev();
        };

        var area = Math.Abs(sumOne - sumTwo) / 2;
        return area - (perimeter / 2) + 1 + perimeter;
    }

    private static long CalculateLavaFill(string[] input)
    {
        var dirs = new Dictionary<string, Vec>
        {
            ["U"] = (0, -1),
            ["R"] = (1, 0),
            ["D"] = (0, 1),
            ["L"] = (-1, 0),
        };
        var moves = input
            .Select(i => i.Split(' '))
            .Select(i => (Dir: dirs[i[0]], Dist: int.Parse(i[1])))
            .ToArray();

        return CalculateArea(moves);
    }
}

public static class VecExtensions
{
    public static Vec Add(this Vec self, Vec other) =>
        (self.X + other.X, self.Y + other.Y);

    public static Vec Mult(this Vec self, long multiplier) =>
        (self.X * multiplier, self.Y * multiplier);
}

public static class LinkedListNodeExtensions
{
    public static LinkedListNode<T>? Prev<T>(this LinkedListNode<T> node) =>
        node?.Previous ?? node!.List!.Last;
}
