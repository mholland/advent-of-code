depths = [int(i) for i in open("inputs/day01.input")]

increases = 0
window_increases = 0
for i in range(len(depths)):
    if i > 0 and depths[i] > depths[i - 1]:
        increases += 1

    if i > 2 and depths[i] > depths[i-3]:
        window_increases += 1


print(f"Larger measurements: {increases}")
print(f"Larger window measurements: {window_increases}")
