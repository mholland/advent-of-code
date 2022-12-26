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

    private static int CalculateBeaconlessPositions(string[] input, int yPos)
    {
        var sensors = input.Select(i => i.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (Sensor: (X: x[2], Y: x[3]), Beacon: (X: x[8], Y: x[9])));
        var parsedSensors = new List<Sensor>();
        parsedSensors.AddRange(sensors.Select(s => new Sensor(
            (int.Parse(s.Sensor.X.Split('=')[1][..^1]), int.Parse(s.Sensor.Y.Split('=')[1][..^1])),
            (int.Parse(s.Beacon.X.Split('=')[1][..^1]), int.Parse(s.Beacon.Y.Split('=')[1]))
            )));

        var intersecting = parsedSensors.Where(s => Math.Abs(s.Position.Y - yPos) <= s.DistanceToNearestBeacon()).ToArray();
        var maxDist = intersecting.Max(s => s.Position.X + s.DistanceToNearestBeacon());
        var minDist = intersecting.Min(s => s.Position.X - s.DistanceToNearestBeacon());
        var noBeacon = 0;
        for (var i = minDist; i < maxDist; i++)
        {
            if (intersecting.Any(s => s.NearestBeacon == (i, yPos)))
                continue;
            if (parsedSensors.Any(s => s.Position == (i, yPos)))
                continue;
            if (intersecting.Any(s => s.DistanceTo((i, yPos)) <= s.DistanceToNearestBeacon()))
                noBeacon++;
        }
        return noBeacon;
    }

    private record Sensor((int X, int Y) Position, (int X, int Y) NearestBeacon)
    {
        public int DistanceToNearestBeacon() =>
            DistanceTo(NearestBeacon);

        public int DistanceTo((int X, int Y) to) =>
            Math.Abs(Position.X - to.X) + Math.Abs(Position.Y - to.Y);
    }
}

