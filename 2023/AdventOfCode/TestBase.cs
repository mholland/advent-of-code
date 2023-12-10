using System.Reflection;
using System.Runtime.CompilerServices;

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

    protected void WriteOutput(int output, [CallerMemberName] string? callerName = null) =>
        WriteOutput(output.ToString(), callerName);

    protected void WriteOutput(string output, [CallerMemberName] string? callerName = null) =>
        Output.WriteLine($"{Day} {callerName}: {output}");

    protected string[] ReadFile(string name) =>
        File.ReadAllLines(Path.Combine(Day, name));

    protected string ReadAll(string name) =>
        File.ReadAllText(Path.Combine(Day, name));
}
