using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day10
{
    public sealed class Day10Tests : TestBase
    {
        protected override string Day { get; } = "Day10";

        public Day10Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOneChecksum() =>
            CalculateJoltageChecksum(new[]
            {
                "16",
                "10",
                "15",
                "5",
                "1",
                "11",
                "7",
                "19",
                "6",
                "12",
                "4"
            })
            .Should()
            .Be(35);

        [Fact]
        public void ExampleTwoChecksum() =>
            CalculateJoltageChecksum(new[]
            {
                "28",
                "33",
                "18",
                "42",
                "31",
                "14",
                "46",
                "20",
                "48",
                "47",
                "24",
                "23",
                "49",
                "45",
                "19",
                "38",
                "39",
                "11",
                "1",
                "32",
                "25",
                "35",
                "8",
                "17",
                "7",
                "9",
                "4",
                "2",
                "34",
                "10",
                "3"
            })
            .Should()
            .Be(220);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Adapter checksum: {CalculateJoltageChecksum(Input)}");

        [Fact]
        public void ExampleOneCombinations() =>
            CountCombinations(new[]
            {
                "16",
                "10",
                "15",
                "5",
                "1",
                "11",
                "7",
                "19",
                "6",
                "12",
                "4"
            })
            .Should()
            .Be(8);

        [Fact]
        public void ExampleTwoCombinations() =>
            CountCombinations(new[]
            {
                "28",
                "33",
                "18",
                "42",
                "31",
                "14",
                "46",
                "20",
                "48",
                "47",
                "24",
                "23",
                "49",
                "45",
                "19",
                "38",
                "39",
                "11",
                "1",
                "32",
                "25",
                "35",
                "8",
                "17",
                "7",
                "9",
                "4",
                "2",
                "34",
                "10",
                "3"
            })
            .Should()
            .Be(19208);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Adapter Combinations: {CountCombinations(Input)}");

        private static int CalculateJoltageChecksum(string[] input)
        {
            var jolts = input.Select(int.Parse).OrderBy(x => x).ToArray();
            var differences = new Dictionary<int, int>
            {
                [1] = 0,
                [2] = 0,
                [3] = 1
            };
            differences[jolts[0]]++;

            for (var i = 1; i < jolts.Length; i++)
            {
                var diff = jolts[i] - jolts[i - 1];
                differences[diff]++;
            }

            return differences[1] * differences[3];
        }

        private static long CountCombinations(string[] input)
        {
            var jolts = input.Select(int.Parse).OrderBy(x => x).ToArray();

            return GroupConsecutive(jolts)
                .Aggregate(1L, (agg, next) =>
                    agg * next switch
                    {
                        { Count: 3 } => 2,
                        { Count: 4 } => 4,
                        { Count: 5 } => 7,
                        _ => 1
                    });

            List<List<int>> GroupConsecutive(int[] jolts)
            {
                var result = new List<List<int>>();
                var sub = new List<int>() { 0 };

                for (var i = 0; i < jolts.Length; i++)
                {
                    if (jolts[i] == sub.Last() + 1)
                    {
                        sub.Add(jolts[i]);
                        continue;
                    }

                    if (sub.Count() > 1)
                        result.Add(sub);
                    sub = new List<int>{ jolts[i] };
                }
                
                result.Add(sub);
                return result;
            }
        }
    }
}