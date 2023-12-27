using System.Collections.Immutable;
using Rating = System.Collections.Generic.Dictionary<string, int>;
using Range = (int Start, int End);

namespace AdventOfCode.Day19;

public sealed class Day19(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day19";

    private readonly string _example =
"""
px{a<2006:qkq,m>2090:A,rfg}
pv{a>1716:R,A}
lnx{m>1548:A,A}
rfg{s<537:gd,x>2440:R,A}
qs{s>3448:A,lnx}
qkq{x<1416:A,crn}
crn{x>2662:A,R}
in{s<1351:px,qqz}
qqz{s>2770:qs,m<1801:hdj,R}
gd{a>3333:R,R}
hdj{m>838:A,pv}

{x=787,m=2655,a=1222,s=2876}
{x=1679,m=44,a=2067,s=496}
{x=2036,m=264,a=79,s=2244}
{x=2461,m=1339,a=466,s=291}
{x=2127,m=1623,a=2188,s=1013}
""";

    [Fact]
    public void ExampleOne() => SumAcceptedParts(_example).Should().Be(19114);

    [Fact]
    public void ExampleTwo() => CountCombinations(_example).Should().Be(167409079868000);

    [Fact]
    public void PartOne() => WriteOutput(SumAcceptedParts(ReadAll("input.txt")));

    [Fact]
    public void PartTwo() => WriteOutput(CountCombinations(ReadAll("input.txt")).ToString());

    private static long CountCombinations(string input)
    {
        var split = input.Split(Environment.NewLine + Environment.NewLine);
        var workflows = ParseWorkflows(split[0]);
        var acceptedWorkFlows = new List<ImmutableList<WorkflowPart>>();
        var queue = new Queue<(WorkflowResult Result, ImmutableDictionary<string, Range> Ranges)>();
        var initialRanges = ImmutableDictionary.CreateRange(
            [
                KeyValuePair.Create("x", (1, 4000)),
                KeyValuePair.Create("m", (1, 4000)),
                KeyValuePair.Create("a", (1, 4000)),
                KeyValuePair.Create("s", (1, 4000))
            ]
        );
        queue.Enqueue((new LabelResult("in"), initialRanges));
        var combos = 0L;
        while (queue.TryDequeue(out var current))
        {
            if (current.Result is TerminalResult { Accepted: true } t)
            {
                combos += current.Ranges.Values
                    .Aggregate(1L, (acc, cur) => acc *= cur.End - cur.Start + 1);
                continue;
            }

            if (current.Result is not LabelResult l)
                continue;

            var newWorkflow = workflows[l.Label];
            var ranges = current.Ranges;
            foreach (var part in newWorkflow.Parts)
            {
                if (part is Label lp)
                    queue.Enqueue((new LabelResult(lp.Lbl), ranges));
                if (part is LessThan lt)
                {
                    var cur = ranges[lt.Variable];
                    var ltIntersection = Intersection(cur, (1, lt.Value - 1));
                    queue.Enqueue((lt.Result, ranges.SetItem(lt.Variable, ltIntersection)));
                    var elseIntersection = Intersection(cur, (lt.Value, 4000));
                    ranges = ranges.SetItem(lt.Variable, elseIntersection);
                }
                if (part is GreaterThan gt)
                {
                    var cur = ranges[gt.Variable];
                    var gtIntersection = Intersection(cur, (gt.Value + 1, 4000));
                    queue.Enqueue((gt.Result, ranges.SetItem(gt.Variable, gtIntersection)));
                    var elseIntersection = Intersection(cur, (1, gt.Value));
                    ranges = ranges.SetItem(gt.Variable, elseIntersection);
                }
                if (part is Terminal term)
                    queue.Enqueue((term.Result == "A" 
                        ? TerminalResult.Accept() 
                        : TerminalResult.Reject(), ranges));
            }

        }

        return combos;

        static Range Intersection(Range one, Range two) =>
            (Math.Max(one.Start, two.Start), Math.Min(one.End, two.End));
    }

    private static int SumAcceptedParts(string input)
    {
        var split = input.Split(Environment.NewLine + Environment.NewLine);
        var workflows = ParseWorkflows(split[0]);
        var ratings = ParseRatings(split[1]);

        var currentWorkflow = workflows["in"];
        var accepted = new List<Rating>();
        foreach (var rating in ratings)
        {
            while (true)
            {
                var result = currentWorkflow.Evaluate(rating);
                if (result is TerminalResult r)
                {
                    if (r.Accepted)
                        accepted.Add(rating);
                    break;
                }
                if (result is LabelResult w)
                    currentWorkflow = workflows[w.Label];
            }
            currentWorkflow = workflows["in"];
        }

        return accepted.Sum(r => r.Sum());
    }

    private static IDictionary<string, Workflow> ParseWorkflows(string v)
    {
        var result = new Dictionary<string, Workflow>();
        var split = v.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var w in split)
        {
            var endOfLabel = w.IndexOf('{');
            var label = w[0..endOfLabel];
            var workflow = w[endOfLabel..];
            var parsed = workflow[1..^1].Split(',')
                .Select(WorkflowPart.Parse)
                .ToArray();
            result[label] = new Workflow(parsed);
        }

        return result;
    }

    private static Rating[] ParseRatings(string split) =>
        split
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(r => r[1..^1])
            .Select(r => r.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(p => p.Select(x => x.Split('=')))
            .Select(p => p.Select(x => (Cat: x[0], Val: int.Parse(x[1]))))
            .Select(r => r.ToDictionary(r => r.Cat, r => r.Val))
            .ToArray();
}

