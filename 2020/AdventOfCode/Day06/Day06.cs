using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day06
{
    public sealed class Day06Tests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _input;

        public Day06Tests(ITestOutputHelper output)
        {
            _output = output;
            _input = File.ReadAllText(Path.Combine("Day06", "input.txt"));
        }

        [Fact]
        public void EachGroupDistinctAnswersCountedCorrectly()
        {
            Day06.CountGroupDistinctAnswers(@"abc

a
b
c

ab
ac

a
a
a
a

b")
            .Should()
            .Be(11);
        }

        [Fact]
        public void PartOne() =>
            _output
                .WriteLine($"Distinct answers {Day06.CountGroupDistinctAnswers(_input)}");

        [Fact]
        public void EachGroupConsensusAnswersCountedCorrectly()
        {
            Day06.CountGroupConsensusAnswers(@"abc

a
b
c

ab
ac

a
a
a
a

b")
            .Should()
            .Be(6);
        }

        [Fact]
        public void PartTwo() =>
            _output
                .WriteLine($"Consensus answers: {Day06.CountGroupConsensusAnswers(_input)}");
    }

    public static class Day06
    {
        public static int CountGroupDistinctAnswers(string input) =>
            ExtractIndividualAnswersByGroup(input)
                .Select(g => new HashSet<char>(g.SelectMany(a => a)))
                .Aggregate(0, (cur, next) => cur + next.Count());

        public static int CountGroupConsensusAnswers(string input)
        {
            var groupAnswers = ExtractIndividualAnswersByGroup(input);
            var alphabet = Enumerable.Range('a', 26).Select(c => (char)c).ToArray();

            return groupAnswers.Aggregate(0, (cur, group) =>
                cur + alphabet.Where(a => group.All(g => g.Contains(a))).Count());
        }

        public static IEnumerable<IEnumerable<string>> ExtractIndividualAnswersByGroup(string input) =>
            input
                .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));
    }
}