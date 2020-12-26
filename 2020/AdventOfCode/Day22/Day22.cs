using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day22
{
    public sealed class Day22Tests : TestBase
    {
        protected override string Day { get; } = "Day22";

        public Day22Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void CombatExample() =>
            PlayCombat(new[]
            {
                "Player 1:",
                "9",
                "2",
                "6",
                "3",
                "1",
                "",
                "Player 2:",
                "5",
                "8",
                "4",
                "7",
                "10"
            })
            .Should()
            .Be(306);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Winning hand: {PlayCombat(Input)}");

        [Fact]
        public void RecursiveCombatExample() =>
            PlayRecursiveCombat(new[]
            {
                "Player 1:",
                "9",
                "2",
                "6",
                "3",
                "1",
                "",
                "Player 2:",
                "5",
                "8",
                "4",
                "7",
                "10"
            })
            .Should()
            .Be(291);

        [Fact]
        public void RecursiveCombatGamesTerminates() =>
            PlayRecursiveCombat(new[]
            {
                "Player 1:",
                "43",
                "19",
                "",
                "Player 2:",
                "2",
                "29",
                "14"
            })
            .Should()
            .BeGreaterThan(0);

        [Fact]
        public void DeckEqualityIsCorrectlyCalculated()
        {
            var hashSet = new HashSet<(int[], int[])>(DeckEqualityComparer<(int[], int[])>.Default);
            hashSet.Add((new[] {1, 2, 3}, new[] {4, 5, 6}));
            hashSet.Add((new[] {1, 2, 3}, new[] {4, 5, 6})).Should().BeFalse();
        }

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Winning hand: {PlayRecursiveCombat(Input)}");

        private int PlayCombat(string[] input)
        {
            var cards = DealCards(input);

            while (cards.Values.All(c => c.Any()))
            {
                var hands = cards
                    .Keys
                    .Select(c => (Player: c, Card: cards[c].Dequeue()))
                    .OrderByDescending(c => c.Card)
                    .ToList();

                var handWinner = hands.First().Player;
                foreach (var hand in hands)
                    cards[handWinner].Enqueue(hand.Card);
            }

            var winningDeck = cards.Values.First(c => c.Any());

            return CalculateWinningScore(winningDeck);
        }

        public int PlayRecursiveCombat(string[] input)
        {
            var cards = DealCards(input);

            var winner = PlayGame(cards);

            return CalculateWinningScore(cards[winner]);

            int PlayGame(IDictionary<int, Queue<int>> cards)
            {
                var previousDecks = new HashSet<(int[], int[])>(DeckEqualityComparer<(int[], int[])>.Default);
                while (cards.Values.All(c => c.Any()))
                {
                    var playerOneCards = cards[1].ToArray();
                    var playerTwoCards = cards[2].ToArray();

                    if (previousDecks.Contains((playerOneCards, playerTwoCards)))
                        return 1;

                    previousDecks.Add((playerOneCards, playerTwoCards));

                    var hands = cards
                        .Keys
                        .Select(c => (Player: c, Card: cards[c].Dequeue()))
                        .OrderByDescending(c => c.Card)
                        .ToList();

                    var winner = hands.First().Player;
                    // Decide with a sub-game.
                    if (hands.All(h => cards[h.Player].Count() >= h.Card))
                    {
                        var recursiveGameDeck = hands
                            .Select(h => (Player: h.Player, Cards: cards[h.Player].Take(h.Card)))
                            .ToDictionary(x => x.Player, x => new Queue<int>(x.Cards));

                        winner = PlayGame(recursiveGameDeck);
                        if (winner == 2)
                            hands = hands.OrderByDescending(h => h.Player).ToList();
                        else
                            hands = hands.OrderBy(h => h.Player).ToList();
                    }

                    foreach (var hand in hands)
                        cards[winner].Enqueue(hand.Card);
                }

                return cards.Keys.First(c => cards[c].Any());
            }
        }

        public class DeckEqualityComparer<T> : IEqualityComparer<T>
        {
            public bool Equals(T x, T y) =>
                StructuralComparisons.StructuralEqualityComparer.Equals(x, y);

            public int GetHashCode(T obj) =>
                StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);

            private static DeckEqualityComparer<T> defaultComparer;
            public static DeckEqualityComparer<T> Default = defaultComparer ??= new DeckEqualityComparer<T>();
        }

        private int CalculateWinningScore(IEnumerable<int> winningDeck) =>
            winningDeck
                .Select((c, i) => (Card: c, Multiplier: winningDeck.Count() - i))
                .Aggregate(0, (agg, cur) => agg + (cur.Card * cur.Multiplier));


        private IDictionary<int, Queue<int>> DealCards(string[] input)
        {
            var result = new Dictionary<int, Queue<int>>()
            {
                [1] = new Queue<int>(),
                [2] = new Queue<int>()
            };

            var currentCards = 1;
            for (var i = 1; i < input.Length; i++)
            {
                if (string.IsNullOrEmpty(input[i]))
                {
                    i+= 2;
                    currentCards = 2;
                }
                result[currentCards].Enqueue(int.Parse(input[i]));
            }

            return result;
        }
    }
}