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
        Output.WriteLine("Steps to ZZZ: " + CountSteps(Input));

    private static (string LR, IDictionary<string, Step> Elements) ParseInput(string[] input)
    {
        var lr = input[0];

        var elements = input[2..]
            .Select(i => i.Split(" = "))
            .Select(i => (Element: i[0], Steps: ParseSteps(i[1])))
            .ToDictionary(d => d.Element, d => d.Steps);

        return (lr, elements);

        static Step ParseSteps(string val)
        {
            var split = val[1..^1].Split(", ");
            return (split[0], split[1]);
        }
    }

    private static int CountSteps(string[] input)
    {

        var instructions = new Dictionary<char, Func<Step, string>>
        {
            ['L'] = (Step s) => s.Left,
            ['R'] = (Step s) => s.Right
        };

        var steps = 0;
        var next = "AAA";
        foreach (var step in GetSteps(lr))
        {
            if (next == "ZZZ") break;
            var inst = instructions[step];
            next = inst.Invoke(elements[next]);
            steps++;
        }

        return steps;


        static IEnumerable<char> GetSteps(string lrSteps)
        {
            var i = 0;
            while (true)
            {
                yield return lrSteps[i++ % lrSteps.Length];
            }
        }
    }
}
