using Xunit.Abstractions;

namespace EverybodyCodes.Day04;

public class Day04(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => nameof(Day04);

    private readonly string[] _example =
    [
        "3",
        "4",
        "7",
        "8"
    ];
    
    [Fact]
    public void ExampleOne() => CountStrikes(_example).Should().Be(10);

    [Fact]
    public void PartOne() => WriteOutput(CountStrikes(ReadFile("A.txt")));

    [Fact]
    public void PartTwo() => WriteOutput(CountStrikes(ReadFile("B.txt")));
    
    [Fact]
    public void PartThree() => WriteOutput(CountMinimalStrikes(ReadFile("C.txt")));

    private static int CountMinimalStrikes(string[] input)
    {
        var nails = input.Select(int.Parse).ToArray();
        Array.Sort(nails);

        var minOne = CountStrikes(nails, nails[249]);
        var minTwo = CountStrikes(nails, nails[250]);
        return Math.Min(minOne, minTwo);
    }
    
    private static int CountStrikes(string[] input)
    {
        var nails = input.Select(int.Parse).ToArray();

        return CountStrikes(nails, nails.Min());
    }

    private static int CountStrikes(int[] nails, int target) => nails.Sum(n => Math.Abs(n - target));
}