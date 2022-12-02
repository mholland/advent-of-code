namespace AdventOfCode.Day01;

public sealed class Day01
{
    private readonly ITestOutputHelper _output;

    public string Input { get; }

    public Day01(ITestOutputHelper output)
    {
        Input = File.ReadAllText(Path.Combine("Day01", "input.txt"));
        _output = output;
    }

    [Fact]
    public void ExampleOne() =>
        _output
            .WriteLine(MostCaloriesCarried(@"1000
2000
3000

4000

5000
6000

7000
8000
9000

10000").ToString());

    [Fact]
    public void CalculatePartOne() =>
        _output
            .WriteLine($"Most cals carried: {MostCaloriesCarried(Input)}");

    [Fact]
    public void CalculatePartTwo() =>
        _output
            .WriteLine($"Top 3 total cals carried: {TopThreeCaloriesCarried(Input)}");

    private IEnumerable<int> CalculateCaloriesCarried(string input) =>
        input.Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(e => e.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
            .Select(e => e.Select(int.Parse).Sum());

    private int MostCaloriesCarried(string input) =>
        CalculateCaloriesCarried(input).Max();

    private int TopThreeCaloriesCarried(string input) =>
        CalculateCaloriesCarried(input)
            .OrderByDescending(x => x)
            .Take(3)
            .Sum();
}