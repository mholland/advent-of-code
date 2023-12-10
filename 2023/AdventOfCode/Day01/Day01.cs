using System.Text.RegularExpressions;

namespace AdventOfCode.Day01;

public sealed class Day01(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day01";

    private readonly string[] _example =
    {
        "1abc2",
        "pqr3stu8vwx",
        "a1b2c3d4e5f",
        "treb7uchet"
    };

    [Fact]
    public void ExampleOne() => CalculateInitialCalibrationValue(_example).Should().Be(142);

    private readonly string[] _exampleTwo =
    [
        "two1nine",
        "eightwothree",
        "abcone2threexyz",
        "xtwone3four",
        "4nineeightseven2",
        "zoneight234",
        "7pqrstsixteen"
    ];

    [Fact]
    public void PartOne() =>
        WriteOutput("Calibration value: " + CalculateInitialCalibrationValue(Input));

    [Fact]
    public void ExampleTwo() => CalculateFinalCalibrationValue(_exampleTwo).Should().Be(281);

    [Fact]
    public void PartTwo() =>
        WriteOutput("Final calibration value: " + CalculateFinalCalibrationValue(Input));

    public static int CalculateFinalCalibrationValue(string[] input) =>
        CalculateCalibrationValue(
            input,
            @"\d|one|two|three|four|five|six|seven|eight|nine");

    public static int CalculateInitialCalibrationValue(string[] input) =>
        CalculateCalibrationValue(input, @"\d");

    private static int CalculateCalibrationValue(string[] input, string pattern) =>
        input
            .Select(l => (First: Regex.Match(l, pattern), Last: Regex.Match(l, pattern, RegexOptions.RightToLeft)))
            .Select(r => Norm(r.First.Value) + Norm(r.Last.Value))
            .Select(int.Parse)
            .Sum();


    private static string Norm(string val) =>
        val switch
        {
            "one" => "1",
            "two" => "2",
            "three" => "3",
            "four" => "4",
            "five" => "5",
            "six" => "6",
            "seven" => "7",
            "eight" => "8",
            "nine" => "9",
            _ => val
        };
}
