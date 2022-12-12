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
    public void ExampleOne() => CalculateMonkeyBusiness(_example)
        .Should()
        .Be(10605);

    [Fact]
    public void PartOne() => Output.WriteLine($"Monkey business: {CalculateMonkeyBusiness(Input)}");

    private int CalculateMonkeyBusiness(string[] input)
    {
        var monkeys = Enumerable.Chunk(input, 7);
        var parsed = monkeys.Select(Monkey.Parse).ToArray();
        for (var i = 0; i < 20; i++)
        foreach (var m in parsed)
        {
            while (m.HasItems)
            {
                var (item, targetMonkey) = m.InspectItem();
                parsed[targetMonkey].TakeItem(item);
            }
        }

        var top = parsed.OrderByDescending(x => x.ItemsInspected).ToArray();
        return top[0].ItemsInspected * top[1].ItemsInspected;
    }

    private sealed class Monkey
    {
        private readonly Queue<int> _items;
        private readonly Func<int, int> _operation;
        private readonly int _trueMonkey;
        private readonly int _falseMonkey;
        private readonly int _divisor;
        public int ItemsInspected { get; private set; }= 0;

        public bool HasItems => _items.Any();

        private Monkey(int[] startingItems, Func<int, int> operation, int divisor, int trueMonkey, int falseMonkey)
        {
            _items = new Queue<int>(startingItems);
            _operation = operation;
            _divisor = divisor;
            _trueMonkey = trueMonkey;
            _falseMonkey = falseMonkey;
        }

        public static Monkey Parse(string[] input)
        {
            var startingItems = input[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)[2..]
                .Select(i => i.TrimEnd(','))
                .Select(int.Parse)
                .ToArray();

            var trueMonkey = int.Parse(input[4][^1].ToString());
            var falseMonkey = int.Parse(input[5][^1].ToString());
            var divisor = int.Parse(input[3].Split(' ')[^1]);

            var op = input[2].Split(' ', StringSplitOptions.RemoveEmptyEntries)[^2..];
            Func<int, int> operation = (op[0], int.TryParse(op[1], out var arg)) switch
            {
                ("+", true) => (old) => old + arg,
                ("*", true) => (old) => old * arg,
                ("+", false) => (old) => old + old,
                ("*", false) => (old) => old * old,
                _ => throw new NotSupportedException()
            };

            return new Monkey(startingItems, operation, divisor, trueMonkey, falseMonkey);
        }

        public (int Item, int TargetMonkey) InspectItem()
        {
            ItemsInspected++;
            var item = _items.Dequeue();
            var worryLevel = _operation.Invoke(item) / 3;
            if (worryLevel % _divisor == 0)
                return (worryLevel, _trueMonkey);
            
            return (worryLevel, _falseMonkey);
        }

        public void TakeItem(int item) => _items.Enqueue(item);
    }
}