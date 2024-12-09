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

    private static long CalculateChecksum(string[] input)
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

        var nextFree = FindFirstEmptySlot(disk, 0);
        var (nextMoveable, _) = FindFirstMoveableBlock(disk, disk.Length - 1, 1);
        while (nextFree < nextMoveable)
        {
            disk[nextFree] = disk[nextMoveable];
            disk[nextMoveable] = null;
            nextFree = FindFirstEmptySlot(disk, nextFree + 1);
            (nextMoveable, _) = FindFirstMoveableBlock(disk, nextMoveable - 1, 1);
        }

        var checksum = 0L;
        for (var i = 0; i < disk.Length; i++)
        {
            if (disk[i] is not { } d) continue;
            checksum += d * i;
        }

        return checksum;

        int FindFirstEmptySlot(int?[] d, int start = 0, int size = 1)
        {
            var cur = start;
            while (cur < d.Length)
            {
                if (d[cur] != null)
                {
                    cur += 1;
                    continue;
                }
                if (d[cur..(cur + size)].All(s => s is null))
                    return cur;
            }

            return -1;
        }

        (int Start, int Length) FindFirstMoveableBlock(int?[] d, int start, int max = int.MaxValue)
        {
            var cur = start;
            while (d[cur] == null)
            {
                cur -= 1;
                if (cur == 0)
                    return (0, -1);
            }
            var length = 1;
            var lc = cur - 1;
            while (d[lc] == d[cur] && length < max)
            {
                length += 1;
                lc -= 1;
            }
            return (cur, length);
        }
    }
}