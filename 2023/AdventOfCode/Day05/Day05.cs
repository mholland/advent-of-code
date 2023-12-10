using Range = (long Start, long End);

namespace AdventOfCode.Day05;

public sealed class Day05(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day05";

    private readonly string _example =
        @"seeds: 79 14 55 13

seed-to-soil map:
50 98 2
52 50 48

soil-to-fertilizer map:
0 15 37
37 52 2
39 0 15

fertilizer-to-water map:
49 53 8
0 11 42
42 0 7
57 7 4

water-to-light map:
88 18 7
18 25 70

light-to-temperature map:
45 77 23
81 45 19
68 64 13

temperature-to-humidity map:
0 69 1
1 0 69

humidity-to-location map:
60 56 37
56 93 4";
    

    [Fact]
    public void ExampleOne() => FindLowestIndividualLocation(_example).Should().Be(35);

    [Fact]
    public void PartOne() =>
        WriteOutput("Lowest location: " + FindLowestIndividualLocation(ReadAll("input.txt")));

    [Fact]
    public void ExampleTwo() => FindLowestRangeLocation(_example).Should().Be(46);

    [Fact]
    public void PartTwo() =>
        WriteOutput("Lowest ranged location: " + FindLowestRangeLocation(ReadAll("input.txt")));

    private static long FindLowestIndividualLocation(string input)
    {
        var (seeds, mappers) = ParseInput(input);

        var finalLocs = new List<long>();
        foreach (var seed in seeds)
        {
            var loc = seed;
            foreach (var mapper in mappers)
            {
                loc = mapper.GetDestination(loc);
            }
            finalLocs.Add(loc);
        }

        return finalLocs.Min();
    }

    private static long FindLowestRangeLocation(string input)
    {
        var (seeds, mappers) = ParseInput(input);

        var starts = seeds
            .Chunk(2)
            .Select(c => (Start: c[0], End: c[0] + c[1]))
            .ToArray();
        
        var mins = new List<long>();
        foreach (var start in starts)
        {
            List<Range> ranges = [start];
            foreach (var mapper in mappers)
            {
                ranges = mapper.GetDestinationRanges(ranges);
            }
            mins.Add(ranges.Min(r => r.Start));
        }

        return mins.Min();
    }

    private static (long[] Seeds, IReadOnlyList<Mapper> Mappers) ParseInput(string input)
    {
        var mappings = input.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var s = mappings[0].Split(':')[1];
        var seeds = s.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

        var mappers = new List<Mapper>();
        foreach (var block in mappings[1..])
        {
            var split = block.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var mm = new List<Mapping>();

            foreach (var mapping in split[1..])
            {
                var values = mapping.Split(" ").Select(long.Parse).ToArray();
                mm.Add(new Mapping(values[0], values[1], values[2]));
            }

            mappers.Add(new Mapper(mm));
        }

        return (seeds, mappers);
    }

    private record Mapper(IEnumerable<Mapping> Mappings)
    {
        public long GetDestination(long source)
        {
            var relevantMappings = Mappings.Where(m => m.IsSourceInRange(source));
            if (!relevantMappings.Any())
                return source;

            var mapping = relevantMappings.First();
            return mapping.GetDestination(source);
        }

        public List<Range> GetDestinationRanges(List<Range> ranges)
        {
            var newRanges = new List<Range>();

            var current = ranges;
            foreach (var mapping in Mappings)
            {
                var sourceStart = mapping.SourceStart;
                var sourceEnd = mapping.SourceEnd;

                var toProcess = new List<Range>();
                foreach (var (start, end) in current)
                {
                    //              |src         r         size|
                    // |s     e|
                    //                                            |s      e|
                    //        |s          e|
                    //                                |s          e|
                    //                    |s        e|
                    var lower = (Start: start, End: Math.Min(sourceStart, end));
                    if (lower.End > lower.Start) toProcess.Add(lower);

                    var greater = (Start: Math.Max(sourceEnd, start), End: end);
                    if (greater.End > greater.Start) toProcess.Add(greater);

                    var mappable = (Start: Math.Max(start, sourceStart), End: Math.Min(sourceEnd, end));
                    if (mappable.End > mappable.Start)
                        newRanges.Add((
                            mapping.DestinationStart + mappable.Start - mapping.SourceStart,
                            mapping.DestinationStart + mappable.End - mapping.SourceStart));
                }
                current = toProcess;
            }
            newRanges.AddRange(current);
            return newRanges;
        }
    }

    private record Mapping(long DestinationStart, long SourceStart, long Range)
    {
        public long SourceEnd { get; } = SourceStart + Range;
        public long DestinationEnd { get; } = DestinationStart + Range;

        public bool IsSourceInRange(long source) => 
            source >= SourceStart && source < SourceStart + Range;

        public long GetDestination(long source) =>
            DestinationStart + (source - SourceStart);
    }
}
