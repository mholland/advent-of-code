using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day09
{
    public sealed class Day09Tests : TestBase
    {
        protected override string Day { get; } = "Day09";
        public Day09Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            FindInvalidNumber(new[]
            {
                "35",
                "20",
                "15",
                "25",
                "47",
                "40",
                "62",
                "55",
                "65",
                "95",
                "102",
                "117",
                "150",
                "182",
                "127",
                "219",
                "299",
                "277",
                "309",
                "576"
            }, 5)
            .Should()
            .Be(127);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"First number not summable: {FindInvalidNumber(Input, 25)}");

        [Fact]
        public void ExampleTwo()
        {
            FindEncryptionWeakness(new[]
            {
                "35",
                "20",
                "15",
                "25",
                "47",
                "40",
                "62",
                "55",
                "65",
                "95",
                "102",
                "117",
                "150",
                "182",
                "127",
                "219",
                "299",
                "277",
                "309",
                "576"
            }, 5)
            .Should()
            .Be(62);
        }

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Encryption weakness: {FindEncryptionWeakness(Input, 25)}");

        public static long FindEncryptionWeakness(string[] input, int preambleSize)
        {
            var invalidNumber = FindInvalidNumber(input, preambleSize);
            var digits = input.Select(long.Parse).ToArray();

            for (var i = 0; i < digits.Length; i++)
            {
                for (var j = i + 1; j < digits.Length; j++)
                {
                    var range = digits[i..(j + 1)];
                    var sum = range.Sum();
                    if (sum == invalidNumber)
                        return range.Min() + range.Max();

                    if (sum > invalidNumber)
                        break;
                }
            }

            throw new Exception("Unable to find encryption weakness.");
        }

        public static long FindInvalidNumber(string[] input, int preambleSize)
        {
            var digits = input.Select(long.Parse).ToArray();
            var offset = preambleSize;
            while (offset < digits.Length)
            {
                var canSum = CanSum(digits, offset, preambleSize);
                if (!canSum)
                    return digits[offset];

                offset++;
            }

            throw new Exception("Unable to find invalid number.");

            static bool CanSum(long[] digits, int offset, int preambleSize)
            {
                for (var i = offset - preambleSize; i < offset; i++)
                {
                    for (var j = i + 1; j < offset; j++)
                    {
                        if (digits[i] + digits[j] == digits[offset])
                            return true;
                    }
                }

                return false;
            }
        }
    }
}