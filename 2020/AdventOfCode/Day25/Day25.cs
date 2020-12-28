using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day25
{
    public sealed class Day25Tests : TestBase
    {
        protected override string Day { get; } = "Day25";

        public Day25Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            FindEncryptionKey(new[]
            {
                "5764801",
                "17807724"
            })
            .Should()
            .Be(14897079);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Encryption key: {FindEncryptionKey(Input)}");

        private double FindEncryptionKey(string[] input)
        {
            var keys = input
                .Select(int.Parse)
                .Select(pk => (PK: pk, LoopSize: CalculateLoopSize(pk)))
                .ToArray();

            var loopSize = keys[0].LoopSize;
            var subject = keys[1].PK;
            var value = 1L;
            for (var i = 0; i < loopSize; i++)
                value = Transform(value, subject);

            return value;

            long CalculateLoopSize(int publicKey)
            {
                var subjectNumber = 7;
                var value = 1L;
                var loopSize = 0;
                while (value != publicKey)
                {
                    loopSize++;
                    value = Transform(value, subjectNumber);
                }

                return loopSize;
            }

            long Transform(long value, int subjectNumber)
            {
                value *= subjectNumber;
                return value % 20201227;
            }
        }
    }
}