namespace AdventOfCode.Day07;

public sealed class Day07(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 07);

    private readonly string[] _example =
    [
        ".......S.......",
        "...............",
        ".......^.......",
        "...............",
        "......^.^......",
        "...............",
        ".....^.^.^.....",
        "...............",
        "....^.^...^....",
        "...............",
        "...^.^...^.^...",
        "...............",
        "..^...^.....^..",
        "...............",
        ".^.^.^.^.^...^.",
        "..............."
    ];

    [Fact]
    public void ExampleOne() => CountBeamSplits(_example).Should().Be(21);

    [Fact]
    public async Task PartOne() => WriteOutput(CountBeamSplits(await ReadInputLines()));

    [Fact]
    public void ExampleTwo() => CountUniquePaths(_example).Should().Be(40);

    [Fact]
    public async Task PartTwo() => WriteOutput(CountUniquePaths(await ReadInputLines()));

    public static long CountUniquePaths(string[] input)
    {
        var splitters = new HashSet<Pos>();
        Pos start = (-1, -1);
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
            {
                if (input[y][x] == '^')
                    splitters.Add((x, y));
                if (input[y][x] == 'S')
                    start = (x, y);
            }
        var memo = new Dictionary<Pos, long>();
        return CountPaths(start);

        long CountPaths(Pos pos)
        {
            if (memo.TryGetValue(pos, out var p))
                return p;

            var splitter = splitters
                .Where(s => s.X == pos.X && s.Y > pos.Y)
                .OrderBy(x => x.Y)
                .ToArray();

            if (splitter.Length == 0)
                return 1;

            var (sx, sy) = splitter.First();
            var left = CountPaths((sx - 1, sy));
            var right = CountPaths((sx + 1, sy));
            memo[(sx - 1, sy)] = left;
            memo[(sx + 1, sy)] = right;

            return left + right;
        }
    }

    private static int CountBeamSplits(string[] input)
    {
        var grid = new Grid();
        var beams = new HashSet<Pos>();
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
            {
                grid[(x, y)] = input[y][x];
                if (input[y][x] == 'S')
                    beams.Add((x, y));
            }
        var split = 0;
        while (beams.Any(b => b.Y < input.Length))
        {
            var updatedBeams = new HashSet<Pos>();
            foreach (var (bx, by) in beams)
            {
                var found = grid.TryGetValue((bx, by + 1), out var tile);

                if (!found || tile == '.')
                    updatedBeams.Add((bx, by + 1));
                if (tile == '^')
                {
                    split += 1;
                    updatedBeams.Add((bx - 1, by + 1));
                    updatedBeams.Add((bx + 1, by + 1));
                }
            }

            beams = updatedBeams;
        }

        return split;
    }
}
