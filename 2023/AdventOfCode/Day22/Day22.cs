using Vec3 = (int X, int Y, int Z);
using Vec2 = (int X, int Y);

namespace AdventOfCode.Day22;

public sealed class Day22(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day22";

    private readonly string[] _example =
    [
        "1,0,1~1,2,1",
        "0,0,2~2,0,2",
        "0,2,3~2,2,3",
        "0,0,4~0,2,4",
        "2,0,5~2,2,5",
        "0,1,6~2,1,6",
        "1,1,8~1,1,9"
    ];

    [Fact]
    public void ExampleOne() => CountDisentegratableBlocks(_example).Should().Be(5);

    [Fact]
    public void PartOne() => WriteOutput(CountDisentegratableBlocks(Input));

    [Fact]
    public void ExampleTwo() => CountChainReactions(_example).Should().Be(7);

    [Fact]
    public void PartTwo() => WriteOutput(CountChainReactions(Input));

    private int CountChainReactions(string[] input)
    {
        var blocks = ParseBlocks(input);
        var atRest = SimulateSettling(blocks);
        var chains = 0;
        var aboveLookup = GenerateLookup(atRest, (b, bs) => bs.Where(a => a.Overlaps(b) && a.Start.Z == b.End.Z + 1));
        var belowLookup = GenerateLookup(atRest, (b, bs) => bs.Where(a => a.Overlaps(b) && a.End.Z == b.Start.Z - 1));
        foreach (var block in atRest)
        {
            var removed = new HashSet<Block>([block]);
            var queue = new Queue<Block>([block]);
            while (queue.TryDequeue(out var current))
            {
                var above = aboveLookup[current];
                foreach (var a in above)
                {
                    if (belowLookup[a].All(removed.Contains))
                    {
                        queue.Enqueue(a);
                        removed.Add(a);
                    }
                }
            }
 

            chains += removed.Count - 1;
        }

        return chains;

        static IReadOnlyDictionary<Block, Block[]> GenerateLookup(List<Block> blocks, Func<Block, List<Block>, IEnumerable<Block>> query)
        {
            var result = new Dictionary<Block, Block[]>(blocks.Count);
            foreach (var block in blocks)
            {
                result.Add(block, query.Invoke(block, blocks).ToArray());
            }

            return result;
        }
    }

    private static Block[] ParseBlocks(string[] input)
    {
        return input.Select(i => i.Split('~'))
            .Select(i => new Block(Start: Parse(i[0]), End: Parse(i[1])))
            .OrderBy(b => b.Start.Z)
            .ToArray();

        static Vec3 Parse(string val)
        {
            var split = val.Split(',').Select(i => int.Parse(i)).ToArray();
            return (split[0], split[1], split[2]);
        }
    }

    private List<Block> SimulateSettling(Block[] blocks)
    {
        var atRest = new List<Block>();
        foreach (var block in blocks)
        {
            var z = block.Start.Z;
            while (z > 1)
            {
                if (atRest.Any(a => a.End.Z == z - 1 && block.Overlaps(a)))
                    break;
                z--;
            }
            var newStart = block.Start.WithZ(z);
            var newEnd = block.End.WithZ(block.End.Z - block.Start.Z + newStart.Z);

            atRest.Add(new Block(newStart, newEnd));
        }

        return atRest;
    }

    private int CountDisentegratableBlocks(string[] input)
    {
        var blocks = ParseBlocks(input);

        var atRest = SimulateSettling(blocks);
        var zappable = 0;
        foreach (var block in atRest)
        {
            if (!IsLoadBearing(block, atRest, out _))
                zappable += 1;
        }

        return zappable;
    }

    private static bool IsLoadBearing(Block supporter, List<Block> blocks, out IEnumerable<Block> above)
    {
        above = blocks.Where(a => a.Start.Z == supporter.End.Z + 1 && supporter.Overlaps(a));
        // nothing above so can be zapped
        if (!above.Any(a => a.Start.Z == supporter.End.Z + 1 && supporter.Overlaps(a)))
            return false;

        // is there at least another one below it to support it
        if (above.All(abv => blocks.Count(ar => ar.End.Z == abv.Start.Z - 1 && ar.Overlaps(abv)) > 1))
            return false;

        return true;
    }

    private record Block(Vec3 Start, Vec3 End)
    {
        public bool Overlaps(Block other)
        {
            var otherFootprint = other.Footprint();
            return Footprint().Any(f => otherFootprint.Contains(f));
        }

        public IEnumerable<Vec2> Footprint()
        {
            if (Start.X != End.X)
                for (var x = Start.X; x <= End.X; x++)
                    yield return (x, Start.Y);
            else if (Start.Y != End.Y)
                for (var y = Start.Y; y <= End.Y; y++)
                    yield return (Start.X, y);
            else
                yield return (Start.X, Start.Y);
        }

        public override string ToString() => $"{Start.X},{Start.Y},{Start.Z}~{End.X},{End.Y},{End.Z}";
    }
}

public static class Vec3Extensions
{
    public static Vec3 WithZ(this Vec3 self, int newZ) =>
        (self.X, self.Y, newZ);


}
