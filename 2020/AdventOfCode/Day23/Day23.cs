using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day23
{
    public sealed class Day23Tests
    {
        private readonly ITestOutputHelper _output;

        public Day23Tests(ITestOutputHelper output) => _output = output;

        [Theory]
        [InlineData(1, "54673289")]
        [InlineData(10, "92658374")]
        [InlineData(100, "67384529")]
        public void EndStatesAreCorrect(int iterations, string expectedValue) =>
            PlayGame("389125467", 9, iterations)
                .EndState
                .Should()
                .Be(expectedValue);

        [Fact]
        public void PartOne() =>
            _output
                .WriteLine($"End state after 100 rounds: {PlayGame("872495136", 9, 100).EndState}");

        [Fact]
        public void StarCupsAreCorrectlyCalculated() =>
            PlayGame("389125467", 1_000_000, 10_000_000)
                .StarCups
                .Should()
                .Be(149245887792);

        [Fact]
        public void PartTwo() =>
            _output
                .WriteLine($"Starcups after 1e7 rounds: {PlayGame("872495136", 1_000_000, 10_000_000).StarCups}");

        private (string EndState, long StarCups) PlayGame(string input, int startingCups, int iterations)
        {
            var circle = new LinkedList<int>();
            var picked = new List<LinkedListNode<int>>(3);
            var cups = new Dictionary<int, LinkedListNode<int>>(startingCups);

            foreach (var c in input)
            {
                var label = int.Parse(c.ToString());
                var node = new LinkedListNode<int>(label);
                circle.AddLast(node);
                cups.Add(label, node);
            }

            for (var j = input.Length + 1; j <= startingCups; j++)
            {
                var node = new LinkedListNode<int>(j);
                circle.AddLast(node);
                cups.Add(j, node);
            }

            var current = circle.First;
            for (var i = 0; i < iterations; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    var next = Next(current);
                    picked.Add(next);
                    circle.Remove(next);
                }

                var destination = current.Value;
                do
                {
                    destination -= 1;
                    if (destination < 1)
                        destination = startingCups;
                }
                while (picked.Any(p => p.Value == destination));

                var destinationNode = cups[destination];

                foreach (var p in picked)
                {
                    circle.AddAfter(destinationNode, p);
                    destinationNode = p;
                }
                picked.Clear();

                current = Next(current);
            }

            var cupOne = cups[1];
            var check = Next(cupOne);

            // Well, this really caught me out in part two.
            // Turns out creating 10 million strings is a bit slow!
            var sb = new StringBuilder();
            while (check.Value != 1)
            {
                sb.Append(check.Value);
                check = Next(check);
            }

            var afterOne = 1L * Next(cupOne).Value * Next(Next(cupOne)).Value;

            return (sb.ToString(), afterOne);

            LinkedListNode<int> Next(LinkedListNode<int> node) =>
                node.Next ?? node.List.First;
        }
    }
}