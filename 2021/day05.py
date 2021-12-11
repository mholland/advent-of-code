from collections import defaultdict

coords = []
with open("inputs/day05.input", encoding = "utf-8") as f:
    for line in f:
        l = line.strip()
        u, v = l.split(" -> ")
        start = [int(x) for x in u.split(",")]
        end = [int(x) for x in v.split(",")]

        coords.append((start, end))

ventsA = defaultdict(int)
ventsB = defaultdict(int)
for coord in coords:
    x1, y1 = coord[0]
    x2, y2 = coord[1]
    if x1 != x2 and y1 != y2:
        continue

    for i in range(min(x1,x2), max(x1+1,x2+1)):
        for j in range(min(y1,y2), max(y1+1,y2+1)):
            ventsA[(i, j)] += 1

overlapA = 0
for k, v in ventsA.items():
    if v > 1:
        overlapA += 1

print(overlapA)