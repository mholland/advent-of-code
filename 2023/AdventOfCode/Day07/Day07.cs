namespace AdventOfCode.Day07;

public sealed class Day07(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day07";

    private readonly string[] _example =
    [
        "32T3K 765",
        "T55J5 684",
        "KK677 28",
        "KTJJT 220",
        "QQQJA 483"
    ];

    [Fact]
    public void ExampleOne() => CalculateRank(_example, false).Should().Be(6440);

    [Fact]
    public void ExampleTwo() => CalculateRank(_example, true).Should().Be(5905);

    [Fact]
    public void PartOne() =>
        WriteOutput("Rank: " + CalculateRank(Input, false));

    [Fact]
    public void PartTwo() =>
        WriteOutput("Rank balanced: " + CalculateRank(Input, true));

    public static IEnumerable<object[]> CardData =>
    [
        [Hand.Parse("23456", "1", false), 1, "high card"],
        [Hand.Parse("A23A4", "1", false), 2, "a pair"],
        [Hand.Parse("23432", "1", false), 3, "two pair"],
        [Hand.Parse("TTT98", "1", false), 4, "three of a kind"],
        [Hand.Parse("23332", "1", false), 5, "full house"],
        [Hand.Parse("AA8AA", "1", false), 6, "four of a kind"],
        [Hand.Parse("AAAAA", "1", false), 7, "five of a kind"]
    ];

    [Theory]
    [MemberData(nameof(CardData))]
    public void RanksAreCorrect(Hand hand, int rank, string because) =>
        hand.Rank().Should().Be(rank, $"hand has {because}");

    private static int CalculateRank(string[] input, bool balanced) =>
        input
            .Select(i => i.Split(" "))
            .Select(h => Hand.Parse(h[0], h[1], balanced))
            .OrderBy(h => h).Select((h, i) => h.Bid * (i + 1))
            .Sum();

    public class Hand : IComparable<Hand>
    {
        public int Bid { get; }
        public Card[] Cards { get; }
        public bool Balanced { get; }

        private Hand(Card[] cards, int bid, bool balanced) =>
            (Cards, Bid, Balanced) = (cards, bid, balanced);

        public static Hand Parse(string cards, string bid, bool balanced)
        {
            var parsedCards = new Card[cards.Length];
            for (var i = 0; i < cards.Length; i++)
            {
                parsedCards[i] = balanced && cards[i] == 'J' ? Card.Create('1') : Card.Create(cards[i]);
            }

            return new Hand(parsedCards, int.Parse(bid), balanced);
        }

        public int Rank() => Balanced ? RankBalanced() : RankUnbalanced();

        public int RankBalanced()
        {
            var jokers = Cards.Count(c => c.Value == 1);
            if (jokers == 5) return 7;
            if (jokers == 0) return RankUnbalanced();
            var nonJokers = Cards
                .Where(c => c.Value != 1)
                .GroupBy(c => c.Value)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Count())
                .ToArray();
            nonJokers[0] += jokers;
            return Rank(nonJokers);
        }

        public int RankUnbalanced() => 
            Rank(Cards
                .GroupBy(c => c.Value)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Count())
                .ToArray());

        private static int Rank(int[] counts) =>
            counts switch
            {
                [5] => 7,
                [4, _] => 6,
                [3, 2] => 5,
                [3, ..] => 4,
                [2, 2, _] => 3,
                [2, ..] => 2,
                [1, ..] => 1,
                _ => 0
            };

        public int CompareTo(Hand? other)
        {
            if (other is null) return 1;
            var thisRank = Rank();
            var otherRank = other.Rank();
            if (thisRank == otherRank)
            {
                for (var i = 0; i < Cards.Length; i++)
                {
                    if (Cards[i].Value == other.Cards[i].Value)
                        continue;

                    return Cards[i].Value.CompareTo(other.Cards[i].Value);
                }
            }
            return Rank().CompareTo(other.Rank());
        }

        public override string ToString() => string.Join("", Cards.Select(c => c.ToString()));
    }

    public record Card(int Value, char Symbol)
    {
        public static Card Create(char c) => 
            c switch
            {
                > '0' and <= '9' => new Card(c - '0', c),
                'T' => new Card(10, c),
                'J' => new Card(11, c),
                'Q' => new Card(12, c),
                'K' => new Card(13, c),
                'A' => new Card(14, c),
                _ => throw new Exception("Unknown card")
            };

        public override string ToString() => Value == 1 ? "J" : $"{Symbol}";
    }
}
