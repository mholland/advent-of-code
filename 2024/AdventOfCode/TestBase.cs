using System.Net;
using System.Net.Http.Headers;
using System.Numerics;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace AdventOfCode;
public abstract class TestBase(ITestOutputHelper output)
{
    protected ITestOutputHelper Output { get; } = output;
    protected abstract DateOnly Date { get; }
    private string DayDirectory => $"Day{Date.Day:D2}";

    protected async Task<string[]> ReadInputLines([CallerFilePath] string? path = null)
    {
        if (File.Exists(Path.Combine(DayDirectory, "input.txt")))
            return ReadFileLines("input.txt");

        Output.WriteLine("Input not found locally, downloading");

        var handler = new HttpClientHandler { CookieContainer = new CookieContainer() };
        handler.CookieContainer.Add(
            new Cookie("session", Environment.GetEnvironmentVariable("AOC_SESSION"), "/", ".adventofcode.com"));
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri($"https://adventofcode.com/"),
        };

        var message = new HttpRequestMessage(
            HttpMethod.Get,
            new Uri($"/{Date.Year}/day/{Date.Day}/input", UriKind.Relative));
        message.Headers.Add("User-Agent", "Input downloader (https://github.com/mholland/advent-of-code)");
        
        using var result = await httpClient.SendAsync(message);
        result.EnsureSuccessStatusCode();
        var content = await result.Content.ReadAsStringAsync() ?? throw new InvalidOperationException("Missing input");
        Directory.CreateDirectory(DayDirectory);
        await File.WriteAllTextAsync(Path.Combine(DayDirectory, "input.txt"), content);

        if (path is null)
            return ReadFileLines("input.txt");
        var srcDirectory = new FileInfo(path).DirectoryName ?? string.Empty;
        await File.WriteAllTextAsync(Path.Combine(srcDirectory, "input.txt"), content);

        return ReadFileLines("input.txt");
    }

    protected string[] ReadFileLines(string name) =>
        File.ReadAllLines(Path.Combine(DayDirectory, name));

    protected string ReadFile(string name) =>
        File.ReadAllText(Path.Combine(DayDirectory, name));

    protected void WriteOutput<T>(T output, [CallerMemberName] string? callerName = null) where T : INumber<T> =>
        WriteOutput(output.ToString(), callerName);

    protected void WriteOutput(string? output, [CallerMemberName] string? callerName = null) =>
        Output.WriteLine($"{DayDirectory} {callerName}: {output}");
}