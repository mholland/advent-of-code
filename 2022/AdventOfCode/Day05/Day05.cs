namespace AdventOfCode.Day05;

public sealed class Day05 : TestBase
{
    protected override string Day => "Day05";
    public Day05(ITestOutputHelper output)
        : base(output)
    {
    }

    private readonly string[] _example = 
    {
        "    [D]    ",
        "[N] [C]    ",
        "[Z] [M] [P]",
        " 1   2   3 ",
        "",
        "move 1 from 2 to 1",
        "move 3 from 1 to 3",
        "move 2 from 2 to 1",
        "move 1 from 1 to 2",
    };

    [Fact]
    public void ExampleOne() => CalculateHeadMessage(_example, 9000).Should().Be("CMZ");

    [Fact]
    public void PartOne() => Output.WriteLine($"Message: {CalculateHeadMessage(Input, 9000)}");

    [Fact]
    public void ExampleTwo() => CalculateHeadMessage(_example, 9001).Should().Be("MCD");

    [Fact]
    public void PartTwo() => Output.WriteLine($"Message: {CalculateHeadMessage(Input, 9001)}");

    private static string CalculateHeadMessage(string[] input, int crateMoverVersion)
    {
        var stacks = GenerateInitialStacks(input);
        var proc = ReadRearrangementProcedure(input);

        foreach (var inst in proc)
        {
            var temp = new char[inst.Amount];
            for (var j = 0; j < inst.Amount; j++)
                temp[j] = stacks[inst.From].Pop();

            if (crateMoverVersion == 9001)
                temp = temp.Reverse().ToArray();

            foreach (var t in temp)
                stacks[inst.To].Push(t);
        }

        return stacks.Aggregate(string.Empty, (message, stack) => message += stack.Peek());
    }

    private static (int Amount, int From, int To)[] ReadRearrangementProcedure(string[] input)
    {
        var stackEnd = Array.IndexOf(input, string.Empty);
        var instructions = input[(stackEnd + 1)..];
        var parsed = new (int, int, int)[instructions.Length];
        for (var i = 0; i < instructions.Length; i++)
        {
            var inst = instructions[i];
            var split = inst.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            parsed[i] = (int.Parse(split[1]), int.Parse(split[3]) - 1, int.Parse(split[5]) - 1);
        }

        return parsed;
    }

    private static Stack<char>[] GenerateInitialStacks(string[] input)
    {
        var numberOfStacks = ParseNumberOfStacks(input);
        var stacks = Enumerable.Range(0, numberOfStacks).Select(_ => new Stack<char>()).ToArray();
        var stackEnd = Array.IndexOf(input, string.Empty);
        for (var i = stackEnd - 2; i >= 0; i--)
        {
            for (var j = 1; j < input[i].Length; j+= 4)
            {
                if (input[i][j] != ' ')
                    stacks[j / 4].Push(input[i][j]);
            }
        }

        return stacks;
    }

    private static int ParseNumberOfStacks(string[] input)
    {
        var stackEnd = Array.IndexOf(input, string.Empty);
        var numbers = input[stackEnd - 1];
        return numbers
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .Max();
    }
}