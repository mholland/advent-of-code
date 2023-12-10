using Coord = (int X, int Y);

namespace AdventOfCode.Day10;

public sealed class Day10(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day10";

    private readonly string[] _example =
    [
        ".....",
        ".S-7.",
        ".|.|.",
        ".L-J.",
        "....."
    ];

    private readonly string[] _exampleTwo =
    [
        "..F7.",
        ".FJ|.",
        "SJ.L7",
        "|F--J",
        "LJ..."
    ];

    [Fact]
    public void ExampleOne() => FindLoopFurthestDistance(_example).Length.Should().Be(4);

    [Fact]
    public void ExampleTwo() => FindLoopFurthestDistance(_exampleTwo).Length.Should().Be(8);

    [Fact]
    public void PartOne() => WriteOutput(FindLoopFurthestDistance(Input).Length);

    private readonly string[] _exampleThree = 
    [
        "...........",
        ".S-------7.",
        ".|F-----7|.",
        ".||.....||.",
        ".||.....||.",
        ".|L-7.F-J|.",
        ".|..|.|..|.",
        ".L--J.L--J.",
        "..........."
    ];

    [Fact]
    public void ExampleThree() => FindLoopFurthestDistance(_exampleThree).Enclosed.Should().Be(4);

    private readonly string[] _exampleFour = 
    [
        ".F----7F7F7F7F-7....",
        ".|F--7||||||||FJ....",
        ".||.FJ||||||||L7....",
        "FJL7L7LJLJ||LJ.L-7..",
        "L--J.L7...LJS7F-7L7.",
        "....F-J..F7FJ|L7L7L7",
        "....L7.F7||L7|.L7L7|",
        ".....|FJLJ|FJ|F7|.LJ",
        "....FJL-7.||.||||...",
        "....L---J.LJ.LJLJ..."
    ];

    [Fact]
    public void ExampleFour() => FindLoopFurthestDistance(_exampleFour).Enclosed.Should().Be(8);

    private readonly string[] _exampleFive = 
    [
        "FF7FSF7F7F7F7F7F---7",
        "L|LJ||||||||||||F--J",
        "FL-7LJLJ||||||LJL-77",
        "F--JF--7||LJLJ7F7FJ-",
        "L---JF-JLJ.||-FJLJJ7",
        "|F|F-JF---7F7-L7L|7|",
        "|FFJF7L7F-JF7|JL---7",
        "7-L-JL7||F7|L7F-7F7|",
        "L.L7LFJ|||||FJL7||LJ",
        "L7JLJL-JLJLJL--JLJ.L"
    ];

    [Fact]
    public void ExampleFive() => FindLoopFurthestDistance(_exampleFive).Enclosed.Should().Be(10);

    [Fact]
    public void PartTwo() => WriteOutput(FindLoopFurthestDistance(Input).Enclosed);

    private readonly Coord _up = (0, -1);
    private readonly Coord _down = (0, 1);
    private readonly Coord _left = (-1, 0);
    private readonly Coord _right = (1, 0);

    private (int Length, int Enclosed) FindLoopFurthestDistance(string[] input)
    {
        var validMovements = new Dictionary<char, Coord[]>
        {
            ['|'] = [_up, _down],
            ['-'] = [_left, _right],
            ['F'] = [_down, _right],
            ['J'] = [_up, _left],
            ['7'] = [_down, _left],
            ['L'] = [_up, _right]
        };

        var (map, start) = ParseInput(input);

        var current = start;
        var length = 0;
        var loop = new HashSet<Coord> {start};
        var next = GetNext();
        while (next.Any()) 
        {
            current = current.Dir(next.First());
            loop.Add(current);
            length++;
            next = GetNext();
        }

        IEnumerable<Coord> GetNext() =>
            validMovements[map[current]]
                .Where(n => !loop.Contains(current.Dir(n)));

        return (Length: (length / 2) + (length % 2 == 0 ? 0 : 1),
            Enclosed: CountEnclosedTiles(map, loop));
    }

    private (IDictionary<Coord, char> Map, Coord Start) ParseInput(string[] input)
    {
        var map = new Dictionary<Coord, char>();
        Coord start = default;
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
        {
            var val = input[y][x];
            if (val == 'S')
                start = (x, y);
                
            map.Add((x, y), val);
        }

        map[start] = FindStartValue(map, start);

        return (map, start);
    }

    private static int CountEnclosedTiles(IDictionary<Coord, char> map, HashSet<Coord> loop)
    {
        // Go through each row of the loop and split into pairs of vertical lines going down
        var rows = loop.GroupBy(s => s.Y).OrderBy(g => g.Key).Select(x => x.OrderBy(y => y.X));
        var inside = 0;
        foreach (var row in rows)
        {
            var verticalPairs = row.Where(r => map[r] is '|' or 'F' or '7').Chunk(2);
            foreach (var pair in verticalPairs)
            {
                for (var x = pair[0].X + 1; x < pair[1].X; x++)
                {
                    if (!loop.Contains((x, pair[0].Y)))
                        inside++;
                }
            }
        }

        return inside;
    }

    private char FindStartValue(Dictionary<Coord, char> map, Coord start)
    {
        var next = new List<Coord>(2);
        if (map.TryGetValue(start.Dir(_up), out var u) &&
            u is '|' or 'F' or '7')
            next.Add(_up);

        if (map.TryGetValue(start.Dir(_down), out var d) &&
            d is '|' or 'J' or 'L')
            next.Add(_down);

        if (map.TryGetValue(start.Dir(_left), out var l) &&
            l is '-' or 'F' or 'L')
            next.Add(_left);

        if (map.TryGetValue(start.Dir(_right), out var r) &&
            r is '-' or '7' or 'J')
            next.Add(_right);

        return next switch
        {
            [(0, -1), (0, 1)] => '|',
            [(-1, 0), (1, 0)] => '-',
            [(0, 1), (1, 0)] => 'F',
            [(0, -1), (-1, 0)] => 'J',
            [(0, 1), (-1, 0)] => '7',
            [(0, -1), (1, 0)] => 'L',
            _ => '.'
        };
    }
}

public static class CoordExtensions
{
    public static Coord Dir(this Coord current, Coord direction) =>
        (current.X + direction.X, current.Y + direction.Y);
}