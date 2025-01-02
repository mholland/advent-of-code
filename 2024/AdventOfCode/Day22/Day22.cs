using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AdventOfCode.Day22;

public sealed class Day22(ITestOutputHelper output) : TestBase(output)
{
    protected override DateOnly Date => new(2024, 12, 22);

    private readonly string[] _example =
    [
        "1",
        "10",
        "100",
        "2024"
    ];

    [Fact]
    public void MixCorrectly() => Mix(42, 15).Should().Be(37);

    [Fact]
    public void PruneCorrectly() => Prune(100000000).Should().Be(16113920);

    [Theory]
    [InlineData(123, 15887950)]
    [InlineData(15887950, 16495136)]
    [InlineData(16495136, 527345)]
    [InlineData(527345, 704524)]
    public void GenerateSecretCorrectly(int secret, int newSecret) => GenerateSecret(secret).Should().Be(newSecret);

    [Fact]
    public void ExampleOne() => CalculateSecrets(_example).Should().Be(37327623);

    [Fact]
    public async Task PartOne() => WriteOutput(CalculateSecrets(await ReadInputLines()));

    private readonly string[] _exampleTwo =
    [
        "1",
        "2",
        "3",
        "2024"
    ];

    [Fact]
    public void ExampleTwo() => MaxBananas(_exampleTwo).Should().Be(23);

    [Fact]
    public async Task PartTwo() => WriteOutput(MaxBananas(await ReadInputLines()));

    private static int MaxBananas(string[] input)
    {
        var secretSets = input.Select(long.Parse).Select(GenerateSecrets).ToArray();
        var comparer = EqualityComparer<int[]>.Create(
            StructuralComparisons.StructuralEqualityComparer.Equals,
            StructuralComparisons.StructuralEqualityComparer.GetHashCode);
        var totals = new Dictionary<int[], int>(comparer);

        foreach (var set in secretSets)
            UpdateBananaCounts(set, totals);

        return totals.Max(t => t.Value);

        void UpdateBananaCounts(List<long> set, Dictionary<int[], int> t)
        {
            var ones = set.Select(s => Convert.ToInt32(s % 10)).ToArray();
            var diffs = new List<(int Secret, int Diff)>(2000);
            var seen = new HashSet<int[]>(comparer);
            for (var i = 1; i < ones.Length; i++)
            {
                var diff = ones[i] - ones[i - 1];
                diffs.Add((ones[i], diff));
            }
            for (var i = 0; i < diffs.Count - 3; i++)
            {
                var four = diffs[i..(i + 4)];
                var d = four.Select(f => f.Diff).ToArray();
                if (!seen.Add(d))
                    continue;
                var bananas = four.Last().Secret;
                if (bananas == 0) continue;
                if (t.TryGetValue(d, out var b))
                    bananas += b;
                t[d] = bananas;
            }
        }
    }

    private record DiffSet(int[] Diffs, int Final);

    private static long CalculateSecrets(string[] input) =>
        input
            .Select(long.Parse)
            .Select(GenerateSecrets)
            .Aggregate(0L, (agg, cur) => agg + cur.Last());

    private static List<long> GenerateSecrets(long secret)
    {
        var s = new List<long>(2001) { secret };
        for (var i = 0; i < 2000; i++)
            s.Add(GenerateSecret(s.Last()));

        return s;
    }

    private static long GenerateSecret(long seed)
    {
        var newSecret = Prune(Mix(seed, seed * 64));
        newSecret = Prune(Mix(newSecret, newSecret / 32));
        return Prune(Mix(newSecret, newSecret * 2048));
    }

    private static long Mix(long number, long value) => value ^ number;

    private static long Prune(long number) => number % 16777216;
}
