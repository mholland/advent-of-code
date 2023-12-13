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

        public int Summary() => Summary(_rows, 100) + Summary(_cols, 1);

        private static int Summary(string[] x, int modifier)
        {
            for (var r = 0; r < x.Length - 1; r++)
            {
                if (IsValidSymmetryLine(x, r))
                    return (r + 1) * modifier;
            }

            return 0;
        }

        public int Fix()
        {
            var originalSummary = Summary();

            var rowCandidates = new List<(int Row, int Index)>();
            for (var r0 = 0; r0 < _rows.Length; r0++)
            {
                for (var r1 = r0 + 1; r1 < _rows.Length; r1++)
                {
                    var (differs, index) = DiffersByOne(_rows[r0], _rows[r1]);
                    if (differs) rowCandidates.Add((r0, index));
                }
            }

            foreach (var (row, index) in rowCandidates)
            {
                var @fixed = ImmutableArray.CreateBuilder<string>(_rows.Length);
                @fixed.AddRange(_rows);

                var cells = @fixed[row].ToCharArray();
                cells[index] = cells[index] == '.' ? '#' : '.';
                @fixed[row] = string.Join("", cells);
                var fixedSummary = Summary(@fixed.ToArray(), 100);
                if (fixedSummary > 0 && fixedSummary != originalSummary)
                    return fixedSummary;
            }

            var colCandidates = new List<(int Col, int Index)>();
            for (var c0 = 0; c0 < _cols.Length; c0++)
            {
                for (var c1 = c0 + 1; c1 < _cols.Length; c1++)
                {
                    var (differs, index) = DiffersByOne(_cols[c0], _cols[c1]);
                    if (differs) colCandidates.Add((c0, index));
                }
            }

            foreach (var (col, index) in colCandidates)
            {
                var @fixed = ImmutableArray.CreateBuilder<string>(_cols.Length);
                @fixed.AddRange(_cols);

                var cells = @fixed[col].ToCharArray();
                cells[index] = cells[index] == '.' ? '#' : '.';
                @fixed[col] = string.Join("", cells);
                var fixedSummary = Summary(@fixed.ToArray(), 1);
                if (fixedSummary > 0 && fixedSummary != originalSummary)
                    return fixedSummary;
            }

            return 0;
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
            return (true, idx);
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
