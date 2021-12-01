lines = []
with open("inputs/day01.input") as f:
    lines = [int(i) for i in f]

increases = 0
window_increases = 0
for i in range(len(lines)):
    if i > 0 and lines[i] > lines[i - 1]:
        increases += 1

    if i > 2 and lines[i] > lines[i-3]:
        window_increases += 1


print(f"Larger measurements: {increases}")
print(f"Larger window measurements: {window_increases}")
