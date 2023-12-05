namespace AdventOfCode;

public abstract class TestBase
{
    protected readonly ITestOutputHelper Output;
    protected readonly string[] Input;
    protected abstract string Day { get; }

    protected TestBase(ITestOutputHelper output)
    {
        Output = output;
        Input = ReadFile("input.txt");
    }

    protected string[] ReadFile(string name) =>
        File.ReadAllLines(Path.Combine(Day, name));

    protected string ReadAll(string name) =>
        File.ReadAllText(Path.Combine(Day, name));
}
