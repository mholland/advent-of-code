using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day08
{
    public sealed class Day08Tests : TestBase
    {
        protected override string Day { get; } = "Day08";
        public Day08Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            ConsoleVM
                .ImportProgram(new[]
                {
                    "nop +0",
                    "acc +1",
                    "jmp +4",
                    "acc +3",
                    "jmp -3",
                    "acc -99",
                    "acc +1",
                    "jmp -4",
                    "acc +6"
                })
                .RunProgram()
                .Accumulator
                .Should()
                .Be(5);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Accumulator without repetition: {ConsoleVM.ImportProgram(Input).RunProgram().Accumulator}");

        [Fact]
        public void ExampleTwo() =>
            DebugProgram(new[]
            {
                "nop +0",
                "acc +1",
                "jmp +4",
                "acc +3",
                "jmp -3",
                "acc -99",
                "acc +1",
                "jmp -4",
                "acc +6"
            })
            .Should()
            .Be(8);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Accumulator after debugging: {DebugProgram(Input)}");

        public int DebugProgram(string[] input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                var program = ConsoleVM.ParseInput(input);
                if (program[i].Instruction != "nop" && program[i].Instruction != "jmp")
                    continue;

                program[i] = SwapCmd(program[i]);
                var result = ConsoleVM
                    .ImportProgram(program)
                    .RunProgram();

                if (result.Status == ConsoleVM.HaltStatus.InfiniteLoopDetected)
                    continue;
                
                return result.Accumulator;
            }

            throw new Exception("Unable to fix program.");

            (string, int) SwapCmd((string, int) instruction) =>
                instruction switch
                {
                    ("jmp", int v) => ("nop", v),
                    ("nop", int v) => ("jmp", v),
                    var i => i
                };
        }

        public sealed class ConsoleVM
        {
            private readonly (string Instruction, int Value)[] _instructions;
            private readonly ISet<int> _executedInstructions = new HashSet<int>();
            private int _instructionPointer = 0;

            private ConsoleVM((string, int)[] instructions) => _instructions = instructions;

            public static ConsoleVM ImportProgram(string[] input) =>
                new ConsoleVM(ParseInput(input));

            public static (string Instruction, int Value)[] ParseInput(string[] input) =>
                input
                    .Select(i => i.Split(' '))
                    .Select(s => (s[0], int.Parse(s[1])))
                    .ToArray();

            public static ConsoleVM ImportProgram((string, int)[] instructions) =>
                new ConsoleVM(instructions);

            public int Accumulator { get; private set; } = 0;

            public (HaltStatus Status, int Accumulator) RunProgram()
            {
                while (!_executedInstructions.Contains(_instructionPointer))
                {
                    if (_instructionPointer >= _instructions.Length)
                        return (HaltStatus.Ended, Accumulator);

                    var instruction = _instructions[_instructionPointer];
                    _executedInstructions.Add(_instructionPointer);
                    switch (instruction)
                    {
                        case ("jmp", _):
                            _instructionPointer += instruction.Value;
                            continue;
                        case ("acc", _):
                            Accumulator += instruction.Value;
                            break;
                    }
                    _instructionPointer++;
                }

                return (HaltStatus.InfiniteLoopDetected, Accumulator);
            }

            public enum HaltStatus
            {
                InfiniteLoopDetected,
                Ended
            }
        }
    }
}