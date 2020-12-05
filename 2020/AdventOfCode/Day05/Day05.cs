using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day05
{
    public sealed class Day05Tests : TestBase
    {
        protected override string Day => "Day05";

        public Day05Tests(ITestOutputHelper output) 
            : base(output)
        {
        }

        [Theory]
        [MemberData(nameof(Passes))]
        public void SeatIdIsCalculatedCorrectly(string[] input, int expectedId) =>
            Day05
                .FindMaxSeatId(input)
                .Should()
                .Be(expectedId);

        [Fact]
        public void MaxSeatIdIsCalculatedCorrectly() =>
            Day05.FindMaxSeatId(new[]
            {
                "FBFBBFFRLR",
                "BFFFBBFRRR",
                "FFFBBBFRRR",
                "BBFFBBFRLL"
            })
            .Should()
            .Be(820);

        public static IEnumerable<object[]> Passes =
            new List<object[]>
            {
                new object[] {new[] {"FBFBBFFRLR"}, 357},
                new object[] {new[] {"BFFFBBFRRR"}, 567},
                new object[] {new[] {"FFFBBBFRRR"}, 119},
                new object[] {new[] {"BBFFBBFRLL"}, 820}
            };

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Max seat id: {Day05.FindMaxSeatId(Input)}");

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"My seat id: {Day05.FindMySeatId(Input)}");
    }

    public static class Day05
    {
        public static int FindMaxSeatId(string[] input) =>
            CalculateSeatIds(input).Max();

        public static int FindMySeatId(string[] input)
        {
            var seatIds = CalculateSeatIds(input).OrderBy(s => s).ToArray();
            for (var i = 1; i < seatIds.Length; i++) 
            {
                if (seatIds[i] - seatIds[i - 1] > 1)
                    return seatIds[i] - 1;
            }

            throw new Exception("Unable to find seat id");
        }

        public static IEnumerable<int> CalculateSeatIds(string[] input)
        {
            return input
                .Select(CalculateSeatId);

            int CalculateSeatId(string pass)
            {
                var rows = Enumerable.Range(0, 128);
                var cols = Enumerable.Range(0, 8);

                foreach (var half in pass.Substring(0, 7))
                {
                    rows = half switch
                    {
                        'F' => rows.Take(rows.Count() / 2),
                        'B' => rows.Skip(rows.Count() / 2),
                        _ => rows
                    };
                }

                foreach (var half in pass.Substring(7))
                {
                    cols = half switch
                    {
                        'L' => cols.Take(cols.Count() / 2),
                        'R' => cols.Skip(cols.Count() / 2),
                        _ => cols
                    };
                }

                return (rows.First() * 8) + cols.First();
            }
        }
    }
}