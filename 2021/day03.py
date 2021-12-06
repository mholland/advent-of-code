lines = [i.strip() for i in open("inputs/day03.input")]
word_length = len(lines[0])

counts = []
for i in range(word_length):
    counts.append([0, 0])

for w in range(word_length):
    for n, line in enumerate(lines):
        digit = line[w]
        if digit == "0":
            counts[w][0] += 1
        else:
            counts[w][1] += 1

gamma = "0b"
epsilon = "0b"
for cts in counts:
    gamma += "0" if cts[0] > cts[1] else "1"
    epsilon += "1" if cts[0] > cts[1] else "0"

print(int(gamma, 2)*int(epsilon, 2))

