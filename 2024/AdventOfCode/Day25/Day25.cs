namespace AdventOfCode.Day25;

public sealed class Day25(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 25);

    private readonly string _example =
        """
        #####
        .####
        .####
        .####
        .#.#.
        .#...
        .....

        #####
        ##.##
        .#.##
        ...##
        ...#.
        ...#.
        .....

        .....
        #....
        #....
        #...#
        #.#.#
        #.###
        #####

        .....
        .....
        #.#..
        ###..
        ###.#
        ###.#
        #####

        .....
        .....
        .....
        #....
        #.#..
        #.#.#
        #####
        """;

    [Fact]
    public void ExampleOne() => CountFits(_example).Should().Be(3);

    [Fact]
    public async Task PartOne() => WriteOutput(CountFits(await ReadInputFile()));

    private static int CountFits(string input)
    {
        var blocks = input.Split(Environment.NewLine + Environment.NewLine);
        List<Grid> keys = [];
        List<Grid> locks = [];
        foreach (var block in blocks)
        {
            Grid grid = [];
            var split = block.Split(Environment.NewLine);
            var key = split[0].All(s => s == '#');
            for (var y = 0; y < split.Length; y++)
            for (var x = 0; x < split[y].Length; x++) 
            {
                grid[(x, y)] = split[y][x];
            }
            var l = key ? keys : locks;
            l.Add(grid);
        }

        var fits = 0;
        foreach (var key in keys)
        foreach (var @lock in locks)
        {
            var fit = true;
            foreach (var (pos, ch) in key)
            {
                if (ch != '#' || @lock[pos] != '#') continue;
                fit = false;
                break;
            }
            if (fit) fits += 1;
        }

        return fits;
    }
}
