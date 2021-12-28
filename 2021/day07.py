crabs = None
with open("inputs/day07.input", encoding = "utf-8") as f:
    crabs = [int(x) for x in f.readline().strip().split(",")]

assert(len(crabs) > 0)

min, max = min(crabs), max(crabs)
min_fuelA = None
min_fuelB = None

for i in range(min, max + 1):
    fuelA = 0
    fuelB = 0
    for c in crabs:
        dist = abs(i-c)
        fuelA += dist
        fuelB += (dist * (dist + 1)) // 2
    if min_fuelA is None or fuelA < min_fuelA:
        min_fuelA = fuelA
    if min_fuelB is None or fuelB < min_fuelB:
        min_fuelB = fuelB

print(min_fuelA)
print(min_fuelB)