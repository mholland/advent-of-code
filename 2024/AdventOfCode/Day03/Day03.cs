using System.Text.RegularExpressions;

namespace AdventOfCode.Day03;

public sealed class Day03(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 03);

    [Fact]
    public void ExampleOne() => SumMultiplications(["xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"]).Should().Be(161);

    [Fact]
    public async Task PartOne() => WriteOutput(SumMultiplications(await ReadInputLines()));

    [Fact]
    public void ExampleTwo() => SumMultiplications(["xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"], true).Should().Be(48);

    [Fact]
    public async Task PartTwo() => WriteOutput(SumMultiplications(await ReadInputLines(), true));

    private static int SumMultiplications(string[] input, bool handleConditionals = false)
    {
        var pattern = @"mul\((\d{1,3}),(\d{1,3})\)";
        if (handleConditionals)
            pattern += @"|(do\(\))|(don't\(\))";
        var sum = 0;
        var enabled = true;
        foreach (var line in input) 
        {
            var matches = Regex.Matches(line, pattern);
            foreach (Match match in matches)
            {
                switch (match.Groups[0].Value)
                {
                    case "do()":
                        enabled = true;
                        break;
                    case "don't()":
                        enabled = false;
                        break;
                    default:
                        if (enabled)
                            sum += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
                        break;
                }
            }
        }

        return sum;
    }
}
