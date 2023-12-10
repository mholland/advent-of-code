using Coord = (int X, int Y);

using System.Text.RegularExpressions;

namespace AdventOfCode.Day03;

public sealed class Day03(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day03";

    private readonly string[] _example =
    [
        "467..114..",
        "...*......",
        "..35..633.",
        "......#...",
        "617*......",
        ".....+.58.",
        "..592.....",
        "......755.",
        "...$.*....",
        ".664.598.."
    ];

    [Fact]
    public void ExampleOne() => PartNumberSum(_example).Should().Be(4361);

    [Fact]
    public void SurroundingCoordinates()
    {
        var number = new Entry("35", (2, 2));
        number.Surrounds().Should().Contain([
            (1,2), (4, 2),
            (1, 1), (2,1), (3, 1), (4,1),
            (1, 3), (2,3), (3, 3), (4,3)
        ]);
    }

    [Fact]
    public void PartOne() =>
        WriteOutput("Parts sum: " + PartNumberSum(Input));

    [Fact]
    public void ExampleTwo() => GearRatioSum(_example).Should().Be(467835);

    [Fact]
    public void PartTwo() =>
        WriteOutput("Gear ratio sum: " + GearRatioSum(Input));

    private static (IEnumerable<Entry> Numbers, IEnumerable<Entry> Symbols) ParseInput(string[] input)
    {
        var numberPattern = new Regex(@"[\d]+", RegexOptions.Compiled);
        var symbolPattern = new Regex(@"[^\d\.]", RegexOptions.Compiled);

        var numbers = new List<Entry>();
        var symbols = new List<Entry>();

        for (var y = 0; y < input.Length; y++)
        {
            var line = input[y];
            var numberMatches = numberPattern.Matches(line);
            var symbolMatches = symbolPattern.Matches(line);

            symbols.AddRange(symbolMatches.Select(m => new Entry(m.Value, (X: m.Index, Y: y))));
            numbers.AddRange(numberMatches.Select(m => new Entry(m.Value, (X: m.Index, Y: y))));
        }

        return (numbers, symbols);
    }

    private static int GearRatioSum(string[] input)
    {
        var (numbers, symbols) = ParseInput(input);
        var sum = 0;

        var gears = symbols.Where(s => s.Value == "*");
        foreach (var gear in gears)
        {
            var adjacentNumbers = numbers
                .Where(n => n.Surrounds().Contains(gear.Location))
                .ToArray();
            if (adjacentNumbers.Length == 2)
                sum += adjacentNumbers.Aggregate(1, (acc, cur) => acc *= cur.ToNumber());
        }

        return sum;
    }

    private static int PartNumberSum(string[] input)
    {
        var (numbers, symbols) = ParseInput(input);
        var symbolSet = symbols.Select(s => s.Location).ToHashSet();

        var sum = 0;
        foreach (var number in numbers)
        {
            if (number.Surrounds().Any(symbolSet.Contains))
                sum += number.ToNumber();
        }

        return sum;
    }

    private record Entry(string Value, Coord Location)
    {
        public IEnumerable<Coord> Surrounds()
        {
            var (x, y) = Location;
            var width = Value.Length;
            yield return (x - 1, y);
            yield return (x + width, y);
            for (var xx = x - 1; xx <= x + width; xx++)
            {
                yield return (xx, y - 1);
                yield return (xx, y + 1);
            }
        }

        public int ToNumber() => int.Parse(Value);
    }
}
