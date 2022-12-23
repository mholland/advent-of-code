namespace AdventOfCode.Day10;

public sealed class Day10 : TestBase
{
    private readonly ITestOutputHelper _output;
    private readonly string[] _example; 

    protected override string Day => "Day10";
    public Day10(ITestOutputHelper output)
        : base(output)
    {
        _output = output;
        _example = ReadFile("input.example.txt");
    }

    [Fact]
    public void ExampleOne() => GenerateImage(_example).SignalStrength.Should().Be(13140);

    [Fact]
    public void PartOne() => _output.WriteLine($"Signal strength: {GenerateImage(Input).SignalStrength}");

    [Fact]
    public void ExampleTwo() => GenerateImage(_example).Image.Should().Be(
@"##  ##  ##  ##  ##  ##  ##  ##  ##  ##  
###   ###   ###   ###   ###   ###   ### 
####    ####    ####    ####    ####    
#####     #####     #####     #####     
######      ######      ######      ####
#######       #######       #######     "
    );

    [Fact]
    public void PartTwo() => _output.WriteLine($"Image:{Environment.NewLine}{GenerateImage(Input).Image}");

    private static (int SignalStrength, string Image) GenerateImage(string[] input)
    {
        var instructions = new Queue<Instruction>(input.Length);
        var x = 1;
        var signalStrength = 0;
        var cycle = 0;
        var image = string.Empty;
        foreach (var inst in input)
        {
            var split = inst.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            instructions.Enqueue(new Noop());
            if (split.Length == 1)
                continue;
            instructions.Enqueue(new Add(int.Parse(split[1])));
        }
        while (instructions.TryDequeue(out var inst))
        {
            if (++cycle % 40 == 20)
                signalStrength += cycle * x;
            if (cycle - 1 != 0 && (cycle - 1) % 40 == 0)
                image += Environment.NewLine;
            image += Math.Abs(((cycle - 1) % 40) - x) <= 1 ? '#' : ' ';
            x = inst.Execute(x);
        }

        return (signalStrength, image);
    }

    private abstract class Instruction
    {
        public abstract int Execute(int register);
    }

    private sealed class Noop : Instruction
    {
        public override int Execute(int register) => register;
    }

    private sealed class Add : Instruction
    {
        private readonly int _value;

        public Add(int value) => _value = value;

        public override int Execute(int register) => register += _value;
    }
}