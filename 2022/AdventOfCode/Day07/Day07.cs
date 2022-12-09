namespace AdventOfCode.Day07;

public sealed class Day07 : TestBase
{
    protected override string Day => "Day07";
    public Day07(ITestOutputHelper output)
        : base(output)
    {
    }

    private string[] Example = new[]
    {
        "$ cd /",
        "$ ls",
        "dir a",
        "14848514 b.txt",
        "8504156 c.dat",
        "dir d",
        "$ cd a",
        "$ ls",
        "dir e",
        "29116 f",
        "2557 g",
        "62596 h.lst",
        "$ cd e",
        "$ ls",
        "584 i",
        "$ cd ..",
        "$ cd ..",
        "$ cd d",
        "$ ls",
        "4060174 j",
        "8033020 d.log",
        "5626152 d.ext",
        "7214296 k"
    };

    [Fact]
    public void ExampleOne() => CalculateDirectorySize(Example).Should().Be(95437);

    [Fact]
    public void PartOne() => Output.WriteLine($"Size: {CalculateDirectorySize(Input)}");

    [Fact]
    public void ExampleTwo() => CalculateSmallestDeletableDirectorySize(Example).Should().Be(24933642);

    [Fact]
    public void PartTwo() => Output.WriteLine($"Size: {CalculateSmallestDeletableDirectorySize(Input)}");

    private int CalculateSmallestDeletableDirectorySize(string[] input)
    {
        var directories = ParseDirectorySizes(input);

        var unused = 70000000 - directories.Values.Max();
        var required = 30000000;

        return directories.Values.Where(v => unused + v >= required).Min();
    }

    private int CalculateDirectorySize(string[] input) =>
        ParseDirectorySizes(Input).Values.Where(s => s <= 100000).Sum();

    private Dictionary<string, int> ParseDirectorySizes(string[] input)
    {
        var pwd = new Stack<string>();
        var directories = new Dictionary<string, int>();
        foreach (var line in input)
        {
            if (line.StartsWith('$'))
            {
                var cmd = line[2..].Split(' ');
                if (cmd[0] == "ls")
                {
                    continue;
                }
                if (cmd[1] != "..")
                {
                    pwd.Push(cmd[1]);
                    var currentDirectory = Path.Combine(pwd.Reverse().ToArray());
                    if (!directories.ContainsKey(currentDirectory))
                        directories[currentDirectory] = 0;
                    continue;
                }

                pwd.Pop();
            }

            if (Char.IsDigit(line[0]))
            {
                var split = line.Split(' ');
                for (var i = 1; i <= pwd.Count(); i++)
                    directories[Path.Combine(pwd.Reverse().Take(i).ToArray())] += int.Parse(split[0]);
            }
        }

        return directories;
    }
}