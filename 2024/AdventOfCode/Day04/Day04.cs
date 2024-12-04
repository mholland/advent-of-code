namespace AdventOfCode.Day04;

using Pos = (int X, int Y);

public sealed class Day04(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 04);

    private readonly string[] _example =
    [
        "MMMSXXMASM",
        "MSAMXMSMSA",
        "AMXSXMAAMM",
        "MSAMASMSMX",
        "XMASAMXAMM",
        "XXAMMXXAMA",
        "SMSMSASXSS",
        "SAXAMASAAA",
        "MAMMMXMMMM",
        "MXMXAXMASX"
    ];

    [Fact]
    public void ExampleOne() => CountXmasOccurrences(_example).Lines.Should().Be(18);

    [Fact]
    public async Task PartOne() => WriteOutput(CountXmasOccurrences(await ReadInputLines()).Lines);

    [Fact]
    public void ExampleTwo() => CountXmasOccurrences(_example).Crosses.Should().Be(9);

    [Fact]
    public async Task PartTwo() => WriteOutput(CountXmasOccurrences(await ReadInputLines()).Crosses);

    private static (int Lines, int Crosses) CountXmasOccurrences(string[] input)
    {
        Dictionary<Pos, char> wordSearch = [];
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            wordSearch.Add((x, y), input[y][x]);
        }

        List<Pos> dirs = [];
        int[] d = [-1, 0, 1];
        dirs.AddRange(d.SelectMany(_ => d, (d1, d2) => (X: d1, Y: d2))
            .Where(dr => dr != (0, 0)));

        var lines = wordSearch
            .Keys
            .Where(k => wordSearch[k] == 'X')
            .Sum(start => dirs.Count(dir => DirectionContainsWord(wordSearch, start, dir)));

        var centres = wordSearch.Keys.Where(k => wordSearch[k] == 'A');
        var crosses = centres.Count(centre => CrossContainsWord(wordSearch, centre));

        return (lines, crosses);
    }

    private static bool CrossContainsWord(IDictionary<Pos, char> wordSearch, Pos centre)
    {
        Pos[] corners = [(-1, -1), (-1, 1)];
        return corners.All(DiagonalMatches);
        
        bool DiagonalMatches(Pos corner)
        {
            var startCorner = centre.Add(corner);
            var oppositeCorner = centre.Add(corner.OppositeDirection());
            if (!wordSearch.TryGetValue(startCorner, out var c) ||
                !wordSearch.TryGetValue(oppositeCorner, out var o)) return false;

            return (c, o) switch
            {
                ('S', 'M') => true,
                ('M', 'S') => true,
                _ => false
            };
        }
    }

    private static bool DirectionContainsWord(IDictionary<Pos, char> wordSearch, Pos start, Pos dir)
    {
        const string xmas = "XMAS";
        var cursor = start;
        foreach (var l in xmas)
        {
            if (!wordSearch.TryGetValue(cursor, out var letter))
                return false;
            if (letter != l)
                return false;

            cursor = cursor.Add(dir);
        }

        return true;
    }
}

public static class Extensions
{
    public static Pos Add(this Pos pos, Pos dir) => (pos.X + dir.X, pos.Y + dir.Y);
    public static Pos OppositeDirection(this Pos pos) => (pos.X * -1, pos.Y * -1);
}
