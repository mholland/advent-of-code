using Card = (int[] Winning, int[] Card);

namespace AdventOfCode.Day04;

public sealed class Day04(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day04";

    private readonly string[] _example =
    [
        "Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53",
        "Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19",
        "Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1",
        "Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83",
        "Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36",
        "Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11"
    ];

    [Fact]
    public void ExampleOne() => CalculateCardPoints(_example).Should().Be(13);

    [Fact]
    public void PartOne() =>
        Output.WriteLine("Card points: " + CalculateCardPoints(Input));

    [Fact]
    public void ExampleTwo() => CountCards(_example).Should().Be(30);

    [Fact]
    public void PartTwo() =>
        Output.WriteLine("Cards won: " + CountCards(Input));

    private static int CountCards(string[] input) 
    {
        var count = 0;
        var cardsWon = ParseCards(input)
            .Select((c, i) => (Id: i + 1, Count: c.Winning.Intersect(c.Card).Count()))
            .ToArray();

        var queue = new Queue<(int Id, int Count)>();
        foreach (var card in cardsWon) queue.Enqueue(card);

        while (queue.TryDequeue(out var game))
        {
            count++;
            if (game.Count == 0) continue;
            var won = cardsWon.Skip(game.Id).Take(game.Count);
            foreach (var w in won) queue.Enqueue(w);
        }

        return count;
    }

    private static double CalculateCardPoints(string[] input) => 
        ParseCards(input)
            .Select(c => c.Winning.Intersect(c.Card).Count())
            .Where(ct => ct > 0)
            .Select(ct => Math.Pow(2, ct - 1))
            .Sum();

    private static IEnumerable<Card> ParseCards(string[] input) =>
        input.Select(i => i.Split(":")[1])
            .Select(c => c.Split("|"))
            .Select(c => (Winning: ParseNumbers(c[0]), Card: ParseNumbers(c[1])));

    private static int[] ParseNumbers(string val) =>
        val.Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse)
            .ToArray();
}
