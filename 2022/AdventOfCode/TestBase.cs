namespace AdventOfCode;

public abstract class TestBase
{
    protected readonly ITestOutputHelper Output;
    protected readonly string[] Input;
    protected abstract string Day { get; }

    public TestBase(ITestOutputHelper output)
    {
        Output = output;
        Input = ReadFile("input.txt");
    }

    public string[] ReadFile(string name) =>
        File.ReadAllLines(Path.Combine(Day, name));
}