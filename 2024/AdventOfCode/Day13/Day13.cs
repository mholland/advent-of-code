namespace AdventOfCode.Day13;

public sealed class Day13(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 13);

    private const string Example =
        """
        Button A: X+94, Y+34
        Button B: X+22, Y+67
        Prize: X=8400, Y=5400

        Button A: X+26, Y+66
        Button B: X+67, Y+21
        Prize: X=12748, Y=12176

        Button A: X+17, Y+86
        Button B: X+84, Y+37
        Prize: X=7870, Y=6450

        Button A: X+69, Y+23
        Button B: X+27, Y+71
        Prize: X=18641, Y=10279
        """;

    [Fact]
    public void ExampleOne() => FewestTokens(Example).Should().Be(480);

    [Fact]
    public async Task PartOne() => WriteOutput(FewestTokens(await ReadInputFile()));

    [Fact]
    public async Task PartTwo() => WriteOutput(FewestTokens(await ReadInputFile(), true));

    private static long FewestTokens(string input, bool unitFix = false)
    {
        var machines = input.Split(Environment.NewLine + Environment.NewLine).Select(m => m.Split(Environment.NewLine)).ToArray();
        var fewest = 0L;
        foreach (var machine in machines)
        { 
            var (ax, ay) = ParseButton(machine[0]);
            var (bx, by) = ParseButton(machine[1]);
            var (px, py) = ParsePrize(machine[2], unitFix);

            // Working presses out via formula
            // Ap = A presses, Bp = B presses
            // For some Ap, Bp
            // AxAp + BxBp = Px
            // AyAp + ByBp = Py
            // (Ax.By)Ap + (Bx.By)Bp = Px.By
            // (Ay.Bx)Ap + (Bx.By)Bp = Py.Bx
            // (Ax.By - Ay.Bx)Ap = Px.By - Py.Bx
            // Ap = (Px.By - Py.Bx) / (Ax.By - Ay.Bx)
            var numerator = px * by - py * bx;
            var denominator = ax * by - ay * bx;
            if (numerator == 0 || denominator == 0) continue;
            var (aPresses, remainder) = Math.DivRem(numerator, denominator);
            if (remainder != 0) continue;
            
            var bPresses = (px - ax * aPresses) / bx;
            if (aPresses * ax + bPresses * bx != px) continue;
            if (aPresses * ay + bPresses * by != py) continue;

            fewest += 3 * aPresses + bPresses;
        }

        return fewest;

        (long X, long Y) ParseButton(string b)
        {
            var split = b.Split(' ');
            var x = long.Parse(split[2][2..^1]);
            var y = long.Parse(split[3][2..]);
            return (x, y);
        }

        (long X, long Y) ParsePrize(string b, bool unitFixed)
        {
            var split = b.Split(' ');
            var x = long.Parse(split[1][2..^1]);
            var y = long.Parse(split[2][2..]);
            return unitFixed ? (x + 10000000000000, y + 10000000000000) : (x, y);
        }
    }

    // For part one, not suitable for part two
    private static long SimulatePresses(int ax, int ay, int bx, int by, long px, long py)
    {
        var fewest = 0L;
        for (var aPresses = 0; aPresses <= 100; aPresses++)
        {
            var remainingX = px - aPresses * ax;
            var remainingY = py - aPresses * ay;
            if (remainingX % bx != 0 || remainingY % by != 0)
                continue;
            var bPressesX = remainingX / bx;
            var bPressesY = remainingY / by;
            if (bPressesX != bPressesY)
                continue;
            fewest += 3 * aPresses + bPressesX;
        }

        return fewest;
    }
}