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

    private readonly string[] _exampleTwo =
    [
        "Register A: 2024",
        "Register B: 0",
        "Register C: 0",
        "",
        "Program: 0,3,5,4,3,0"
    ];

    [Fact]
    public void ExampleTwo() => FindCopyARegister(_exampleTwo).Should().Be(117440);

    // 0: B = A % 8
    // 1: B = B ^ 5
    // 2: C = A / B
    // 3: B = B ^ C
    // 4: B = B ^ 6
    // 5: A = A / 2**3
    // 6: OUT += B % 8
    // 7: JNZ A 0
    [Fact]
    public async Task DecompileProgram() => WriteOutput(Program.Parse(await ReadInputLines()).ToString());

    [Fact]
    public async Task PartTwo() => WriteOutput(FindCopyARegister(await ReadInputLines()));

    [Fact]
    public async Task Test() => Program.Parse(await ReadInputLines()).OverrideRegisterA(105734774294938).Run();

    private static long FindCopyARegister(string[] input)
    {
        var program = Program.Parse(input);
        var queue = new Queue<long>([0]);
        var finalOutput = string.Join(",", program.Instructions);
        while (queue.TryDequeue(out var current))
        {
            for (var i = 0; i < 8; i++)
            {
                var a = current + i;
                var output = program
                    .Reset()
                    .OverrideRegisterA(a)
                    .Run();
                var formattedOutput = string.Join(",", output);
                var subset = string.Join(",", program.Instructions[^output.Length..]);
                if (subset != formattedOutput)
                    continue;
                if (formattedOutput == finalOutput)
                    return a;

                queue.Enqueue(a * 8);
            }
        }

        return -1;
    }

    private static string RunProgram(string[] input) =>
        string.Join(",", Program.Parse(input).Run());

    private class Program
    {
        private long A { get; set; }
        private long B { get; set; }
        private long C { get; set; }
        public int[] Instructions { get; }

        private List<long> Output { get; } = [];

        private int _ip;

        private Program(long a, long b, long c, int[] instructions)
        {
            A = a;
            B = b;
            C = c;
            Instructions = instructions;
        }

        public Program OverrideRegisterA(long a)
        {
            A = a;
            return this;
        }

        public static Program Parse(string[] input)
        {
            var a = long.Parse(input[0].Split(' ')[^1]);
            var b = long.Parse(input[1].Split(' ')[^1]);
            var c = long.Parse(input[2].Split(' ')[^1]);

            var instructions = input[4].Split(' ')[1].Split(',').Select(int.Parse).ToArray();
            return new Program(a, b, c, instructions);
        }

        public long[] Run()
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

            return [.. Output];

            long Combo(int op) => op switch
            {
                >= 0 and <= 3 => op,
                4 => A,
                5 => B,
                6 => C,
                _ => throw new Exception("Invalid op")
            };
        }

        public Program Reset()
        {
            _ip = 0;
            Output.Clear();
            A = 0;
            B = 0;
            C = 0;
            return this;
        }

        public override string ToString()
        {
            var result = "\n";
            var chunked = Instructions.Chunk(2).ToArray();
            for (var i = 0; i < chunked.Length; i++)
            {
                result += $"{i}: " + chunked[i] switch
                {
                    [0, var op] => "A = A / 2**" + Combo(op),
                    [1, var op] => $"B = B ^ {op}",
                    [2, var op] => $"B = {Combo(op)} % 8",
                    [3, var op] => $"JNZ A {op}",
                    [4, _] => "B = B ^ C",
                    [5, var op] => $"OUT += {Combo(op)} % 8",
                    [6, var op] => "B = A / " + Combo(op),
                    [7, var op] => "C = A / " + Combo(op),
                    _ => ""
                };
                result += "\n";
            }

            return result;

            static string Combo(int op) =>
                op switch
                {
                    >= 0 and <= 3 => op.ToString(),
                    4 => "A",
                    5 => "B",
                    6 => "C",
                    _ => ""
                };
        }
    }
}