public class Workflow(WorkflowPart[] parts)
{
    public WorkflowPart[] Parts { get; } = parts;

    public WorkflowResult Evaluate(Rating rating)
    {
        foreach (var part in Parts)
        {
            var r = part.Evaluate(rating);
            if (r is LabelResult or TerminalResult)
                return r;
        }

        throw new Exception("No result from Workflow evaluation.");
    }
}

public abstract class WorkflowPart
{
    public static WorkflowPart Parse(string val)
    {
        if (val is "R" or "A")
            return new Terminal(val);
        if (!val.Contains('<') && !val.Contains('>'))
            return new Label(val);
        return ParseInequality(val, val.Contains('<') ? '<' : '>');
    }

    private static WorkflowPart ParseInequality(string val, char relationship)
    {
        var parts = val.Split(relationship);
        var variable = parts[0];
        var spl = parts[1].Split(':');
        var value = int.Parse(spl[0]);
        var branch = spl[1];
        var result = GetResult(branch);

        return relationship == '<'
            ? new LessThan(variable, value, result)
            : new GreaterThan(variable, value, result);

        static WorkflowResult GetResult(string v)
        {
            if (v == "A") return TerminalResult.Accept();
            if (v == "R") return TerminalResult.Reject();

            return new LabelResult(v);
        }
    }

    public abstract WorkflowResult Evaluate(Rating value);
}

public sealed class Terminal(string result) : WorkflowPart
{
    public string Result { get; } = result;

    public override WorkflowResult Evaluate(Rating _) =>
        Result == "A" ? TerminalResult.Accept() : TerminalResult.Reject();
}

public sealed class LessThan(string variable, int value, WorkflowResult result) : WorkflowPart
{
    public string Variable { get; } = variable;
    public int Value { get; } = value;
    public WorkflowResult Result { get; } = result;

    public override WorkflowResult Evaluate(Rating rating) =>
        rating[Variable] < Value ? Result : NoopResult.Instance;
}

public sealed class GreaterThan(string variable, int value, WorkflowResult result) : WorkflowPart
{
    public string Variable { get; } = variable;
    public int Value { get; } = value;
    public WorkflowResult Result { get; } = result;

    public override WorkflowResult Evaluate(Rating rating) =>
        rating[Variable] > Value ? Result : NoopResult.Instance;
}

public class NoopResult : WorkflowResult
{
    public static NoopResult Instance { get; } = new();
}

public sealed class Label(string label) : WorkflowPart
{
    public string Lbl { get; } = label;

    public override WorkflowResult Evaluate(Rating _) =>
        new LabelResult(Lbl);
}

public class WorkflowResult { }
public class LabelResult(string label) : WorkflowResult
{
    public string Label { get; set; } = label;
}

public class TerminalResult : WorkflowResult
{
    public bool Accepted { get; }

    private TerminalResult(bool accepted) => Accepted = accepted;

    public static TerminalResult Accept() => new(true);
    public static TerminalResult Reject() => new(false);
}

public static class RatingExtensions
{
    public static int Sum(this Rating rating) =>
        rating["x"] + rating["m"] + rating["a"] + rating["s"];
}