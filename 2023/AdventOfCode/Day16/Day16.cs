using Coord = (int X, int Y);

namespace AdventOfCode.Day16;

public sealed class Day16(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day16";

    private readonly string[] _example =
    [
        @".|...\....",
        @"|.-.\.....",
        @".....|-...",
        @"........|.",
        @"..........",
        @".........\",
        @"..../.\\..",
        @".-.-/..|..",
        @".|....-|.\",
        @"..//.|...."
    ];

    [Fact]
    public void ExampleOne() => CountEnergisedTiles(_example, new Beam((-1, 0), (1, 0))).Should().Be(46);

    [Fact]
    public void PartOne() => WriteOutput(CountEnergisedTiles(Input, new Beam((-1, 0), (1, 0))));

    [Fact]
    public void ExampleTwo() => FindMaximalEnergisedTiles(_example).Should().Be(51);

    [Fact]
    public void PartTwo() => WriteOutput(FindMaximalEnergisedTiles(Input));

    private static int FindMaximalEnergisedTiles(string[] input) => 
        Enumerable.Range(0, input.Length)
                .Select(y => new Beam((-1, y), (1, 0)))
            .Concat(Enumerable.Range(0, input.Length)
                .Select(y => new Beam((input.Length, y), (-1, 0))))
            .Concat(Enumerable.Range(0, input[0].Length)
                .Select(x => new Beam((x, -1), (0, 1))))
            .Concat(Enumerable.Range(0, input[0].Length)
                .Select(x => new Beam((x, input[0].Length), (0, -1))))
            .Max(b => CountEnergisedTiles(input, b));

    private static int CountEnergisedTiles(string[] input, Beam startBeam)
    {
        var grid = new Dictionary<Coord, char>();
        var energised = new HashSet<Coord>();
        var splittersVisited = new HashSet<(Coord, Axis)>();
        var axes = new Dictionary<Coord, Axis>()
        {
            [(1, 0)] = Axis.Horizontal,
            [(-1, 0)] = Axis.Horizontal,
            [(0, -1)] = Axis.Vertical,
            [(0, 1)] = Axis.Vertical,
        };
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
            {
                grid[(x, y)] = input[y][x];
            }
        var beams = new List<Beam>()
        {
            startBeam
        };
        var i = 0;
        while (beams.Count != 0)
        {
            var toRemove = new List<Beam>();
            var toAdd = new List<Beam>();
            foreach (var beam in beams)
            {
                var nextPos = beam.Next();
                if (!grid.TryGetValue(nextPos, out var nextCell))
                {
                    toRemove.Add(beam);
                    continue;
                }

                energised.Add(nextPos);
                if (splittersVisited.Contains((nextPos, axes[beam.Dir])))
                {
                    toRemove.Add(beam);
                    continue;
                }
                if (nextCell is '|' or '-')
                    splittersVisited.Add((nextPos, axes[beam.Dir]));
                beam.Pos = nextPos;
                switch ((nextCell, beam.Dir))
                {
                    case ('.', _):
                    case ('-', (-1, 0)):
                    case ('-', (1, 0)):
                    case ('|', (0, -1)):
                    case ('|', (0, 1)):
                        break;
                    case ('/', (-1, 0)):
                    case ('\\', (1, 0)):
                        beam.Dir = (0, 1);
                        break;
                    case ('/', (0, 1)):
                    case ('\\', (0, -1)):
                        beam.Dir = (-1, 0);
                        break;
                    case ('\\', (-1, 0)):
                    case ('/', (1, 0)):
                        beam.Dir = (0, -1);
                        break;
                    case ('\\', (0, 1)):
                    case ('/', (0, -1)):
                        beam.Dir = (1, 0);
                        break;
                    case ('-', (0, 1)):
                    case ('-', (0, -1)):
                        beam.Dir = (-1, 0);
                        toAdd.Add(new Beam(nextPos, (1, 0)));
                        break;
                    case ('|', (1, 0)):
                    case ('|', (-1, 0)):
                        beam.Dir = (0, -1);
                        toAdd.Add(new Beam(nextPos, (0, 1)));
                        break;
                    default:
                        throw new Exception("huh");
                }
            }
            foreach (var r in toRemove) beams.Remove(r);
            foreach (var a in toAdd) beams.Add(a);

            if (i++ > 1_000_000_000) throw new Exception("uh oh");
        }
        return energised.Count;
    }

    private enum Axis
    {
        Horizontal,
        Vertical
    }

    private record Beam
    {
        public Coord Pos { get; set; }
        public Coord Dir { get; set; }

        public Beam(Coord pos, Coord dir)
        {
            Pos = pos;
            Dir = dir;
        }

        public Coord Next() => (Pos.X + Dir.X, Pos.Y + Dir.Y);
    }
}
