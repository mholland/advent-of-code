namespace AdventOfCode.Day06;

public sealed class Day06
{
    private readonly string _input;
    private readonly ITestOutputHelper _output;

    public Day06(ITestOutputHelper output)
    {
        _input = File.ReadAllText("Day06/input.txt");
        _output = output;
    }

    [Theory]
    [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 5)]
    [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 6)]
    [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 10)]
    [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 11)]
    public void ExampleOne(string input, int marker) => 
        CharactersPrePacketStartMarker(input, 4)
            .Should()
            .Be(marker);

    [Fact]
    public void PartOne() => _output.WriteLine($"Preprocess: {CharactersPrePacketStartMarker(_input, 4)}");

    [Theory]
    [InlineData("bvwbjplbgvbhsrlpgdmjqwftvncz", 23)]
    [InlineData("nppdvjthqldpwncqszvftbrmjlhg", 23)]
    [InlineData("nznrnfrfntjfmvfwmzdfjlvtqnbhcprsg", 29)]
    [InlineData("zcfzfwzzqfrljwzlrfnpqdbhtmscgvjw", 26)]
    public void ExampleTwo(string input, int marker) =>
        CharactersPrePacketStartMarker(input, 14)
            .Should()
            .Be(marker);

    [Fact]
    public void PartTwo() => _output.WriteLine($"Preprocess: {CharactersPrePacketStartMarker(_input, 14)}");

    private static int CharactersPrePacketStartMarker(string input, int width)
    {
        for (var i = width; i < input.Length; i++)
        {
            if (input.Take((i-width)..i).Distinct().Count() == width)
                return i;
        }

        return -1;
    }
}