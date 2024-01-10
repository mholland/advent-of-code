from collections import defaultdict
import sys

T = ""
RULES = {}
with open("inputs/" + sys.argv[1], encoding = "utf-8") as f:
    lines = [l.strip() for l in f.readlines()]
    T = lines[0]
    for r in lines[2:]:
        frm, to = r.split(" -> ")
        RULES[frm] = to

def find_quantity_range(counts):
    PC = defaultdict(int)
    for p in counts:
        PC[p[0]] += counts[p]
    PC[T[-1]] += 1
    return max(PC.values()) - min(PC.values())

COUNTS = defaultdict(int)
for i in range(len(T) - 1):
    COUNTS[T[i] + T[i + 1]] += 1

for i in range(40):
    NEW_COUNTS = defaultdict(int)
    for p in COUNTS:
        result = RULES[p]
        NEW_COUNTS[p[0] + result] += COUNTS[p]
        NEW_COUNTS[result + p[1]] += COUNTS[p]
    COUNTS = NEW_COUNTS
    if i + 1 in [10, 40]:
        print(find_quantity_range(COUNTS))


