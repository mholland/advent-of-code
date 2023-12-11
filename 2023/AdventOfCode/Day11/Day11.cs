namespace AdventOfCode.Day11;

public sealed class Day11(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day11";

    private readonly string[] _example =
    [
        "...#......",
        ".......#..",
        "#.........",
        "..........",
        "......#...",
        ".#........",
        ".........#",
        "..........",
        ".......#..",
        "#...#....."
    ];

    [Fact]
    public void ExampleOne() => FindShortestPaths(_example, 2).Should().Be(374);

    [Fact]
    public void PartOne() => WriteOutput("Paths: " + FindShortestPaths(Input, 2));

    [Fact]
    public void ExampleTwo() => FindShortestPaths(_example, 100).Should().Be(8410);

    [Fact]
    public void PartTwo() => WriteOutput("Expanded paths: " + FindShortestPaths(Input, 1_000_000));

    private static long FindShortestPaths(string[] input, int expansion)
    {
        var galaxies = new List<Galaxy>();
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            if (input[y][x] == '#')
                galaxies.Add(new Galaxy{ X = x, Y = y });
        }

        var noGalaxiesY = new List<int>();
        for (var y = 0; y < input.Length; y++)
        {
            if (!galaxies.Any(g => g.Y == y))
                noGalaxiesY.Add(y);
        }

        var noGalaxiesX = new List<int>();
        for (var x = 0; x < input[0].Length; x++)
        {
            if (!galaxies.Any(g => g.X == x))
                noGalaxiesX.Add(x);
        }

        foreach (var galaxy in galaxies)
        {
            var ctX = noGalaxiesX.Count(g => galaxy.X > g);
            var ctY = noGalaxiesY.Count(g => galaxy.Y > g);
            galaxy.X += ctX * (expansion - 1);
            galaxy.Y += ctY * (expansion - 1);
        }

        var toProcess = galaxies;
        var dists = 0L;
        while (toProcess.Count > 1)
        {
            var first = toProcess.First();
            foreach (var g in toProcess[1..])
            {
                dists += first.Dist(g);
            }
            toProcess = toProcess[1..];
        }

        return dists;
    }

    private sealed class Galaxy
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Dist(Galaxy other) =>
            Math.Abs(other.X - X) + Math.Abs(other.Y - Y);
    }
}
