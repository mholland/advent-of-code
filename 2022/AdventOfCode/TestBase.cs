namespace AdventOfCode;

public abstract class TestBase
{
    protected readonly ITestOutputHelper Output;
    protected readonly string[] Input;
    protected abstract string Day { get; }

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
        Input = File.ReadAllLines(Path.Combine(Day, "input.txt"));
    }
}