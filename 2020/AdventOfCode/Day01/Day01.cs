using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day01
{
    public sealed class Day01Tests : TestBase
    {
        protected override string Day => "Day01";

        public Day01Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void FindsProductOfEntriesSummingTo2020() =>
            Day01.CalculateProductOfTwoEntries(new[]
                {
                    "1721",
                    "979",
                    "366",
                    "299",
                    "675",
                    "1456"
                })
                .Should()
                .Be(514579);

        [Fact]
        public void CalculatePartOne() =>
            Output
                .WriteLine($"Products of entries summing to 2020: {Day01.CalculateProductOfTwoEntries(Input)}");

        [Fact]
        public void FindsProductOfThreeEntriesSummingTo2020() =>
            Day01.CalculateProductOfThreeEntries(new[]
                {
                    "1721",
                    "979",
                    "366",
                    "299",
                    "675",
                    "1456"
                })
                .Should()
                .Be(241861950);

        [Fact]
        public void CalculatePartTwo() =>
            Output
                .WriteLine($"Products of three entries summing to 2020: {Day01.CalculateProductOfThreeEntries(Input)}");
    }

    public static class Day01
    {
        public static int CalculateProductOfTwoEntries(string[] input)
        {
            var entries = input.Select(int.Parse).ToArray();

            for (int i = 0; i < entries.Length; i++)
            {
                var first = entries[i];
                for (var j = i + 1; j < entries.Length; j++)
                {
                    if (first + entries[j] != 2020)
                        continue;

                    return first * entries[j];
                }
            }

            throw new Exception("Unable to find entries summing to 2020");
        }

        public static int CalculateProductOfThreeEntries(string[] input)
        {
            var entries = input.Select(int.Parse).ToArray();

            for (int i = 0; i < entries.Length; i++)
            {
                var first = entries[i];
                for (var j = i + 1; j < entries.Length; j++)
                {
                    var second = entries[j];
                    for (int k = j + 1; k < entries.Length; k++)
                    {
                        if (first + second + entries[k] != 2020)
                            continue;

                        return first * second * entries[k];
                    }
                }
            }

            throw new Exception("Unable to find entries summing to 2020");
        }
    }
}