using System.Collections.Immutable;

namespace AdventOfCode.Day13;

public sealed class Day13(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day13";

    private readonly string _example =

"""
#.##..##.
..#.##.#.
##......#
##......#
..#.##.#.
..##..##.
#.#.##.#.

#...##..#
#....#..#
..##..###
#####.##.
#####.##.
..##..###
#....#..#
""";

    [Fact]
    public void ExampleOne() => SummariseTerrain(_example).Summary.Should().Be(405);
    
    [Fact]
    public void ExampleTwo() => SummariseTerrain(_example).Fixed.Should().Be(400);

    private readonly string _exampleThree =
"""
...#..#
...#..#
##..##.
.#.##.#
..#..##
#.#.##.
#.#.###
##.##..
##.##..
#.#.###
..#.##.
..#..##
.#.##.#
##..##.
...#..#
""";

    [Fact]
    public void ExampleThree() => SummariseTerrain(_exampleThree).Fixed.Should().Be(800);

    [Fact]
    public void PartOne() => WriteOutput(SummariseTerrain(ReadAll("input.txt")).Summary);
    
    [Fact]
    public void PartTwo() => WriteOutput(SummariseTerrain(ReadAll("input.txt")).Fixed);

    private static (int Summary, int Fixed) SummariseTerrain(string input)
    {
        var result = input.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(g => Grid.Create(g.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)))
            .Select(g => (Summary: g.Summary(), Fixed: g.Fix()));

        return (Summary: result.Sum(r => r.Summary), Fixed: result.Sum(r => r.Fixed));
    }


    private sealed class Grid
    {
        private readonly string[] _rows;
        private readonly string[] _cols;

        private Grid(string[] rows, string[] cols) =>
            (_rows, _cols) = (rows, cols);

        public static Grid Create(string[] input)
        {
            var rows = input;
            var cols = new string[input[0].Length];
            for (var x = 0; x < input[0].Length; x++)
            {
                var col = string.Empty;
                for (var y = 0; y < input.Length; y++)
                {
                    col += input[y][x];
                }
                cols[x] = col;
            }

            return new Grid(rows, cols);
        }

        public int Summary() => Summary(_rows, 100).First() + Summary(_cols, 1).First();

        private static IEnumerable<int> Summary(string[] x, int modifier)
        {
            for (var r = 0; r < x.Length - 1; r++)
            {
                if (IsValidSymmetryLine(x, r))
                    yield return (r + 1) * modifier;
            }

            yield return 0;
        }

        
        public static int FindNewSummary(string[] toCheck, int originalSummary, int summaryModifier)
        {
            var candidates = new List<(int Position, int Index)>();
            for (var p0 = 0; p0 < toCheck.Length; p0++)
            {
                for (var p1 = p0 + 1; p1 < toCheck.Length; p1++)
                {
                    var (differs, index) = DiffersByOne(toCheck[p0], toCheck[p1]);
                    if (differs) candidates.Add((p0, index));
                }
            }

            foreach (var (pos, index) in candidates.Distinct())
            {
                var @fixed = ImmutableArray.CreateBuilder<string>(toCheck.Length);
                @fixed.AddRange(toCheck);

                var cells = @fixed[pos].ToCharArray();
                cells[index] = cells[index] == '.' ? '#' : '.';
                @fixed[pos] = string.Join("", cells);
                
                var fixedSummary = Summary(@fixed.ToArray(), summaryModifier)
                    .FirstOrDefault(f => f > 0 && f != originalSummary);
                if (fixedSummary > 0)
                    return fixedSummary;
            }

            return 0;
        }


        public int Fix()
        {
            var originalSummary = Summary();
            var newRowSummary = FindNewSummary(_rows, originalSummary, 100);
            return newRowSummary > 0 ? newRowSummary : FindNewSummary(_cols, originalSummary, 1);
        }

        private static (bool Differs, int Index) DiffersByOne(string a, string b)
        {
            var diff = 0;
            var idx = 0;
            for (var i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i])
                {
                    if (diff >= 1) return (false, -1);

                    diff++;
                    idx = i;
                }
            }
            return diff == 1 ? (true, idx) : (false, -1);
        }

        private static bool IsValidSymmetryLine(string[] toCheck, int line)
        {
            var offset = (L: line, R: line + 1);
            while (offset.L >= 0 && offset.R < toCheck.Length)
            {
                if (toCheck[offset.L] != toCheck[offset.R])
                    return false;
                offset = (offset.L - 1, offset.R + 1);
            }

            return true;
        }
    }
}
