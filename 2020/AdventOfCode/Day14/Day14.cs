using Xunit;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using FluentAssertions;
using Xunit.Abstractions;
using System.Numerics;

namespace AdventOfCode.Day14
{
    public sealed class Day14Tests : TestBase
    {
        protected override string Day { get; } = "Day14";

        public Day14Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            SeaPortEmulator.ImportProgram(new[]
            {
                "mask = XXXXXXXXXXXXXXXXXXXXXXXXXXXXX1XXXX0X",
                "mem[8] = 11",
                "mem[7] = 101",
                "mem[8] = 0"
            })
            .RunProgram()
            .TotalMemoryValues()
            .Should()
            .Be(165);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($@"Total memory: {SeaPortEmulator
                    .ImportProgram(Input)
                    .RunProgram()
                    .TotalMemoryValues()}");

        [Fact]
        public void ExampleTwo() =>
            SeaPortEmulator.ImportProgram(new[]
            {
                "mask = 000000000000000000000000000000X1001X",
                "mem[42] = 100",
                "mask = 00000000000000000000000000000000X0XX",
                "mem[26] = 1"
            }, 2)
            .RunProgram()
            .TotalMemoryValues()
            .Should()
            .Be(208);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($@"Total memory: {SeaPortEmulator
                    .ImportProgram(Input, 2)
                    .RunProgram()
                    .TotalMemoryValues()}");

        private sealed class SeaPortEmulator
        {
            private readonly Instruction[] _instructions;
            private readonly int _version;

            private SeaPortEmulator(Instruction[] instructions, int version)
            {
                _instructions = instructions;
                _version = version;
            }

            public IDictionary<long, long> Memory { get; } = new Dictionary<long, long>();
            public string CurrentMask { get; private set; }

            public static SeaPortEmulator ImportProgram(string[] input, int version = 1)
            {

                var instructions = new List<Instruction>();
                foreach (var instruction in input)
                {
                    switch (instruction)
                    {
                        case var i when i.StartsWith("mem"):
                            instructions.Add(new SetMemoryInstruction(i));
                            break;
                        case var i when i.StartsWith("mask"):
                            instructions.Add(new SetMaskInstruction(i));
                            break;
                    }
                }

                return new SeaPortEmulator(instructions.ToArray(), version);
            }

            public SeaPortEmulator RunProgram()
            {
                foreach (var instruction in _instructions)
                {
                    switch (instruction)
                    {
                        case SetMaskInstruction inst:
                            ExecuteSetMaskInstruction(inst);
                            continue;
                        case SetMemoryInstruction inst:
                            ExecuteSetMemoryInstruction(inst);
                            continue;
                    }
                }

                return this;
            }

            private void ExecuteSetMemoryInstruction(SetMemoryInstruction instruction)
            {
                var memoryLocation = instruction.Address;
                var value = instruction.Value;
                var locations = new List<long>();

                if (_version == 1)
                {
                    value = ApplyMaskToValue(value);
                    locations.Add(memoryLocation);
                }

                if (_version == 2)
                    locations.AddRange(ApplyMaskToMemoryLocation(memoryLocation));

                foreach (var location in locations)
                {
                    Memory[location] = value;
                }
            }

            private long ApplyMaskToValue(long value)
            {
                for (var i = 0; i < CurrentMask.Length; i++)
                {
                    switch (CurrentMask[i])
                    {
                        case '0':
                            value &= ~(1L << i);
                            continue;
                        case '1':
                            value |= (1L << i);
                            continue;
                    }
                }

                return value;
            }

            private IEnumerable<long> ApplyMaskToMemoryLocation(long memoryLocation)
            {
                var floatingIndices = new List<int>();
                for (var i = 0; i < CurrentMask.Length; i++)
                {
                    switch (CurrentMask[i])
                    {
                        case '1':
                            memoryLocation |= (1L << i);
                            continue;
                        case 'X':
                            floatingIndices.Add(i);
                            continue;
                    }
                }

                return EnumerateIndices(memoryLocation, floatingIndices.ToArray());

                IEnumerable<long> EnumerateIndices(long value, int[] floatingIndices)
                {
                    if (floatingIndices.Length == 0)
                        return new[] { value };

                    var head = floatingIndices[0];
                    var tail = floatingIndices[1..];

                    return EnumerateIndices(value & ~(1L << head), tail)
                        .Concat(EnumerateIndices(value | (1L << head), tail));
                }
            }

            private void ExecuteSetMaskInstruction(SetMaskInstruction instruction) =>
                CurrentMask = new string(instruction.Mask.Reverse().ToArray());

            public long TotalMemoryValues() => Memory.Values.Sum();

            private abstract class Instruction
            {
            }

            private class SetMaskInstruction : Instruction
            {
                public string Mask { get; set; }

                public SetMaskInstruction(string instruction) =>
                    Mask = instruction.Split('=')[1].Trim();
            }

            private class SetMemoryInstruction : Instruction
            {
                public long Address { get; }
                public long Value { get; }

                public SetMemoryInstruction(string instruction)
                {
                    var match = Regex.Match(instruction, @"mem\[(\d+)\] = (\d+)");

                    Address = long.Parse(match.Groups[1].Value);
                    Value = long.Parse(match.Groups[2].Value);
                }
            }
        }
    }
}