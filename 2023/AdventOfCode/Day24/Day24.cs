using Vec3 = (double X, double Y, double Z);

namespace AdventOfCode.Day24;

public sealed class Day24(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day24";

    private readonly string[] _example =
    [
        "19, 13, 30 @ -2,  1, -2",
        "18, 19, 22 @ -1, -1, -2",
        "20, 25, 34 @ -2, -2, -4",
        "12, 31, 28 @ -1, -2, -1",
        "20, 19, 15 @  1, -5, -3"
    ];

    [Fact]
    public void ExampleOne() => CountIntersections(_example, 7, 27).Should().Be(2);

    [Fact]
    public void PartOne() => WriteOutput(CountIntersections(Input, 2e14, 4e14));

    [Fact]
    public void ExampleTwo() => CalculateRockPosition(_example).Should().Be(47);

    [Fact]
    public void PartTwo() => WriteOutput(CalculateRockPosition(Input));

    [Fact]
    public void GaussianElimination()
    {
        var m = new double[,]
        {
            { 3.0, 2.0, -4.0, 3.0 },
            { 2.0, 3.0, 3.0, 15.0 },
            { 5.0, -3, 1.0, 14.0 }
        };
        var p = PartialPivot(m);
        var s = BackSubstitute(p).Select(x => Math.Round(x)).ToArray();
        s[..3].Should().BeEquivalentTo([3, 1, 2]);
    }

    private static double CalculateRockPosition(string[] input)
    {
        var hails = input.Select(i => i.Split('@', StringSplitOptions.TrimEntries))
            .Select(h => new Hail(Parse(h[0]), Parse(h[1])))
            .ToArray();

        var (h1, h2, h3) = (hails[0], hails[1], hails[2]);
        var matrix = new double[6, 7];
        matrix = FillXY(matrix, 0, h1, h2);
        matrix = FillXY(matrix, 1, h1, h3);
        matrix = FillXZ(matrix, 2, h1, h2);
        matrix = FillXZ(matrix, 3, h1, h3);
        matrix = FillYZ(matrix, 4, h1, h2);
        matrix = FillYZ(matrix, 5, h1, h3);

        var pivot = PartialPivot(matrix);
        var solutions = BackSubstitute(pivot);

        return solutions.Take(3).Select(x => Math.Round(x)).Sum();

        static double[,] FillXY(double[,] mat, int row, Hail h1, Hail h2)
        {
            var (x1, y1, _) = h1.P;
            var (vx1, vy1, _) = h1.V;
            var (x2, y2, _) = h2.P;
            var (vx2, vy2, _) = h2.V;
            mat[row, 0] = vy2 - vy1;
            mat[row, 1] = vx1 - vx2;
            mat[row, 2] = 0;
            mat[row, 3] = y1 - y2;
            mat[row, 4] = x2 - x1;
            mat[row, 5] = 0;
            mat[row, 6] = x2 * vy2 - vx2 * y2 + vx1 * y1 - x1 * vy1;

            return mat;
        }

        static double[,] FillXZ(double[,] mat, int row, Hail h1, Hail h2)
        {
            var (x1, _, z1) = h1.P;
            var (vx1, _, vz1) = h1.V;
            var (x2, _, z2) = h2.P;
            var (vx2, _, vz2) = h2.V;
            mat[row, 0] = vz2 - vz1;
            mat[row, 1] = 0;
            mat[row, 2] = vx1 - vx2;
            mat[row, 3] = z1 - z2;
            mat[row, 4] = 0;
            mat[row, 5] = x2 - x1;
            mat[row, 6] = x2*vz2-vx2*z2+vx1*z1-x1*vz1;

            return mat;
        }

        static double[,] FillYZ(double[,] mat, int row, Hail h1, Hail h2)
        {
            var (_, y1, z1) = h1.P;
            var (_, vy1, vz1) = h1.V;
            var (_, y2, z2) = h2.P;
            var (_, vy2, vz2) = h2.V;
            mat[row, 0] = 0;
            mat[row, 1] = vz1 - vz2;
            mat[row, 2] = vy2 - vy1;
            mat[row, 3] = 0;
            mat[row, 4] = z2 - z1;
            mat[row, 5] = y1 - y2;
            mat[row, 6] = z2 * vy2 - vz2 * y2 + vz1 * y1 - z1 * vy1;

            return mat;
        }
    }

    private static double[] BackSubstitute(double[,] matrix)
    {
        var n = matrix.GetLength(0);
        var s = new double[n];
        for (var i = n - 1; i >= 0; i--)
        {
            double sum = 0;
            for (var j = i + 1; j < n; j++)
            {
                sum += matrix[i, j] * s[j];
            }
            s[i] = (matrix[i, n] - sum) / matrix[i, i];
        }

        return s;
    }

    private static double[,] PartialPivot(double[,] m)
    {
        var n = m.GetLength(0);
        for (var i = 0; i < n; i++)
        {
            var pivotRow = i;
            for (var j = i + 1; j < n; j++)
            {
                if (Math.Abs(m[j, i]) > Math.Abs(m[pivotRow, i]))
                {
                    pivotRow = j;
                }
            }
            if (pivotRow != i)
            {
                for (var j = i; j <= n; j++)
                {
                    (m[i, j], m[pivotRow, j]) = (m[pivotRow, j], m[i, j]);
                }
            }
            for (var j = i + 1; j < n; j++)
            {
                var factor = m[j, i] / m[i, i];
                for (var k = i; k <= n; k++)
                    m[j, k] -= factor * m[i, k];
            }
        }

        return m;
    }

    private static int CountIntersections(string[] input, double testAreaMin, double testAreaMax)
    {
        var hails = input.Select(i => i.Split('@', StringSplitOptions.TrimEntries))
            .Select(h => new Hail(Parse(h[0]), Parse(h[1])))
            .ToArray();

        var intersect = 0;
        for (var i = 0; i < hails.Length - 1; i++)
        {
            var hail = hails[i];
            for (var j = i + 1; j < hails.Length; j++)
            {
                var other = hails[j];
                if (other.P == hail.P && other.V == hail.V)
                    continue;

                var (x1, y1, _) = hail.P;
                var (x2, y2, _) = hail.Progress();
                var (x3, y3, _) = other.P;
                var (x4, y4, _) = other.Progress();

                // https://en.wikipedia.org/wiki/Line%E2%80%93line_intersection#Given_two_points_on_each_line
                var t1 = ((x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
                var t2 = ((x1 - x3) * (y1 - y2) - (y1 - y3) * (x1 - x2)) / ((x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4));
                if (t1 < 0 || t2 < 0)
                    continue;

                var px = x1 + t1 * (x2 - x1);
                var py = y1 + t1 * (y2 - y1);

                if (px >= testAreaMin && px <= testAreaMax &&
                    py >= testAreaMin && py <= testAreaMax)
                    intersect += 1;
            }
        }

        return intersect;
    }

    private static Vec3 Parse(string val)
    {
        var v = val.Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .Select(double.Parse)
            .ToArray();
        return (v[0], v[1], v[2]);
    }

    private record Hail(Vec3 P, Vec3 V)
    {
        public Vec3 Progress() =>
            (P.X + V.X, P.Y + V.Y, P.Z + V.Z);
    }
}
