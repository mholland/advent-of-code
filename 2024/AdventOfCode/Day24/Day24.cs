using Xunit.Sdk;
using Operation = (string V1, string Op, string V2, string VR);

namespace AdventOfCode.Day24;

public sealed class Day24(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 24);

    private readonly string[] _example =
    [
        "x00: 1",
        "x01: 1",
        "x02: 1",
        "y00: 0",
        "y01: 1",
        "y02: 0",
        "",
        "x00 AND y00 -> z00",
        "x01 XOR y01 -> z01",
        "x02 OR y02 -> z02"
    ];

    private readonly string[] _exampleTwo =
    [
        "x00: 1",
        "x01: 0",
        "x02: 1",
        "x03: 1",
        "x04: 0",
        "y00: 1",
        "y01: 1",
        "y02: 1",
        "y03: 1",
        "y04: 1",
        "",
        "ntg XOR fgs -> mjb",
        "y02 OR x01 -> tnw",
        "kwq OR kpj -> z05",
        "x00 OR x03 -> fst",
        "tgd XOR rvg -> z01",
        "vdt OR tnw -> bfw",
        "bfw AND frj -> z10",
        "ffh OR nrd -> bqk",
        "y00 AND y03 -> djm",
        "y03 OR y00 -> psh",
        "bqk OR frj -> z08",
        "tnw OR fst -> frj",
        "gnj AND tgd -> z11",
        "bfw XOR mjb -> z00",
        "x03 OR x00 -> vdt",
        "gnj AND wpb -> z02",
        "x04 AND y00 -> kjc",
        "djm OR pbm -> qhw",
        "nrd AND vdt -> hwm",
        "kjc AND fst -> rvg",
        "y04 OR y02 -> fgs",
        "y01 AND x02 -> pbm",
        "ntg OR kjc -> kwq",
        "psh XOR fgs -> tgd",
        "qhw XOR tgd -> z09",
        "pbm OR djm -> kpj",
        "x03 XOR y03 -> ffh",
        "x00 XOR y04 -> ntg",
        "bfw OR bqk -> z06",
        "nrd XOR fgs -> wpb",
        "frj XOR qhw -> z04",
        "bqk OR frj -> z07",
        "y03 OR x01 -> nrd",
        "hwm AND bqk -> z03",
        "tgd XOR rvg -> z12",
        "tnw OR pbm -> gnj"
    ];

    [Fact]
    public void ExampleOne() => CalculateOutput(_example).Should().Be(4);

    [Fact]
    public void ExampleTwo() => CalculateOutput(_exampleTwo).Should().Be(2024);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculateOutput(await ReadInputLines()));

    [Fact]
    public async Task DesiredOutput()
    {
        var (variables, _) = ParseInput(await ReadInputLines());
        var xs = variables.Keys
            .Where(k => k.StartsWith('x'))
            .OrderByDescending(x => int.Parse(x[1..]))
            .Aggregate(string.Empty, (agg, cur) => agg + variables[cur]);
        var ys = variables.Keys
            .Where(k => k.StartsWith('y'))
            .OrderByDescending(x => int.Parse(x[1..]))
            .Aggregate(string.Empty, (agg, cur) => agg + variables[cur]);
        var desired = Convert.ToInt64(xs, 2) + Convert.ToInt64(ys, 2);

        WriteOutput("Desired: " + Convert.ToString(desired, 2));
        WriteOutput("Actual:  " + Convert.ToString(CalculateOutput(await ReadInputLines()), 2));
    }

    [Fact]
    public async Task Sorted()
    {
        var (_, ops) = ParseInput(await ReadInputLines());
        var operations = new List<Operation>();
        var queue = new Queue<Operation>([ops.First(o => o.VR == "z45")]);
        var seen = new HashSet<Operation>();
        while (queue.TryDequeue(out var cur))
        {
            operations.Add(cur);
            if (!seen.Add(cur))
                continue;
            foreach (var pre in ops.Where(o => o.VR == cur.V1 || o.VR == cur.V2))
                queue.Enqueue(pre);
        }
        operations.Reverse();
        List<Operation> newOps = [];
        var reqs = new HashSet<string>();
        var zeds = ops.Where(o => o.VR.StartsWith('z')).ToHashSet();
        for (var i = 0; i < operations.Count; i++)
        {
            newOps.Add(operations[i]);
            reqs.Add(operations[i].VR);
            var insert = zeds.Where(z => reqs.Contains(z.V1) && reqs.Contains(z.V2));
            newOps.AddRange(insert);
            foreach (var ins in insert)
                zeds.Remove(ins);
        }

        WriteOutput(Environment.NewLine + string.Join(Environment.NewLine, newOps.Select(x => $"{x.V1} {x.Op} {x.V2} -> {x.VR}")));
    }

    [Fact]
    public async Task PartTwo() => WriteOutput(FindSwapped(await ReadInputLines()));

    private static string FindSwapped(string[] input)
    {
        HashSet<string> swapped = [];
        var (_, ops) = ParseInput(input);
        
        foreach (var op in ops)
        {
            var v1xy = op.V1[0] is 'x' or 'y';
            var v2xy = op.V2[0] is 'x' or 'y';
            var zOut = op.VR.StartsWith('z');
            if (op.VR == "z00" || op.VR == "z45")
                continue;
            if (op.V1 == "x00" || op.V1 == "y00" ||
                op.V2 == "x00" || op.V2 == "y00")
                continue;
            // X XOR Y -> LABEL
            // X AND Y -> LABEL
            // LABEL XOR LABEL -> Z
            // LABEL OR LABEL -> LABEL
            // LABEL AND LABEL -> LABEL
            switch ((v1xy, op.Op, v2xy, zOut))
            {
                case (true, "XOR", true, true):
                case (false, "XOR", false, false):
                case (true, "AND", true, true):
                case (false, "AND", false, true):
                case (false, "OR", false, true):
                    swapped.Add(op.VR);
                    break;
                case (true, "XOR", true, false):
                    if (!ops.Any(o => o.Op == "XOR" && (o.V1 == op.VR || o.V2 == op.VR)))
                        swapped.Add(op.VR);
                    break;
                case (true, "AND", true, false):
                case (false, "AND", false, false):
                    if (!ops.Any(o => o.Op == "OR" && (o.V1 == op.VR || o.V2 == op.VR)))
                        swapped.Add(op.VR);
                    break;
            }
        }


        return string.Join(",", swapped.OrderBy(x => x));
    }

    private static (Dictionary<string, int> Variables, List<Operation> Ops) ParseInput(string[] input)
    {
        var split = Array.IndexOf(input, "");
        var vars = input[..split];
        var variables = vars
            .Select(v => v.Split(':'))
            .Select(v => (Name: v[0], Val: int.Parse(v[1].Trim())))
            .ToDictionary();
        var ops = input[(split + 1)..];
        List<Operation> operations = [];
        foreach (var op in ops)
        {
            var spl = op.Split(" -> ");
            var lhs = spl[0].Split(' ');
            operations.Add((lhs[0], lhs[1], lhs[2], spl[1]));
        }

        return (variables, operations);
    }

    private long CalculateOutput(string[] input)
    {
        var (variables, operations) = ParseInput(input);
        var opFuncs = new Dictionary<string, Func<int, int, int>>
        {
            ["AND"] = (o1, o2) => o1 & o2,
            ["OR"] = (o1, o2) => o1 | o2,
            ["XOR"] = (o1, o2) => o1 ^ o2
        };

        var zedTotal = operations.Count(o => o.VR.StartsWith('z'));
        var executed = new HashSet<Operation>();
        var output = Environment.NewLine;

        while (executed.Count(e => e.VR.StartsWith('z')) < zedTotal)
        {
            var executableOps = operations.Where(o => !executed.Contains(o))
                .Where(o => variables.ContainsKey(o.V1))
                .Where(o => variables.ContainsKey(o.V2));
            foreach (var op in executableOps)
            {
                var (v1, o, v2, vr) = op;
                variables[vr] = opFuncs[o].Invoke(variables[v1], variables[v2]);
                executed.Add(op);
                output += $"{v1} {o} {v2} -> {vr}" + Environment.NewLine;
            }
        }

        WriteOutput(output);

        var zeds = variables.Keys
            .Where(k => k.StartsWith('z'))
            .OrderByDescending(x => int.Parse(x[1..]))
            .Aggregate(string.Empty, (agg, cur) => agg + variables[cur]);

        return Convert.ToInt64(zeds, 2);
    }
}
