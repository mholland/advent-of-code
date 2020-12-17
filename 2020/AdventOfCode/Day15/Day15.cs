using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day15
{
    public sealed class Day15Tests
    {
        private readonly ITestOutputHelper _output;

        public Day15Tests(ITestOutputHelper output) => _output = output;

        // "9,19,1,6,0,5,4"
        [Theory]
        [InlineData("0,3,6", 436)]
        [InlineData("1,3,2", 1)]
        [InlineData("2,1,3", 10)]
        [InlineData("1,2,3", 27)]
        [InlineData("2,3,1", 78)]
        [InlineData("3,2,1", 438)]
        [InlineData("3,1,2", 1836)]
        public void Examples(string input, int expectedValue) =>
            PlayGame(input, 2020)
                .Should()
                .Be(expectedValue);

        [Fact]
        public void PartOne() =>
            _output.WriteLine($"2020th turn: {PlayGame("9,19,1,6,0,5,4", 2020)}");

        [Fact]
        public void PartTwo() =>
            _output.WriteLine($"30000000th turn: {PlayGame("9,19,1,6,0,5,4", 30_000_000)}");

        public long PlayGame(string input, int turns)
        {
            var previous = new Dictionary<int, int>(turns);
            var values = input.Split(',').Select(int.Parse).ToList();

            for (var i = 0; i < values.Count() - 1; i++)
            {
                previous[values[i]] = i;
            }
            var last = values[^1];

            for (var i = values.Count() - 1; i < turns - 1; i++)
            {
                var hasPrevious = previous.TryGetValue(last, out var prior);
                previous[last] = i;
                if (!hasPrevious)
                {
                    last = 0;
                    continue;
                }

                last = i - prior;
            }

            return last;
        }
    }
}