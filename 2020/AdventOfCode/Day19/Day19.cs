using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day19
{
    public sealed class Day19Tests : TestBase
    {
        protected override string Day { get; } = "Day19";

        public Day19Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void TerminalRulesAreReturned() =>
            CountValidMessages(new[]
            {
                @"0: ""a""",
                "",
                "a",
                "b",
            })
            .Should()
            .Be(1);

        [Fact]
        public void SubRulesAreTraversedCorrectly() =>
            CountValidMessages(new[]
            {
                "0: 1",
                @"1: ""a""",
                "",
                "a",
                "b",
            })
            .Should()
            .Be(1);

        [Fact]
        public void MultipleSubRulesAreTraversedCorrectly() =>
            CountValidMessages(new[]
            {
                "0: 1 1",
                @"1: ""b""",
                "",
                "aa",
                "bb",
            })
            .Should()
            .Be(1);

        [Fact]
        public void BranchingSingleSubRulesAreTraversedCorrectly() =>
            CountValidMessages(new[]
            {
                "0: 1 | 2",
                @"1: ""a""",
                @"2: ""b""",
                "",
                "ab",
                "aa",
                "b",
                "a",
            })
            .Should()
            .Be(2);

        [Fact]
        public void SingleBranchingMultipleSubRulesAreTraversedCorrectly() =>
            CountValidMessages(new[]
            {
                "0: 1 1 | 2 2",
                @"1: ""a""",
                @"2: ""b""",
                "",
                "ab",
                "ba",
                "aa",
                "bb",
            })
            .Should()
            .Be(2);

        [Fact]
        public void MultipleBranchingSubRulesAreTraversedCorrectly() =>
            CountValidMessages(new[]
            {
                "0: 1 1 | 2 2",
                "1: 3 3",
                "2: 4 4",
                @"3: ""a""",
                @"4: ""b""",
                "",
                "aaaa",
                "bbbb",
                "aa",
                "bb",
            })
            .Should()
            .Be(2);

        [Fact]
        public void MultipleBranchesWithSubRulesAreTraversedCorrectly() =>
            CountValidMessages(new[]
            {
                "0: 1 1 | 4 4",
                "1: 3 3 | 4 4",
                @"3: ""a""",
                @"4: ""b""",
                "",
                "aaaa",
                "bbbb",
                "aabb",
                "bbaa",
                "bb"
            })
            .Should()
            .Be(5);

        [Fact]
        public void ExampleOne() =>
            CountValidMessages(new[]
            {
                "0: 4 1 5",
                "1: 2 3 | 3 2",
                "2: 4 4 | 5 5",
                "3: 4 5 | 5 4",
                @"4: ""a""",
                @"5: ""b""",
                "",
                "ababbb",
                "bababa",
                "abbbab",
                "aaabbb",
                "aaaabbb"
            })
            .Should()
            .Be(2);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Valid message count {CountValidMessages(Input)}");

        [Fact]
        public void ExampleTwo() =>
            CountValidMessages(new[]
            {
                "42: 9 14 | 10 1",
                "9: 14 27 | 1 26",
                "10: 23 14 | 28 1",
                @"1: ""a""",
                "11: 42 31",
                "5: 1 14 | 15 1",
                "19: 14 1 | 14 14",
                "12: 24 14 | 19 1",
                "16: 15 1 | 14 14",
                "31: 14 17 | 1 13",
                "6: 14 14 | 1 14",
                "2: 1 24 | 14 4",
                "0: 8 11",
                "13: 14 3 | 1 12",
                "15: 1 | 14",
                "17: 14 2 | 1 7",
                "23: 25 1 | 22 14",
                "28: 16 1",
                "4: 1 1",
                "20: 14 14 | 1 15",
                "3: 5 14 | 16 1",
                "27: 1 6 | 14 18",
                @"14: ""b""",
                "21: 14 1 | 1 14",
                "25: 1 1 | 1 14",
                "22: 14 14",
                "8: 42",
                "26: 14 22 | 1 20",
                "18: 15 15",
                "7: 14 5 | 1 21",
                "24: 14 1",
                "",
                "abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa",
                "bbabbbbaabaabba",
                "babbbbaabbbbbabbbbbbaabaaabaaa",
                "aaabbbbbbaaaabaababaabababbabaaabbababababaaa",
                "bbbbbbbaaaabbbbaaabbabaaa",
                "bbbababbbbaaaaaaaabbababaaababaabab",
                "ababaaaaaabaaab",
                "ababaaaaabbbaba",
                "baabbaaaabbaaaababbaababb",
                "abbbbabbbbaaaababbbbbbaaaababb",
                "aaaaabbaabaaaaababaa",
                "aaaabbaaaabbaaa",
                "aaaabbaabbaaaaaaabbbabbbaaabbaabaaa",
                "babaaabbbaaabaababbaabababaaab",
                "aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba"
            })
            .Should()
            .Be(3);

        [Fact]
        public void ExampleTwoModified() =>
            CountValidMessagesModified(new[]
            {
                "42: 9 14 | 10 1",
                "9: 14 27 | 1 26",
                "10: 23 14 | 28 1",
                @"1: ""a""",
                "11: 42 31",
                "5: 1 14 | 15 1",
                "19: 14 1 | 14 14",
                "12: 24 14 | 19 1",
                "16: 15 1 | 14 14",
                "31: 14 17 | 1 13",
                "6: 14 14 | 1 14",
                "2: 1 24 | 14 4",
                "0: 8 11",
                "13: 14 3 | 1 12",
                "15: 1 | 14",
                "17: 14 2 | 1 7",
                "23: 25 1 | 22 14",
                "28: 16 1",
                "4: 1 1",
                "20: 14 14 | 1 15",
                "3: 5 14 | 16 1",
                "27: 1 6 | 14 18",
                @"14: ""b""",
                "21: 14 1 | 1 14",
                "25: 1 1 | 1 14",
                "22: 14 14",
                "8: 42",
                "26: 14 22 | 1 20",
                "18: 15 15",
                "7: 14 5 | 1 21",
                "24: 14 1",
                "",
                "abbbbbabbbaaaababbaabbbbabababbbabbbbbbabaaaa",
                "bbabbbbaabaabba",
                "babbbbaabbbbbabbbbbbaabaaabaaa",
                "aaabbbbbbaaaabaababaabababbabaaabbababababaaa",
                "bbbbbbbaaaabbbbaaabbabaaa",
                "bbbababbbbaaaaaaaabbababaaababaabab",
                "ababaaaaaabaaab",
                "ababaaaaabbbaba",
                "baabbaaaabbaaaababbaababb",
                "abbbbabbbbaaaababbbbbbaaaababb",
                "aaaaabbaabaaaaababaa",
                "aaaabbaaaabbaaa",
                "aaaabbaabbaaaaaaabbbabbbaaabbaabaaa",
                "babaaabbbaaabaababbaabababaaab",
                "aabbbbbaabbbaaaaaabbbbbababaaaaabbaaabba"
            })
            .Should()
            .Be(12);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Modified count: {CountValidMessagesModified(Input)}");

        private int CountValidMessages(string[] input)
        {
            var (rules, messages) = ParseInput(input);
            var memo = new Dictionary<int, IEnumerable<string>>();
            var allCombinations = CalculateCombinations(rules, 0, memo).ToHashSet();

            return messages.Count(allCombinations.Contains);
        }

        private int CountValidMessagesModified(string[] input)
        {
            var (rules, messages) = ParseInput(input);
            var fortyTwo = CalculateCombinations(rules, 42, new Dictionary<int, IEnumerable<string>>()).ToArray();
            var thirtyOne = CalculateCombinations(rules, 31, new Dictionary<int, IEnumerable<string>>()).ToArray();

            var validMessages = 0;
            foreach (var m in messages)
            {
                var chunks = Chunk(m, fortyTwo.Max(f => f.Length)).Select((c, i) => (IsFortyTwo: fortyTwo.Contains(c), IsThirtyOne: thirtyOne.Contains(c), Index: i));
                if (chunks.Any(m => !m.IsFortyTwo && !m.IsThirtyOne))
                    continue;

                var fortyTwos = chunks.Where(m => m.IsFortyTwo);
                var thirtyOnes = chunks.Where(m => m.IsThirtyOne);
                if (fortyTwos.Count() == 0 || thirtyOnes.Count() == 0)
                    continue;

                if (fortyTwos.Count() <= thirtyOnes.Count())
                    continue;

                if (fortyTwos.Max(m => m.Index) > thirtyOnes.Min(m => m.Index))
                    continue;

                validMessages++;
            }

            return validMessages;

            IEnumerable<string> Chunk(string message, int size) =>
                Enumerable.Range(0, message.Length / size)
                    .Select(i => message.Substring(i * size, size));
        }

        IEnumerable<string> CalculateCombinations(IDictionary<int, Rule> rules, int ruleNumber, IDictionary<int, IEnumerable<string>> memo)
        {
            var currentRule = rules[ruleNumber];
            if (currentRule.IsTerminal)
                return new[] { currentRule.Character };

            if (memo.TryGetValue(ruleNumber, out var combos))
                return combos;

            var result = currentRule
                .SetOne
                .Select(r => CalculateCombinations(rules, r, memo))
                .CartesianProduct()
                .Select(c => string.Join("", c));

            if (currentRule.SetTwo.Length > 0) 
                result = result.Concat(currentRule
                    .SetTwo
                    .Select(r => CalculateCombinations(rules, r, memo))
                    .CartesianProduct()
                    .Select(c => string.Join("", c)));

            memo[ruleNumber] = result;

            return result;
        }

        private (IDictionary<int, Rule> Rules, string[] Messages) ParseInput(string[] input)
        {
            var splitterLine = Array.IndexOf(input, string.Empty);
            var ruleDefinitions = input[..splitterLine];
            var messages = input[(splitterLine + 1)..];

            var rules = new Dictionary<int, Rule>();
            foreach(var rule in ruleDefinitions)
            {
                var numberDef = rule.Split(':');
                var number = int.Parse(numberDef[0]);
                var r = Rule.Parse(numberDef[1]);

                rules.Add(number, r);
            }

            return (rules, messages);
        }

        public record Rule(string Character, int[] SetOne, int[] SetTwo)
        {
            public static Rule Parse(string rule)
            {
                if (rule.Contains("\""))
                {
                    var character = Regex.Match(rule, @"([a-z])").Groups[1].Value;
                    return new Rule(character, Array.Empty<int>(), Array.Empty<int>());
                }
                var pair = rule.Split('|', StringSplitOptions.RemoveEmptyEntries);
                var setOne = ExtractSet(pair[0]);
                if (pair.Length == 2)
                    return new Rule(default, setOne, ExtractSet(pair[1]));

                return new Rule(default, setOne, Array.Empty<int>());

                int[] ExtractSet(string set) =>
                    set.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
            }

            public bool IsTerminal {get;} = !string.IsNullOrEmpty(Character);
        }
    }

    public static class Extensions
    {
        /// Nabbed (and modified) from:
        /// https://ericlippert.com/2010/06/28/computing-a-cartesian-product-with-linq/
        public static IEnumerable<IEnumerable<string>> CartesianProduct(this IEnumerable<IEnumerable<string>> sequences) => 
            sequences.Aggregate(
                (IEnumerable<IEnumerable<string>>)new[] { Enumerable.Empty<string>() },
                (accumulator, sequence) => 
                    accumulator.SelectMany(accseq => sequence, (accseq, item) =>
                        accseq.Concat(new[] { item })));
    }
}