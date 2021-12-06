lines = [i.strip() for i in open("inputs/day03.input")]
word_length = len(lines[0])

def bit_counts(list: list, digit: int) -> list:
    bits = [0,0]
    for code in list:
        bits[0 if code[digit] == "0" else 1] += 1

    return bits

gamma = "0b"
epsilon = "0b"
for w in range(word_length):
    bits = bit_counts(lines, w)
    gamma += "0" if bits[0] > bits[1] else "1"
    epsilon += "0" if bits[0] < bits[1] else "1"

print(int(gamma, 2)*int(epsilon, 2))

OGR = lines.copy()
CSR = lines.copy()

for w in range(word_length):
    if len(OGR) > 1:
        ogr0 = [c for c in OGR if c[w] == "0"]
        ogr1 = [c for c in OGR if c[w] == "1"]
        OGR = ogr1 if len(ogr1) >= len(ogr0) else ogr0

    if len(CSR) > 1:
        csr0 = [c for c in CSR if c[w] == "0"]
        csr1 = [c for c in CSR if c[w] == "1"]
        CSR = csr0 if len(csr0) <= len(csr1) else csr1

assert(len(CSR) == 1)
assert(len(OGR) == 1)

print(int(OGR[0], 2)*int(CSR[0], 2))
