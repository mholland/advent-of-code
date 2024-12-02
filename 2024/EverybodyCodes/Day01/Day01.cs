using Xunit.Abstractions;

namespace EverybodyCodes.Day01;

public class Day01(ITestOutputHelper outputHelper) : TestBase(outputHelper)
{
    protected override string Day => "Day01";

    [Fact]
    public void ExampleOne() => CountPotions("ABBAC", 1).Should().Be(5);

    [Fact]
    public void PartOne() => WriteOutput(CountPotions(ReadAll("A.txt"), 1));

    [Fact]
    public void ExampleTwo() => CountPotions("AxBCDDCAxD", 2).Should().Be(28);

    [Fact]
    public void PartTwo() => WriteOutput(CountPotions(ReadAll("B.txt"), 2));

    [Fact]
    public void ExampleThree() => CountPotions("xBxAAABCDxCC", 3).Should().Be(30);

    [Fact]
    public void PartThree() => WriteOutput(CountPotions(ReadAll("C.txt"), 3));

    private static int CountPotions(string input, int groupSize)
    {
        var triplets = input.Chunk(groupSize);
        var total = 0;
        foreach (var triplet in triplets)
        {
            total += triplet.Sum(RequiredPotions);
            var creatureCount = triplet.Count(c => c != 'x');
            if (creatureCount == 2) total += 2;
            if (creatureCount == 3) total += 6;
        }

        return total;

        static int RequiredPotions(char creature) =>
        creature switch
        {
            'B' => 1,
            'C' => 3,
            'D' => 5,
            _ => 0
        };
    }
}

