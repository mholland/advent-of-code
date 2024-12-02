using System.Text.RegularExpressions;
using Xunit.Abstractions;

namespace EverybodyCodes.Day02;

public sealed class Day02(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day02";

    private readonly string[] _exampleOne = [
"WORDS:THE,OWE,MES,ROD,HER",
"",
"AWAKEN THE POWER ADORNED WITH THE FLAMES BRIGHT IRE"
    ];

    [Fact]
    public void ExampleOne() => CountRunes(_exampleOne).Should().Be(4);

    [Fact]
    public void PartOne() => WriteOutput(CountRunes(ReadFile("A.txt")));

    private readonly string[] _exampleTwo =
    [
        "WORDS:THE,OWE,MES,ROD,HER,QAQ",
        "",
        "AWAKEN THE POWE ADORNED WITH THE FLAMES BRIGHT IRE",
        "THE FLAME SHIELDED THE HEART OF THE KINGS",
        "POWE PO WER P OWE R",
        "THERE IS THE END",
        "QAQAQ",
    ];

    [Fact]
    public void ExampleTwo() => CountSymbols(_exampleTwo).Should().Be(42);

    [Fact]
    public void PartTwo() => WriteOutput(CountSymbols(ReadFile("B.txt")));

    private readonly string[] _exampleThree =
    [
        "WORDS:THE,OWE,MES,ROD,RODEO",
        "",
        "HELWORLT",
        "ENIGWDXL",
        "TRODEOAL",
    ];

    [Fact]
    public void ExampleThree() => CountScales(_exampleThree).Should().Be(10);

    [Fact]
    public void PartThree() => WriteOutput(CountScales(ReadFile("C.txt")));
    
    private static int CountScales(string[] input)
    {
        var words = input[0][6..].Split(',');
        var reversed = words.Select(w => new string(w.Reverse().ToArray())).ToArray();
        var longestWordLen = words.Max(x => x.Length);
        var horizontalScales = new List<string>();
        var verticalScales = new List<string>();
        var sentences = input[2..];
        var sentenceWidth = sentences.Max(x => x.Length);
        horizontalScales.AddRange(sentences.Select(x => x + x[..longestWordLen]));
        var seen = new HashSet<(int x, int y)>();

        for (var x = 0; x < sentenceWidth; x++)
        {
            var sentence = sentences
                .Aggregate(string.Empty, (current, t) => current + t[x]);
            verticalScales.Add(sentence);
        }
        
        for (var y = 0; y < horizontalScales.Count; y++)
        {
            var sentence = horizontalScales[y];
            foreach (var word in words.Union(reversed))
            {
                var matches = Regex.Matches(sentence, word)
                    .Union(Regex.Matches(sentence, word, RegexOptions.RightToLeft));
                foreach (var match in matches)
                {
                    if (match.Index >= sentenceWidth) continue;
                    for (var x = match.Index; x < match.Index + match.Length; x++)
                        seen.Add((x % sentenceWidth, y));
                }
            }
        }

        for (var x = 0; x < verticalScales.Count; x++)
        {
            var sentence = verticalScales[x];
            foreach (var word in words.Union(reversed))
            {
                var matches = Regex.Matches(sentence, word)
                    .Union(Regex.Matches(sentence, word, RegexOptions.RightToLeft));
                foreach (var match in matches)
                {
                    if (match.Index >= sentenceWidth) continue;
                    for (var y = match.Index; y < match.Index + match.Length; y++)
                        seen.Add((x, y));
                }
            }
        }
        
        return seen.Count;
    }

    private static int CountRunes(string[] input)
    {
        var words = input[0][6..].Split(',', StringSplitOptions.TrimEntries);

        return words.SelectMany(w => Regex.Matches(input[2], w)).Count();
    }

    private static int CountSymbols(string[] input)
    {
        var words = input[0][6..].Split(',');
        var reversed = words.Select(w => new string(w.Reverse().ToArray()));

        var sentences = input[2..];
        var total = 0;
        foreach (var sentence in sentences)
        {
            var symbols = new HashSet<int>();
            foreach (var word in words.Union(reversed))
            {
                var matches = Regex.Matches(sentence, word)
                    .Union(Regex.Matches(sentence, word, RegexOptions.RightToLeft));
                foreach (var match in matches)
                {
                    for (var i = match.Index; i < match.Index + match.Length; i++)
                        symbols.Add(i);
                }
            }

            total += symbols.Count;
        }
        return total;
    }
}