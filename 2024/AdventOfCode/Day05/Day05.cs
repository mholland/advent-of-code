using Rules = (int Required, int Page)[];

namespace AdventOfCode.Day05;

public class Day05(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 05);

    private readonly string[] _example =
    [
        "47|53",
        "97|13",
        "97|61",
        "97|47",
        "75|29",
        "61|13",
        "75|53",
        "29|13",
        "97|29",
        "53|29",
        "61|53",
        "97|53",
        "61|29",
        "47|13",
        "75|47",
        "97|75",
        "47|61",
        "75|61",
        "47|29",
        "75|13",
        "53|13",
        "",
        "75,47,61,53,29",
        "97,61,53,29,13",
        "75,29,13",
        "75,97,47,61,53",
        "61,13,29",
        "97,13,75,29,47"
    ];

    [Fact]
    public void ExampleOne() => CheckCorrectlyOrderedUpdates(_example).Should().Be(143);

    [Fact]
    public async Task PartOne() => WriteOutput(CheckCorrectlyOrderedUpdates(await ReadInputLines()));

    [Fact]
    public void ExampleTwo() => CheckCorrectedUpdates(_example).Should().Be(123);
    
    [Fact]
    public async Task PartTwo() => WriteOutput(CheckCorrectedUpdates(await ReadInputLines()));

    private static int CheckCorrectlyOrderedUpdates(string[] input)
    {
        var (rules, updates) = ParseInput(input);

        return updates.Where(u => IsValid(u, rules)).Sum(update => update[update.Length / 2]);
    }

    private static int CheckCorrectedUpdates(string[] input)
    {
        var (rules, updates) = ParseInput(input);
        
        var invalid = updates.Where(u => !IsValid(u, rules));

        var sum = 0;
        foreach (var update in invalid)
        {
            var relevantRules = rules.Where(r => update.Contains(r.Required) && update.Contains(r.Page)).ToArray();
            var requirementsByPage = relevantRules.GroupBy(r => r.Page)
                .ToDictionary(r => r.Key, r => r.Select(rr => rr.Required));
            List<int> ordered = [];
            while (ordered.Count <= update.Length / 2)
            {
                // Pick the next page that hasn't already been selected
                // and either
                //      has no requirements, in the case of the first page
                //      or all requirements have already been placed in ordered.
                var next = update.First(
                    u => !ordered.Contains(u) && (!requirementsByPage.TryGetValue(u, out var rr) ||
                                                  rr.All(r => ordered.Contains(r))));
                ordered.Add(next);
            }
            sum += ordered[update.Length / 2];
        }
        
        return sum;
    }
    
    private static bool IsValid(int[] update, Rules rules)
    {
        var relevantRules = rules.Where(r => update.Contains(r.Required) && update.Contains(r.Page)).ToArray();
        List<int> printed = [];
        foreach (var page in update)
        {
            var requirements = relevantRules.Where(r => r.Page == page);
            if (!requirements.All(r => printed.Contains(r.Required)))
                return false;
            printed.Add(page);
        }

        return true;
    }

    private static (Rules Rules, int[][] Updates) ParseInput(string[] input)
    {
        var sep = Array.IndexOf(input, "");
        var rules = input[..sep]
            .Select(s => s.Split('|'))
            .Select(s => (Required: int.Parse(s[0]), Page: int.Parse(s[1])))
            .ToArray();
        var updates = input[(sep + 1)..]
            .Select(u => u.Split(',').Select(int.Parse).ToArray())
            .ToArray();
        return (rules, updates);
    }
}