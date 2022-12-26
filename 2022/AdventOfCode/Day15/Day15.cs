namespace AdventOfCode.Day15;

public sealed class Day15 : TestBase
{
    protected override string Day => "Day15";

    public Day15(ITestOutputHelper output)
        : base(output)
    {
    }

    private readonly string[] _example =
    {
        "Sensor at x=2, y=18: closest beacon is at x=-2, y=15",
        "Sensor at x=9, y=16: closest beacon is at x=10, y=16",
        "Sensor at x=13, y=2: closest beacon is at x=15, y=3",
        "Sensor at x=12, y=14: closest beacon is at x=10, y=16",
        "Sensor at x=10, y=20: closest beacon is at x=10, y=16",
        "Sensor at x=14, y=17: closest beacon is at x=10, y=16",
        "Sensor at x=8, y=7: closest beacon is at x=2, y=10",
        "Sensor at x=2, y=0: closest beacon is at x=2, y=10",
        "Sensor at x=0, y=11: closest beacon is at x=2, y=10",
        "Sensor at x=20, y=14: closest beacon is at x=25, y=17",
        "Sensor at x=17, y=20: closest beacon is at x=21, y=22",
        "Sensor at x=16, y=7: closest beacon is at x=15, y=3",
        "Sensor at x=14, y=3: closest beacon is at x=15, y=3",
        "Sensor at x=20, y=1: closest beacon is at x=15, y=3"
    };

    [Fact]
    public void ExampleOne() => CalculateBeaconlessPositions(_example, 10).Should().Be(26);

    [Fact]
    public void PartOne() => Output.WriteLine($"Beaconless: {CalculateBeaconlessPositions(Input, 2_000_000)}");

    [Fact]
    public void ExampleTwo() => FindDistressTuningFrequency(_example, 20).Should().Be(56000011);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Tuning frequency: {FindDistressTuningFrequency(Input, 4_000_000)}");

    private static ulong FindDistressTuningFrequency(string[] input, int boxSize)
    {
        var sensors = ParseSensors(input);
        for (var y = 0; y <= boxSize; y++)
        {
            var sensorlessRanges = SensorlessRanges(sensors, y);
            var merged = MergeRanges(sensorlessRanges)
                .Where(x => x.X2 >= 0)
                .Where(x => x.X1 <= boxSize)
                .ToArray();
            if (merged[0].X1 < 0)
                merged[0] = (0, merged[0].X2);
            if (merged[^1].X2 > boxSize)
                merged[^1] = (merged[^1].X1, boxSize);
            if (merged.Length > 1)
                return 4_000_000 * ((ulong)merged[0].X2 + 1) + (ulong)y;
        }

        return 0;
    }

    private static int CalculateBeaconlessPositions(string[] input, int yPos)
    {
        var sensors = ParseSensors(input);
        var ranges = SensorlessRanges(sensors, yPos);

        var merged = MergeRanges(ranges);
        var beacons = sensors.Where(s => s.NearestBeacon.Y == yPos).Select(x => x.NearestBeacon).Distinct().Count();
        return merged.Aggregate(0, (agg, cur) => agg += cur.X2 - cur.X1 + 1) - beacons;
    }

    private static List<(int X1, int X2)> SensorlessRanges(IEnumerable<Sensor> sensors, int yPos) =>
        sensors.Where(s => Math.Abs(s.Position.Y - yPos) <= s.DistanceToNearestBeacon()).ToArray()
            .Select(x => (Sensor: x, Dist: Math.Abs(x.Position.Y - yPos)))
            .Select(x => (x.Sensor, x.Dist, X1: x.Sensor.Position.X - x.Sensor.DistanceToNearestBeacon() + x.Dist))
            .Select(x => (x.X1, X2: x.Sensor.Position.X + x.Sensor.DistanceToNearestBeacon() - x.Dist))
            .ToList();

    private static List<Sensor> ParseSensors(string[] input)
    {
        var sensors = input.Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (Sensor: (X: x[2], Y: x[3]), Beacon: (X: x[8], Y: x[9])));
        var parsedSensors = new List<Sensor>();
        parsedSensors.AddRange(sensors.Select(s => new Sensor(
            (int.Parse(s.Sensor.X.Split('=')[1][..^1]), int.Parse(s.Sensor.Y.Split('=')[1][..^1])),
            (int.Parse(s.Beacon.X.Split('=')[1][..^1]), int.Parse(s.Beacon.Y.Split('=')[1]))
        )));
        return parsedSensors;
    }

    private static IEnumerable<(int X1, int X2)> MergeRanges(List<(int X1, int X2)> ranges)
    {
        ranges = ranges.OrderBy(x => x.X1).ToList();

        var stack = new Stack<(int X1, int X2)>();
        stack.Push(ranges.First());
        foreach (var rng in ranges.Skip(1))
        {
            var current = stack.Peek();

            if (current.X2 < rng.X1)
            {
                stack.Push(rng);
                continue;
            }

            if (current.X2 >= rng.X2) continue;

            current.X2 = rng.X2;
            stack.Pop();
            stack.Push(current);
        }

        return stack.Reverse().ToList();
    }

    private record Sensor((int X, int Y) Position, (int X, int Y) NearestBeacon)
    {
        public int DistanceToNearestBeacon() =>
            Math.Abs(Position.X - NearestBeacon.X) + Math.Abs(Position.Y - NearestBeacon.Y);
    }
}

