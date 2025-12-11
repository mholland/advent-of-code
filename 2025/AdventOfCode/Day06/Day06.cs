namespace AdventOfCode.Day06;

public sealed class Day06(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2025, 12, 06);

    private readonly string[] _example =
    [
        "123 328  51 64 ",
        " 45 64  387 23 ",
        "  6 98  215 314",
        "*   +   *   +  "
    ];

    [Fact]
    public void ExampleOne() => CalculateGrandTotal(_example).Should().Be(4277556);

    [Fact]
    public void ExampleTwo() => CalculateGrandTotalVertical(_example).Should().Be(3263827);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculateGrandTotal(await ReadInputLines()));

    [Fact]
    public async Task PartTwo() => WriteOutput(CalculateGrandTotalVertical(await ReadInputLines()));

    public static long CalculateGrandTotalVertical(string[] input)
    {
        var digits = input[..^1];
        var operators = input[^1];
        var colIdx = 0;
        var grandTotal = 0L;

        var sumsLength = digits.Max(d => d.Length);
        while (colIdx < sumsLength)
        {
            var op = operators[colIdx];
            var nextSep = colIdx;
            while (true)
            {
                nextSep += 1;
                if (nextSep >= sumsLength || IsSeparator(nextSep))
                    break;
            }
            var total = GetNumber(colIdx);
            for (var x = colIdx + 1; x < nextSep; x++)
            {
                var num = GetNumber(x);
                total = op switch
                {
                    '*' => total * num,
                    '+' => total + num,
                    _ => throw new NotImplementedException()
                };
            }
            grandTotal += total;
            colIdx = nextSep + 1;
        }

        return grandTotal;

        long GetNumber(int x)
        {
            var num = string.Empty;
            for (var y = 0; y < digits.Length; y++)
                num += digits[y][x];
            return long.Parse(num.Trim());
        }

        bool IsSeparator(int x)
        {
            for (var y = 0; y < digits.Length; y++)
                if (char.IsDigit(digits[y][x]))
                    return false;

            return true;
        }
    }

    private static long CalculateGrandTotal(string[] input)
    {
        var numbers = input[..^1]
            .Select(row => row
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(long.Parse)
                .ToArray())
            .ToArray();
        var operators = input[^1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var grandTotal = 0L;
        var width = numbers.Max(x => x.Length);
        for (var x = 0; x < width; x++)
        {
            var colTotal = numbers[0][x];
            var op = operators[x];
            for (var y = 1; y < numbers.Length; y++)
            {
                colTotal = op switch
                {
                    "+" => colTotal + numbers[y][x],
                    "*" => colTotal * numbers[y][x],
                    _ => throw new NotImplementedException()
                };
            }
            grandTotal += colTotal;
        }

        return grandTotal;
    }
}
