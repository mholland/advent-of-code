using System.Diagnostics;

namespace AdventOfCode.Day02;

public sealed class Day02(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 02);

    private readonly string _example =
        "11-22,95-115,998-1012,1188511880-1188511890,222220-222224,1698522-1698528,446443-446449,38593856-38593862,565653-565659,824824821-824824827,2121212118-2121212124";

    [Fact]
    public void RangeTest()
    {
        var str = "111222";
        str[..(str.Length / 2)].Should().Be("111");
        str[(str.Length / 2)..].Should().Be("222");
    }

    [Fact]
    public void ChunkTest()
    {
        var str = "121212";
        var chunks = str.Chunk(2);
        chunks.All(c => c.SequenceEqual(chunks.First())).Should().BeTrue();
    }

    [Fact]
    public void Example() => FindInvalidIds(_example).Should().Be((1227775554, 4174379265));

    [Fact]
    public async Task PartOne() => WriteOutput(FindInvalidIds(await ReadInputFile()).Double);

    [Fact]
    public async Task PartTwo() => WriteOutput(FindInvalidIds(await ReadInputFile()).Multiple);

    private static (long Double, long Multiple) FindInvalidIds(string input)
    {
        var rawRanges = input.Split(",", StringSplitOptions.RemoveEmptyEntries);
        var ranges = rawRanges.Select(r => r.Split("-", StringSplitOptions.RemoveEmptyEntries))
            .Select(r => (From: long.Parse(r[0]), To: long.Parse(r[1])))
            .ToArray();

        var invalidDouble = 0L;
        var invalidMultiple = 0L;
        foreach (var (from, to) in ranges)
        {
            for (var i = from; i <= to; i++)
            {
                var str = i.ToString();
                var len = str.Length;
                if (str[..(len / 2)] == str[(len / 2)..])
                    invalidDouble += i;
                for (var j = 1; j <= len / 2; j++)
                {
                    var chunks = str.Chunk(j);
                    if (chunks.All(c => c.SequenceEqual(chunks.First())))
                    {
                        invalidMultiple += i;
                        break;
                    }
                }
            }
        }

        return (invalidDouble, invalidMultiple);
    }
}
