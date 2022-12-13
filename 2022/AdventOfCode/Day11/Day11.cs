namespace AdventOfCode.Day11;

public sealed class Day11 : TestBase
{
    protected override string Day => "Day11";
    private readonly string[] _example;
    public Day11(ITestOutputHelper output)
        : base(output)
    {
        _example = ReadFile("input.example.txt");
    }

    [Fact]
    public void ExampleOne() => CalculateMonkeyBusiness(_example, 20, true)
        .Should()
        .Be(10605);

    [Fact]
    public void PartOne() => Output.WriteLine($"Monkey business: {CalculateMonkeyBusiness(Input, 20, true)}");

    [Fact]
    public void ExampleTwo() => CalculateMonkeyBusiness(_example, 10000, false)
        .Should()
        .Be(2713310158);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Monkey business {CalculateMonkeyBusiness(Input, 10000, false)}");

    private long CalculateMonkeyBusiness(string[] input, int rounds, bool lowerWorry)
    {
        var monkeys = Enumerable.Chunk(input, 7);
        var parsed = monkeys.Select(Monkey.Parse).ToArray();
        var lcm = parsed.Select(x => x.Divisor).Aggregate(1, (x, y) => x * y);
        for (var i = 0; i < rounds; i++)
        foreach (var m in parsed)
        {
            while (m.HasItems)
            {
                var (item, targetMonkey) = m.InspectItem(lowerWorry, lcm);
                parsed[targetMonkey].TakeItem(item);
            }
        }

        var top = parsed.OrderByDescending(x => x.ItemsInspected).ToArray();
        return (long)top[0].ItemsInspected * (long)top[1].ItemsInspected;
    }

    private sealed class Monkey
    {
        private readonly Queue<long> _items;
        private readonly Func<long, long> _operation;
        private readonly int _trueMonkey;
        private readonly int _falseMonkey;
        public int Divisor { get; private set; }
        public long ItemsInspected { get; private set; } = 0;

        public bool HasItems => _items.Any();

        private Monkey(long[] startingItems, Func<long, long> operation, int divisor, int trueMonkey, int falseMonkey)
        {
            _items = new Queue<long>(startingItems);
            _operation = operation;
            Divisor = divisor;
            _trueMonkey = trueMonkey;
            _falseMonkey = falseMonkey;
        }

        public static Monkey Parse(string[] input)
        {
            var startingItems = input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[2..]
                .Select(i => i.TrimEnd(','))
                .Select(long.Parse)
                .ToArray();

            var trueMonkey = int.Parse(input[4][^1].ToString());
            var falseMonkey = int.Parse(input[5][^1].ToString());
            var divisor = int.Parse(input[3].Split(' ')[^1]);

            var op = input[2].Split(' ', StringSplitOptions.RemoveEmptyEntries)[^2..];
            Func<long, long> operation = (op[0], long.TryParse(op[1], out var arg)) switch
            {
                ("+", true) => (old) => old + arg,
                ("*", true) => (old) => old * arg,
                ("+", false) => (old) => old + old,
                ("*", false) => (old) => old * old,
                _ => throw new NotSupportedException()
            };

            return new Monkey(startingItems, operation, divisor, trueMonkey, falseMonkey);
        }

        public (long Item, int TargetMonkey) InspectItem(bool lowerWorry, int lcm)
        {
            ItemsInspected++;
            var item = _items.Dequeue();
            var worryLevel = _operation.Invoke(item);
            if (lowerWorry)
                worryLevel /= 3;
            else
                worryLevel %= lcm;

            if (worryLevel % Divisor == 0)
                return (worryLevel, _trueMonkey);
            
            return (worryLevel, _falseMonkey);
        }

        public void TakeItem(long item) => _items.Enqueue(item);
    }
}