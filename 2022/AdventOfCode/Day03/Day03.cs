namespace AdventOfCode.Day03;

public sealed class Day03 : TestBase
{
    protected override string Day => "Day03";

    public Day03(ITestOutputHelper output)
        : base(output)
    {
    }

    private string[] Example = new[]
    {
        "vJrwpWtwJgWrhcsFMMfFFhFp",
        "jqHRNqRjqzjGDLGLrsFMfFZSrLrFZsSL",
        "PmmdzqPrVvPwwTWBwg",
        "wMqvLMZHhHMvwLHjbvcjnnSBnvTQFn",
        "ttgJtRGJQctTZtZT",
        "CrZsJsPPZsGzwwsLwLmpwMDw"
    };

    [Fact]
    public void ExampleOne() => CalculatePrioritySum(Example).Should().Be(157);

    [Fact]
    public void PartOne() => Output.WriteLine($"Priority sum: {CalculatePrioritySum(Input)}");

    [Fact]
    public void ExampleTwo() => CalculateBadgePrioritySum(Example).Should().Be(70);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Badge priority sum: {CalculateBadgePrioritySum(Input)}");

    private int CalculateBadgePrioritySum(string[] input) => 
        Enumerable.Chunk(input, 3)
            .Select(group => group[0].Intersect(group[1]).Intersect(group[2]).First())
            .Select(g => CalculatePriority(g)).Sum();

    private int CalculatePrioritySum(string[] input) =>
        input
            .Select(l => (One: l[..(l.Length / 2)], Two: l[(l.Length / 2)..]))
            .Select(c => c.One.Intersect(c.Two).First())
            .Select(i => CalculatePriority(i))
            .Sum();

    private static int CalculatePriority(char item) => 
        Char.IsUpper(item) ? (int)item - 64 + 26 : (int)item - 96;
}