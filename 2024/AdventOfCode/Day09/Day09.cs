namespace AdventOfCode.Day09;

public sealed class Day09(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 09);

    private readonly string[] _example =
    [//  0123456789
        "2333133121414131402"
    ];// 0 1 2 3 4

    [Fact]
    public void ExampleOne() => CalculateChecksum(_example).Should().Be(1928);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculateChecksum(await ReadInputLines()));

    [Fact]
    public void ExampleTwo() => CalculateChecksum(_example, true).Should().Be(2858);

    [Fact]
    public async Task PartTwo() => WriteOutput(CalculateChecksum(await ReadInputLines(), true));

    private static long CalculateChecksum(string[] input, bool wholeFiles = false)
    {
        var files = input[0].Select(c => c - '0').ToArray();
        var inputSize = files.Sum();
        var disk = new int?[inputSize];
        var cursor = 0;
        for (var i = 0; i < files.Length; i++)
        {
            var file = files[i];
            if (i % 2 == 0)
                for (var x = 0; x < file; x++)
                {
                    disk[cursor + x] = i / 2;
                }

            cursor += file;
        }

        var moved = new HashSet<int>();
        var (nextMoveable, size, id) = wholeFiles 
            ? FindFirstMoveableFile(disk, disk.Length - 1, moved) 
            : FindFirstMoveableFile(disk, disk.Length - 1, moved, 1);
        var nextFree = FindFirstEmptySlot(disk, size);

        while (nextMoveable > 0)
        {
            if (nextFree >= 0 && nextFree < nextMoveable - size)
            {
                for (var i = nextFree; i < nextFree + size; i++)
                    disk[i] = id;
                for (var i = nextMoveable; i > nextMoveable - size; i--)
                    disk[i] = null;
                moved.Add(id);
            }

            (nextMoveable, size, id) = wholeFiles 
                ? FindFirstMoveableFile(disk, nextMoveable - size, moved)
                : FindFirstMoveableFile(disk, nextMoveable - 1, moved, 1);
            if (nextMoveable == -1) break;
            nextFree = FindFirstEmptySlot(disk, size);
        }

        var checksum = 0L;
        for (var i = 0; i < disk.Length; i++)
        {
            if (disk[i] is not { } d) continue;
            checksum += d * i;
        }

        return checksum;

        int FindFirstEmptySlot(int?[] d, int sz = 1)
        {
            var cur = 0;
            while (cur < d.Length - sz)
            {
                if (d[cur] != null)
                {
                    cur += 1;
                    continue;
                }
                if (d[cur..(cur + sz)].All(s => s is null))
                    return cur;
                cur += 1;
            }

            return -1;
        }

        (int Start, int Length, int Id) FindFirstMoveableFile(int?[] d, int start, HashSet<int> alreadyMoved, int max = int.MaxValue)
        {
            var cur = start;
            while (cur > 0)
            {
                if (d[cur] is not { } idn || (wholeFiles && alreadyMoved.Contains(idn)))
                {
                    cur -= 1;
                    continue;
                }

                var length = 1;
                var lc = cur - 1;
                while (lc >= 0 && d[lc] == d[cur] && length < max)
                {
                    length += 1;
                    lc -= 1;
                }
                return (cur, length, idn);
            }

            return (-1, -1, -1);
        }
    }
}