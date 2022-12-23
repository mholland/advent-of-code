namespace AdventOfCode.Day08;

public sealed class Day08 : TestBase
{
    protected override string Day => "Day08";
    public Day08(ITestOutputHelper output)
        : base(output)
    {
    }

    private readonly string[] _example = 
    {
        "30373",
        "25512",
        "65332",
        "33549",
        "35390"
    };

    [Fact]
    public void ExampleOne() => VisibleTrees(_example).Should().Be(21);

    [Fact]
    public void PartOne() => Output.WriteLine($"Visible: {VisibleTrees(Input)}");

    [Fact]
    public void PartTwo() => Output.WriteLine($"Scenic score: {HighestScenicScore(Input)}");

    [Fact]
    public void ExampleTwo() => HighestScenicScore(_example).Should().Be(8);

    private static int HighestScenicScore(string[] input)
    {
        var (parsed, width, height) = ParseTrees(input);
        var highest = 0;
        for (var y = 1; y < height - 1; y++)
        for (var x = 1; x < width - 1; x++)
        {
            var current = parsed[y][x];
            var top = 0;
            var bottom = 0;
            var right = 0;
            var left = 0;
            for (var yy = y; yy > 0; yy--)
            {
                top++;
                if (parsed[yy - 1][x] >= current)
                    break;
            }
            for (var yy = y; yy < height - 1; yy++)
            {
                bottom++;
                if (parsed[yy + 1][x] >= current)
                    break;
            }
            for (var xx = x; xx > 0; xx--)
            {
                left++;
                if (parsed[y][xx - 1] >= current)
                    break;
            }
            for (var xx = x; xx < width - 1; xx++)
            {
                right++;
                if (parsed[y][xx + 1] >= current)
                    break;
            }

            var score = top * right * bottom * left;
            if (score > highest) highest = score;
        }
        

        return highest;
    }

    private static int VisibleTrees(string[] input)
    {
        var (parsed, width, height) = ParseTrees(input);
        var visible = width * 2 + height * 2 - 4;

        for (var y = 1; y < height - 1; y++)
            for (var x = 1; x < width - 1; x++)
            {
                if (parsed[y][..x].All(t => t < parsed[y][x]) || //left
                    parsed[y][(x + 1)..].All(t => t < parsed[y][x]) || //right
                    parsed[..y].All(t => t[x] < parsed[y][x]) || //up
                    parsed[(y + 1)..].All(t => t[x] < parsed[y][x])) //down
                    visible++;
            }

        return visible;
    }

    private static (int[][], int, int) ParseTrees(string[] input)
    {
        var height = input.Length;
        var width = input.Max(x => x.Length);
        var parsed = new int[height][];
        for (var y = 0; y < input.Length; y++)
        {
            parsed[y] = new int[width];
            for (var x = 0; x < input[y].Length; x++)
            {
                parsed[y][x] = int.Parse(input[y][x].ToString());
            }
        }

        return (parsed, width, height);
    }
}