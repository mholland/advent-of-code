using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day13
{
    public sealed class Day13Tests : TestBase
    {
        protected override string Day { get; } = "Day13";

        public Day13Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            CalculateBusChecksum(new[]
            {
                "939",
                "7,13,x,x,59,x,31,19"
            })
            .Should()
            .Be(295);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"First bus checksum: {CalculateBusChecksum(Input)}");

        public static IList<object[]> ContestExamples() =>
            new List<object[]>
            {
                new object[] { new[] {"", "7,13,x,x,59,x,31,19"}, 1068781 },
                new object[] { new[] {"", "17,x,13,19"}, 3417 },
                new object[] { new[] {"", "67,7,59,61"}, 754018 },
                new object[] { new[] {"", "67,x,7,59,61"}, 779210},
                new object[] { new[] {"", "67,7,x,59,61"}, 1261476 },
                new object[] { new[] {"", "1789,37,47,1889"}, 1202161486 }
            };

        [Theory]
        [MemberData(nameof(ContestExamples))]
        public void ExampleTwo(string[] input, long expectedValue) =>
            FindContestSolution(input)
            .Should()
            .Be(expectedValue);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Contest answer: {FindContestSolution(Input)}");

        private long FindContestSolution(string[] input)
        {
            var buses =
                input[1]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select((b, i) => (Order: i, Id: b))
                    .Where(b => b.Id != "x")
                    .Select(b => (b.Order, Id: long.Parse(b.Id)))
                    .ToArray();

            // t + i ≡ 0 mod m (m being each bus id)
            // t ≡ -i mod m
            // t ≡ b-i mod m
            // crt solves t ≡ a_i mod b_i
            // https://brilliant.org/wiki/chinese-remainder-theorem/
            var N = buses.Aggregate(1L, (acc, cur) => acc * cur.Id);
            long sum = 0;
            for (var i = 0; i < buses.Length; i++)
            {
                var (order, id) = buses[i];
                var a = id - order;
                var y = N / id;
                var z = Inverse(y, id) % id;

                sum += a * y * z;
            }

            return sum % N;
        }

        private long Inverse(long a, long m)
        {
            return Power(a, m - 2, m);

            long Power(long x, long y, long m)
            {
                if (y == 0)
                    return 1;

                var p = Power(x, y / 2, m) % m;
                p = (p * p) % m;

                if (y % 2 == 0)
                    return p;

                return (x * p) % m;
            }
        }

        private int CalculateBusChecksum(string[] input)
        {
            var earliestTimestamp = int.Parse(input[0]);
            var firstBus =
                input[1]
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Where(bus => bus != "x")
                    .Select(int.Parse)
                    .Select(b => (Id: b, Diff: FindTimeToWaitForBus(b, earliestTimestamp)))
                    .OrderBy(b => b.Diff)
                    .First();

            return firstBus.Diff * firstBus.Id;

            int FindTimeToWaitForBus(int cadence, int earliest)
            {
                var timestamp = 0;
                while (timestamp < earliest)
                {
                    timestamp += cadence;
                }

                return timestamp - earliest;
            }
        }
    }
}