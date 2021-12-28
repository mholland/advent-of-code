from collections import defaultdict

fish = None
with open("inputs/day06.input") as f:
    fish = [int(x) for x in f.readline().strip().split(",")]

counts = defaultdict(int)
for f in range(len(fish)):
    counts[fish[f]] += 1

def iterate(counts: dict, days: int) -> int:
    for _ in range(days):
        new_counts = defaultdict(int)
        for d, count in counts.items():
            if d == 0:
                new_counts[6] += count
                new_counts[8] += count
            else:
                new_counts[d - 1] += count
        counts = new_counts

    return sum(counts.values())

print(iterate(counts, 80))
print(iterate(counts, 256))
