namespace AdventOfCode.Day12;

public sealed class Day12(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day12";

    private readonly string[] _example =
    [
        "???.### 1,1,3",
        ".??..??...?##. 1,1,3",
        "?#?#?#?#?#?#?#? 1,3,1,6",
        "????.#...#... 4,1,1",
        "????.######..#####. 1,6,5",
        "?###???????? 3,2,1"
    ];

    [Fact]
    public void ExampleOne() => FindArrangements(_example).Should().Be(21);

    [Fact]
    public void PartOne() => WriteOutput(FindArrangements(Input));

    [Fact]
    public void ExampleTwo() => FindFixedArrangements(_example).Should().Be(525152);

    [Fact]
    public void PartTwo() => WriteOutput(FindFixedArrangements(Input));

    private static long FindFixedArrangements(string[] input)
    {
        var records = input.Select(i => i.Split(' '))
            .Select(x => (Line: string.Join('?', Enumerable.Repeat(x[0], 5)),
                Groups: string.Join(',', Enumerable.Repeat(x[1], 5)).Split(',').Select(y => int.Parse(y)).ToArray()))
            .ToArray();

        return CalculateCombinations(records);
    }

    private static long FindArrangements(string[] input)
    {
        var records = input.Select(i => i.Split(' '))
            .Select(i => (Line: i[0], Groups: i[1].Split(',').Select(x => int.Parse(x)).ToArray()))
            .ToArray();

        return CalculateCombinations(records);
    }

    private static long CalculateCombinations((string Line, int[] groups)[] records)
    {
        var combos = 0L;
        foreach (var (line, groups) in records)
        {
            var memo = new Dictionary<State, long>();
            combos += Count(line, groups, new State(0, 0, 0), memo);
        }

        return combos;

        static long Count(string line, int[] groups, State current, Dictionary<State, long> memo)
        {
            if (memo.TryGetValue(current, out var res))
                return res;
            char[] opts = ['.', '#'];
            if (current.Pos == line.Length)
            {
                if (current.Group == groups.Length && current.GroupPos == 0)
                    return 1;

                if (current.Group == groups.Length - 1 && current.GroupPos == groups[current.Group])
                    return 1;

                return 0;
            }
            var combos = 0L;
            foreach (var opt in opts)
            {
                if (line[current.Pos] != '?' && line[current.Pos] != opt)
                    continue;
                if (opt == '#')
                    combos += Count(line, groups, current.AddHash(), memo);
                if (opt == '.')
                {
                    var newState = current.AddDot(groups);
                    if (newState is not null)
                        combos += Count(line, groups, newState, memo);
                }
            }
            memo[current] = combos;
            return combos;
        }
    }

    private record State(int Pos, int Group, int GroupPos)
    {
        public State AddHash() => this with { Pos = Pos + 1, GroupPos = GroupPos + 1 };

        public State? AddDot(int[] groups)
        {
            if (GroupPos == 0)
                return this with { Pos = Pos + 1 };

            if (GroupPos > 0 && Group < groups.Length && GroupPos == groups[Group])
                return this with { Pos = Pos + 1, Group = Group + 1, GroupPos = 0 };

            return null;
        }
    }
}
