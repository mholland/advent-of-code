namespace AdventOfCode.Day17;

public sealed class Day17(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 17);

    private readonly string[] _example =
    [
        "Register A: 729",
        "Register B: 0",
        "Register C: 0",
        "",
        "Program: 0,1,5,4,3,0"
    ];
    
    [Fact]
    public void ExampleOne() => RunProgram(_example).Should().Be("4,6,3,5,6,3,5,2,1,0");

    [Fact]
    public async Task PartOne() => WriteOutput(RunProgram(await ReadInputLines()));

    private static string RunProgram(string[] input)
    {
        return Program.Parse(input).Run();
    }

    private class Program
    {
        private int A { get; set; }
        private int B { get; set; }
        private int C { get; set; }
        private int[] Instructions { get; }

        private List<int> Output { get; } = [];

        private int _ip;

        private Program(int a, int b, int c, int[] instructions)
        {
            A = a;
            B = b;
            C = c;
            Instructions = instructions;
        }

        public static Program Parse(string[] input)
        {
            var a = int.Parse(input[0].Split(' ')[^1]);
            var b = int.Parse(input[1].Split(' ')[^1]);
            var c = int.Parse(input[2].Split(' ')[^1]);
            
            var instructions = input[4].Split(' ')[1].Split(',').Select(int.Parse).ToArray();
            return new Program(a, b, c, instructions);
        }

        // Combo operands 0 through 3 represent literal values 0 through 3.
        // Combo operand 4 represents the value of register A.
        // Combo operand 5 represents the value of register B.
        // Combo operand 6 represents the value of register C.
        // Combo operand 7 is reserved and will not appear in valid programs.
        // The eight instructions are as follows:
        //
        // The adv instruction (opcode 0) performs division. The numerator is the value in the A register. The denominator is found by raising 2 to the power of the instruction's combo operand. (So, an operand of 2 would divide A by 4 (2^2); an operand of 5 would divide A by 2^B.) The result of the division operation is truncated to an integer and then written to the A register.
        //
        // The bxl instruction (opcode 1) calculates the bitwise XOR of register B and the instruction's literal operand, then stores the result in register B.
        //
        // The bst instruction (opcode 2) calculates the value of its combo operand modulo 8 (thereby keeping only its lowest 3 bits), then writes that value to the B register.
        //
        // The jnz instruction (opcode 3) does nothing if the A register is 0. However, if the A register is not zero, it jumps by setting the instruction pointer to the value of its literal operand; if this instruction jumps, the instruction pointer is not increased by 2 after this instruction.
        //
        // The bxc instruction (opcode 4) calculates the bitwise XOR of register B and register C, then stores the result in register B. (For legacy reasons, this instruction reads an operand but ignores it.)
        //
        // The out instruction (opcode 5) calculates the value of its combo operand modulo 8, then outputs that value. (If a program outputs multiple values, they are separated by commas.)
        //
        // The bdv instruction (opcode 6) works exactly like the adv instruction except that the result is stored in the B register. (The numerator is still read from the A register.)
        //
        // The cdv instruction (opcode 7) works exactly like the adv instruction except that the result is stored in the C register. (The numerator is still read from the A register.)
        public string Run()
        {
            var i = 0;
            while (_ip < Instructions.Length)
            {
                i += 1;
                var combo = Combo(Instructions[_ip + 1]);
                switch (Instructions[_ip])
                {
                    case 0:
                        A /= (int)Math.Pow(2, combo);
                        break;
                    case 1:
                        B ^= Instructions[_ip + 1];
                        break;
                    case 2:
                        B = combo % 8;
                        break;
                    case 3:
                        if (A == 0) break;
                        _ip = Instructions[_ip + 1];
                        continue;
                    case 4:
                        B ^= C;
                        break;
                    case 5:
                        Output.Add(combo % 8);
                        break;
                    case 6:
                        B = A / (int)Math.Pow(2, combo);
                        break;
                    case 7:
                        C = A / (int)Math.Pow(2, combo);
                        break;
                }
                _ip += 2;
            }
            return string.Join(",", Output);

            int Combo(int op) => op switch
            {
                >= 0 and <= 3 => op,
                4 => A,
                5 => B,
                6 => C,
                _ => throw new Exception("Invalid op")
            };
        }
    }
}
