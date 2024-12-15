using FluentAssertions.Formatting;

namespace AdventOfCode.Day15;

public sealed class Day15(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 15);

    private readonly string[] _example =
    [
        "########",
        "#..O.O.#",
        "##@.O..#",
        "#...O..#",
        "#.#.O..#",
        "#...O..#",
        "#......#",
        "########",
        "",
        "<^^>>>vv<v>>v<<"
    ];

    private readonly string[] _exampleTwo =
    [
        "##########",
        "#..O..O.O#",
        "#......O.#",
        "#.OO..O.O#",
        "#..O@..O.#",
        "#O#..O...#",
        "#O..O..O.#",
        "#.OO.O.OO#",
        "#....O...#",
        "##########",
        "",
        "<vv>^<v^>v>^vv^v>v<>v^v<v<^vv<<<^><<><>>v<vvv<>^v^>^<<<><<v<<<v^vv^v>^",
        "vvv<<^>^v^^><<>>><>^<<><^vv^^<>vvv<>><^^v>^>vv<>v<<<<v<^v>^<^^>>>^<v<v",
        "><>vv>v^v^<>><>>>><^^>vv>v<^^^>>v^v^<^^>v^^>v^<^v>v<>>v^v^<v>v^^<^^vv<",
        "<<v<^>>^^^^>>>v^<>vvv^><v<<<>^^^vv^<vvv>^>v<^^^^v<>^>vvvv><>>v^<<^^^^^",
        "^><^><>>><>^^<<^^v>>><^<v>^<vv>>v>>>^v><>^v><<<<v>>v<v<v>vvv>^<><<>^><",
        "^>><>^v<><^vvv<^^<><v<<<<<><^v<<<><<<^^<v<^^^><^>>^<v^><<<^>>^v<v^v<v^",
        ">^>>^v>vv>^<<^v<>><<><<v<<v><>v<^vv<<<>^^v^>^^>>><<^v>>v^v><^^>>^<>vv^",
        "<><^^>^^^<><vvvvv^v<v<<>^v<v>v<<^><<><<><<<^^<<<^<<>><<><^^^>^^<>^>v<>",
        "^^>vv<^v^v<vv>^<><v<^v>^^^>>>^^vvv^>vvv<>>>^<^>>>>>^<<^v>^vvv<>^<><<v>",
        "v^^>>><<^^<>>^v^<v^vv<>v^<<>^<^v^v><^<<<><<^<v><v<>vv>>v><v^<vv<>v^<<^",
    ];

    [Fact]
    public void ExampleOne() => GPSSum(_example).Should().Be(2028);

    [Fact]
    public void ExampleTwo() => GPSSum(_exampleTwo).Should().Be(10092);

    [Fact]
    public async Task PartOne() => WriteOutput(GPSSum(await ReadInputLines()));

    private readonly string[] _exampleThree =
    [
        "#######",
        "#...#.#",
        "#.....#",
        "#..OO@#",
        "#..O..#",
        "#.....#",
        "#######",
        "",
        "<vv<<^^<<^^"
    ];

    [Fact]
    public void ExampleThree() => GPSSumWiden(_exampleThree, true);

    [Fact]
    public void ExampleFour() => GPSSumWiden(_exampleTwo, true).Should().Be(9021);

    [Fact]
    public async Task PartTwo() => WriteOutput(GPSSumWiden(await ReadInputLines()));

    private readonly Dictionary<char, Pos> _directions = new()
    {
        ['^'] = new Pos(0, -1),
        ['>'] = new Pos(1, 0),
        ['v'] = new Pos(0, 1),
        ['<'] = new Pos(-1, 0)
    };

    private int GPSSumWiden(string[] input, bool print = false)
    {
        var divider = Array.IndexOf(input, "");
        var g = input[..divider];
        var instructions = string.Join("", input[(divider + 1)..]);

        Dictionary<Pos, char> grid = [];
        var robot = new Pos(0, 0);
        for (var y = 0; y < g.Length; y++)
            for (var x = 0; x < g[y].Length; x++)
            {
                var p1 = new Pos(2 * x, y);
                var p2 = new Pos(2 * x + 1, y);
                var val = input[y][x];

                if (val is '.' or '#')
                {
                    grid[p1] = val;
                    grid[p2] = val;
                }
                if (val is 'O')
                {
                    grid[p1] = '[';
                    grid[p2] = ']';
                }
                if (input[y][x] == '@')
                {
                    grid[p1] = '.';
                    grid[p2] = '.';
                    robot = p1;
                }
            }

        foreach (var direction in instructions)
        {
            var dir = _directions[direction];
            var to = robot.To(dir);
            if (grid[to] == '.')
                robot = to;
            if (grid[to] is '[' or ']')
            {
                var (canMove, boxPositions) = CanMoveBoxes(grid, robot, dir);
                if (!canMove)
                    continue;
                for (var i = boxPositions.Length - 1; i >= 0; i--)
                {
                    var val = grid[boxPositions[i]];
                    var next = boxPositions[i].To(dir);
                    grid[next] = val;
                    grid[boxPositions[i]] = '.';
                }
                robot = to;
            }
        }

        if (print)
            PrintGrid(grid, robot);

        return grid
            .Where(x => x.Value == '[')
            .Aggregate(0, (agg, cur) => agg += cur.Key.Y * 100 + cur.Key.X);
    }

    (bool CanMove, Pos[] BoxPositions) CanMoveBoxes(Dictionary<Pos, char> grid, Pos robot, Pos direction)
    {
        var to = robot.To(direction);
        var seen = new HashSet<Pos>();
        var queue = new Queue<Pos>([to]);
        while (queue.TryDequeue(out var current))
        {
            if (!seen.Add(current))
                continue;
            if (grid[current] is '[')
                queue.Enqueue(current.To(_directions['>']));
            if (grid[current] is ']')
                queue.Enqueue(current.To(_directions['<']));

            var next = current.To(direction);
            if (grid[next] is '#')
                return (false, []);
            if (grid[next] is '.')
                continue;

            queue.Enqueue(next);
        }

        return (true, seen.ToArray());
    }

    private int GPSSum(string[] input, bool print = false)
    {
        var divider = Array.IndexOf(input, "");
        var g = input[..divider];
        var instructions = string.Join("", input[(divider + 1)..]);

        Dictionary<Pos, char> grid = [];
        var robot = new Pos(0, 0);
        for (var y = 0; y < g.Length; y++)
            for (var x = 0; x < g[y].Length; x++)
            {
                var pos = new Pos(x, y);
                grid[pos] = input[y][x];
                if (input[y][x] == '@')
                {
                    robot = pos;
                    grid[pos] = '.';
                }
            }

        foreach (var direction in instructions)
        {
            var dir = _directions[direction];
            var to = robot.To(dir);
            if (grid[to] == '.')
                robot = to;
            if (grid[to] == 'O')
            {
                var nextNonBoxPosition = GetNextNonBox(to, dir);
                if (grid[nextNonBoxPosition] == '.')
                {
                    grid[nextNonBoxPosition] = 'O';
                    grid[to] = '.';
                    robot = to;
                }
            }
            if (print)
                PrintGrid(grid, robot);
        }

        return grid
            .Where(x => x.Value == 'O')
            .Aggregate(0, (agg, cur) => agg += cur.Key.Y * 100 + cur.Key.X);

        Pos GetNextNonBox(Pos c, Pos dir)
        {
            var current = c;
            while (grid[current] == 'O')
                current = current.To(dir);
            return current;
        }
    }

    private void PrintGrid(Dictionary<Pos, char> grid, Pos robot)
    {
        var output = "\n";
        for (var y = 0; y <= grid.Max(g => g.Key.Y); y++)
        {
            var line = "";
            for (var x = 0; x <= grid.Max(g => g.Key.X); x++)
            {
                var p = new Pos(x, y);
                line += p == robot ? '@' : grid[new Pos(x, y)];
            }
            output += line + "\n";
        }
        WriteOutput(output);
    }

    private record Pos(int X, int Y)
    {
        public Pos To(Pos dir) => this with { X = X + dir.X, Y = Y + dir.Y };
    }
}
