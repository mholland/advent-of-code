using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day02
{
    public sealed class Day02Tests : TestBase
    {
        protected override string Day => "Day02";

        public Day02Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void NumberOfValidPasswordsExampleIsCalculatedCorrectly() =>
            Day02.CountValidPasswordsPartOne(new[]
                {
                    "1-3 a: abcde",
                    "1-3 b: cdefg",
                    "2-9 c: ccccccccc"
                })
                .Should()
                .Be(2);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Valid passwords {Day02.CountValidPasswordsPartOne(Input)}");

        [Fact]
        public void NumberOfValidPasswordsExampleTwoIsCalculatedCorrectly() =>
            Day02.CountValidPasswordsPartTwo(new[]
                {
                    "1-3 a: abcde",
                    "1-3 b: cdefg",
                    "2-9 c: ccccccccc"
                })
                .Should()
                .Be(1);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Valid passwords {Day02.CountValidPasswordsPartTwo(Input)}");
    }

    public static class Day02
    {
        public static int CountValidPasswordsPartOne(string[] input) =>
            CountMatchingPasswords(input, entry =>
            {
                var characterCount = entry.Password.Count(l => l == entry.Letter);
                return characterCount >= entry.OperandOne && characterCount <= entry.OperandTwo;
            });

        public static int CountValidPasswordsPartTwo(string[] input) =>
            CountMatchingPasswords(input, entry =>
            {
                var firstChar = entry.Password[entry.OperandOne - 1] == entry.Letter;
                var secondChar = entry.Password[entry.OperandTwo - 1] == entry.Letter;

                return firstChar ^ secondChar;
            });

        public sealed class PasswordEntry
        {
            public int OperandOne { get; set; }
            public int OperandTwo { get; set; }
            public char Letter { get; set; }
            public string Password { get; set; }

            public PasswordEntry(Match match)
            {
                OperandOne = int.Parse(match.Groups[1].Value);
                OperandTwo = int.Parse(match.Groups[2].Value);
                Letter = char.Parse(match.Groups[3].Value);
                Password = match.Groups[4].Value;
            }
        }

        public static int CountMatchingPasswords(string[] input, Func<PasswordEntry, bool> matcher) {
            var passwordPattern = new Regex(@"(\d+)\-(\d+) (\w): (\w+)");
            var entries = input
                .Select(e => passwordPattern.Match(e))
                .Where(m => m.Success)
                .Select(m => new PasswordEntry(m))
                .ToArray();

            return entries.Count(matcher.Invoke);
        }
    }
}