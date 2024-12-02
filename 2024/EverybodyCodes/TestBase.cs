using System.Numerics;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace EverybodyCodes;

public abstract class TestBase(ITestOutputHelper output)
{
    protected readonly ITestOutputHelper Output = output;
    protected abstract string Day { get; }

    protected void WriteOutput<T>(T output, [CallerMemberName] string? callerName = null) where T : INumber<T> =>
        WriteOutput(output.ToString(), callerName);

    protected void WriteOutput(string? output, [CallerMemberName] string? callerName = null) =>
        Output.WriteLine($"{Day} {callerName}: {output}");

    protected string[] ReadFile(string name) =>
        File.ReadAllLines(Path.Combine(Day, name));

    protected string ReadAll(string name) =>
        File.ReadAllText(Path.Combine(Day, name));
}