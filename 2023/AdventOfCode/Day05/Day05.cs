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
    public void ExampleOne() => FindLowestLocation(_example).Should().Be(35);

    [Fact]
    public void PartOne() =>
        Output.WriteLine("Lowest location: " + FindLowestLocation(ReadAll("input.txt")));

    private static long FindLowestLocation(string input)
    {
        var mappings = input.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var s = mappings[0].Split(':')[1];
        var seeds = s.Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse).ToArray();

        var mappers = new List<Mapper>();
        foreach (var block in mappings[1..])
        {
            var split = block.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            var label = split[0].Split(' ')[0].Split("-");
            var from = label[0];
            var to = label[2];
            var mm = new List<Mapping>();

            foreach (var mapping in split[1..])
            {
                var values = mapping.Split(" ").Select(long.Parse).ToArray();
                mm.Add(new Mapping(values[0], values[1], values[2]));
            }

            mappers.Add(new Mapper(from, to, mm));
        }

        var finalLocs = new List<long>();
        foreach (var seed in seeds)
        {
            var sd = mappers.ToDictionary(m => m.From, m => m);
            var start = "seed";
            var loc = seed;
            while (sd.TryGetValue(start, out var mapper))
            {
                loc = mapper.GetDestination(loc);
                start = mapper.To;
            }
            finalLocs.Add(loc);
        }

        return finalLocs.Min();
    }

    private record Mapper(string From, string To, IEnumerable<Mapping> Mappings)
    {
        public long GetDestination(long source)
        {
            var relevantMappings = Mappings.Where(m => m.IsSourceInRange(source));
            if (!relevantMappings.Any())
                return source;

            var mapping = relevantMappings.First();
            return mapping.GetDestination(source);
        }
    }

    private record Mapping(long DestinationOffset, long SourceOffset, long Range)
    {
        public bool IsSourceInRange(long source) => 
            source >= SourceOffset && source <= SourceOffset + Range;

        public long GetDestination(long source) =>
            DestinationOffset + (source - SourceOffset);
    }
}
