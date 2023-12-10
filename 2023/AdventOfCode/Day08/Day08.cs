using Step = (string Left, string Right);

namespace AdventOfCode.Day08;

public sealed class Day08(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day08";

    private readonly string[] _example =
    [
        "LLR",
        "",
        "AAA = (BBB, BBB)",
        "BBB = (AAA, ZZZ)",
        "ZZZ = (ZZZ, ZZZ)"
    ];

    [Fact]
    public void ExampleOne() => CountSteps(_example).Should().Be(6);

    [Fact]
    public void PartOne() => 
        WriteOutput("Steps to ZZZ: " + CountSteps(Input));

    private static string[] _exampleTwo =
    [
        "LR",
        "",
        "11A = (11B, XXX)",
        "11B = (XXX, 11Z)",
        "11Z = (11B, XXX)",
        "22A = (22B, XXX)",
        "22B = (22C, 22C)",
        "22C = (22Z, 22Z)",
        "22Z = (22B, 22B)",
        "XXX = (XXX, XXX)"
    ];

    [Fact]
    public void ExampleTwo() => CountAllSteps(_exampleTwo).Should().Be(6);

    [Fact]
    public void PartTwo() =>
        WriteOutput("All steps " + CountAllSteps(Input));

    private long CountAllSteps(string[] input)
    {
        var (lr, elements) = ParseInput(input);

        var starts = elements.Keys.Where(k => k.EndsWith('A')).ToArray();
        var endCycles = starts
            .Select(s => CountSteps(elements, lr, s, e => e.EndsWith('Z')))
            .ToArray();

        return LCM(endCycles);
    }

    private static long GCD(long a, long b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    private static long LCM(long a, long b) => a * b / GCD(a, b);
    private static long LCM(params long[] l) =>
        l[1..].Aggregate(l[0], (agg, cur) => agg = LCM(agg, cur));

    private static (string LR, IDictionary<string, Step> Elements) ParseInput(string[] input)
    {
        var lr = input[0];

        var elements = input[2..]
            .Select(i => i.Split(" = "))
            .Select(i => (Element: i[0], Steps: ParseStep(i[1])))
            .ToDictionary(d => d.Element, d => d.Steps);

        return (lr, elements);

        static Step ParseStep(string val)
        {
            var split = val[1..^1].Split(", ");
            return (split[0], split[1]);
        }
    }

    private static long CountSteps(string[] input)
    {
        var (lr, elements) = ParseInput(input);

        return CountSteps(elements, lr, "AAA", e => e == "ZZZ");
    }

    public static long CountSteps(IDictionary<string, Step> elements, string lr, string start, Func<string, bool> end)
    {
        var instructions = new Dictionary<char, Func<Step, string>>
        {
            ['L'] = (Step s) => s.Left,
            ['R'] = (Step s) => s.Right
        };

        var count = 0;
        var next = start;
        foreach (var step in GetSteps(lr))
        {
            if (end.Invoke(next)) break;
            var inst = instructions[step];
            next = inst.Invoke(elements[next]);
            count++;
        }

        return count;
    }

    private static IEnumerable<char> GetSteps(string lrSteps)
    {
        var i = 0;
        while (true)
        {
            yield return lrSteps[i++ % lrSteps.Length];
        }
    }
}
