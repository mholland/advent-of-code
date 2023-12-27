using System.Text;
using Lens = (string Label, int FocalLength);

namespace AdventOfCode.Day15;

public sealed class Day15(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day15";

    private readonly string[] _example =
    [
        "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7"
    ];

    [Fact]
    public void ExampleOne() => CalculateHASH(_example).Should().Be(1320);

    [Fact]
    public void ExampleTwo() => CalculateHASHMAP(_example).Should().Be(145);

    [Fact]
    public void PartOne() => WriteOutput(CalculateHASH(Input));

    [Fact]
    public void PartTwo() => WriteOutput(CalculateHASHMAP(Input));

    private static int CalculateHASHMAP(string[] input)
    {
        var boxes = Enumerable.Range(0, 256)
            .Select(i => new Box(i + 1))
            .ToArray();
        var steps = input.SelectMany(i => i.Split(","));
        foreach (var step in steps)
        {
            var label = string.Empty;
            if (step.Contains('-'))
            {
                label = step[0..^1];
                var hash = Hash(label);
                boxes[hash].RemoveLens(label);
                continue;
            }

            var split = step.Split('=');
            label = split[0];
            var focalLength = int.Parse(split[1]);
            var h = Hash(label);
            boxes[h].AddLens(label, focalLength);
        }

        return boxes.Select(b => b.FocusingPower()).Sum();
    }

    private static int CalculateHASH(string[] input) =>
        input
            .SelectMany(i => i.Split(','))
            .Select(Hash)
            .Sum();

    private static int Hash(string step)
    {
        var current = 0;
        foreach (var character in Encoding.ASCII.GetBytes(step))
        {
            current = (current + character) * 17 % 256;
        }
        return current;
    }

    private sealed class Box(int number)
    {
        public int Number { get; } = number;
        public LinkedList<Lens> Lenses { get; } = [];

        public void AddLens(string label, int focalLength)
        {
            var lens = Lenses.FindFirst(l => l.Label == label);
            if (lens is not null)
            {
                lens.Value = (label, focalLength);
                return;
            }
            Lenses.AddLast(new LinkedListNode<Lens>((label, focalLength)));
        }

        public int FocusingPower() =>
            Lenses.Select((l, i) => Number * (i + 1) * l.FocalLength).Sum();

        public void RemoveLens(string label)
        {
            var lens = Lenses.FindFirst(l => l.Label == label);
            if (lens is not null)
                Lenses.Remove(lens);
        }
    }
}

public static class LinkedListExtensions
{
    public static LinkedListNode<T>? FindFirst<T>(this LinkedList<T> linkedList, Func<T, bool> predicate)
    {
        var cur = linkedList.First;
        while (cur is not null)
        {
            if (predicate.Invoke(cur.Value))
                return cur;
            cur = cur.Next;
        }

        return null;
    }
}