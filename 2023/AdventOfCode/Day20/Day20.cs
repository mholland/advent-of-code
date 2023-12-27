using System.Reflection;

namespace AdventOfCode.Day20;

public sealed class Day20(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day20";

    private readonly string[] _example =
    [
        "broadcaster -> a, b, c",
        "%a -> b",
        "%b -> c",
        "%c -> inv",
        "&inv -> a"
    ];

    private readonly string[] _exampleTwo =
    [
        "broadcaster -> a",
        "%a -> inv, con",
        "&inv -> b",
        "%b -> con",
        "&con -> output",
    ];

    [Fact]
    public void ExampleOne() => CalculatePulseCounts(_example).Should().Be(32000000);

    [Fact]
    public void ExampleTwo() => CalculatePulseCounts(_exampleTwo).Should().Be(11687500);

    [Fact]
    public void PartOne() => WriteOutput(CalculatePulseCounts(Input));

    [Fact]
    public void PartTwo() => WriteOutput(FindButtonPressesRequired(Input).ToString());

    private static long FindButtonPressesRequired(string[] input)
    {
        var modules = ParseInput(input);

        var processingQueue = new Queue<(Pulse Pulse, Module Module, string Input)>();
        var inputsToRx = GetInputs(modules, "rx")
            .SelectMany(m => GetInputs(modules, m))
            .ToDictionary(m => m, _ => new List<int>());
        var cycles = new List<long>();
        var i = 0;
        while (true)
        {
            processingQueue.Enqueue((Pulse.Low, modules["broadcaster"], "button"));
            while (processingQueue.TryDequeue(out var current))
            {
                var (pulse, module, inp) = current;
                var result = module.Receive(inp, pulse);
                if (inputsToRx.TryGetValue(module.Name, out var occs) &&
                    result.Pulse is { IsHigh: true })
                {
                    if (occs.Count == 1)
                        cycles.Add(i - occs[0]);
                    occs.Add(i);
                }
                if (result.Pulse is not null)
                {
                    foreach (var res in result.Destinations)
                    {
                        if (modules.TryGetValue(res, out var mod))
                            processingQueue.Enqueue((result.Pulse, mod, module.Name));
                    }
                }
            }
            if (cycles.Count == inputsToRx.Count)
                return LCM([.. cycles]);
            i++;
        }

        static string[] GetInputs(Dictionary<string, Module> modules, string module) =>
            modules.Keys.Where(m => modules[m].Destinations.Contains(module)).ToArray();
    }

    static long GCD(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    static long LCM(long a, long b) => a * b / GCD(a, b);
    static long LCM(params long[] l) =>
        l[1..].Aggregate(l[0], (agg, cur) => agg = LCM(agg, cur));

    private static Dictionary<string, Module> ParseInput(string[] input)
    {
        var modules = input
            .Select(i => i.Split(" -> "))
            .Select(i => (Name: i[0], Dests: i[1].Split(", ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)));
        var destinationLookup = modules.ToDictionary(m => m.Name, m => m.Dests);

        return modules
            .Select(m => (Name: Norm(m.Name), Module: CreateModule(m.Name, m.Dests, destinationLookup)))
            .ToDictionary(m => m.Name, m => m.Module);

        static Module CreateModule(string name, string[] destinations, Dictionary<string, string[]> lookup) =>
            name[0] switch
            {
                '%' => new FlipFlopModule(Norm(name), destinations),
                '&' => new ConjunctionModule(Norm(name), destinations, GetInputs(lookup, name)),
                _ => new SimpleModule(Norm(name), destinations)
            };
    }

    private static string[] GetInputs(Dictionary<string, string[]> modules, string module) =>
        modules.Keys.Where(k => modules[k].Contains(Norm(module))).Select(Norm).ToArray();

    private static string Norm(string val) => val.Replace("%", "").Replace("&", "");

    private static int CalculatePulseCounts(string[] input)
    {
        var modules = ParseInput(input);

        var processingQueue = new Queue<(Pulse Pulse, Module Module, string Input)>();
        var n = 1000;
        for (var i = 0; i < n; i++)
        {
            processingQueue.Enqueue((Pulse.Low, modules["broadcaster"], "button"));
            while (processingQueue.TryDequeue(out var current))
            {
                var (pulse, module, inp) = current;
                var result = module.Receive(inp, pulse);
                if (result.Pulse is not null)
                {
                    foreach (var res in result.Destinations)
                    {
                        if (modules.TryGetValue(res, out var mod))
                            processingQueue.Enqueue((result.Pulse, mod, module.Name));
                    }
                }
            }
        }

        return (modules.Values.Sum(m => m.LowPulsesSent) + n) * modules.Values.Sum(m => m.HighPulsesSent);
    }

    private abstract class Module(string name, string[] destinations)
    {
        public string[] Destinations = destinations;
        public string Name { get; } = name;
        public int HighPulsesSent { get; protected set; }
        public int LowPulsesSent { get; protected set; }

        public abstract (Pulse? Pulse, string[] Destinations) Receive(string input, Pulse pulse);
        protected (Pulse? Pulse, string[] Destinations) Send(Pulse? pulse, string[] destinations)
        {
            if (pulse is not null)
            {
                if (pulse.IsHigh) HighPulsesSent += destinations.Length;
                else LowPulsesSent += destinations.Length;
            }

            return (pulse, destinations);
        }
    }

    private sealed class SimpleModule(string name, string[] destinations) : Module(name, destinations)
    {
        public override (Pulse? Pulse, string[] Destinations) Receive(string _, Pulse pulse) =>
            Send(pulse, Destinations);
    }

    /// <summary>
    /// Flip-flop modules (prefix %) are either on or off; they are initially off. If a flip-flop module receives a high pulse, it is ignored and nothing happens. However, if a flip-flop module receives a low pulse, it flips between on and off. If it was off, it turns on and sends a high pulse. If it was on, it turns off and sends a low pulse.
    /// </summary>
    /// <param name="destinations"></param>
    private sealed class FlipFlopModule(string name, string[] destinations) : Module(name, destinations)
    {
        private bool _state = false;
        public override (Pulse? Pulse, string[] Destinations) Receive(string _, Pulse pulse)
        {
            if (pulse.IsHigh) return Send(default, []);
            _state = !_state;
            if (_state) return Send(Pulse.High, Destinations);
            return Send(Pulse.Low, Destinations);
        }
    }

    /// <summary>
    /// Conjunction modules (prefix &) remember the type of the most recent pulse received from each of their connected input modules; they initially default to remembering a low pulse for each input. When a pulse is received, the conjunction module first updates its memory for that input. Then, if it remembers high pulses for all inputs, it sends a low pulse; otherwise, it sends a high pulse.
    /// </summary>
    /// <param name="destinations"></param>
    private sealed class ConjunctionModule(string name, string[] destinations, string[] inputs) : Module(name, destinations)
    {
        private readonly Dictionary<string, bool> _states = inputs.ToDictionary(i => i, i => false);
        public override (Pulse? Pulse, string[] Destinations) Receive(string input, Pulse pulse)
        {
            _states[input] = pulse.IsHigh;
            if (_states.Values.All(s => s))
                return Send(Pulse.Low, Destinations);
            return Send(Pulse.High, Destinations);
        }
    }

    private record Pulse
    {
        public bool IsHigh { get; init; }
        public static Pulse High => new() { IsHigh = true };
        public static Pulse Low => new() { IsHigh = false };
    }
}
