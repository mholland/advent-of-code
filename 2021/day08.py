entries = None
with open("inputs/day08.input", encoding = "utf-8") as f:
    entries = [x.strip() for x in f]

def count(l: list, length: int) -> int:
    return len([x for x in l if len(x) == length])

def norm(inp: str) -> str:
    s = sorted(inp)
    return "".join(s)

def contains_segs(first: str, second: str) -> bool:
    for seg in second:
        if seg not in first:
            return False
    return True

checksum = 0
total_output = 0
cs_digit_lens = [2,3,4,7]
for e in entries:
    patterns, output = [x.strip().split() for x in e.split("|")]

    checksum += sum([count(output, l) for l in cs_digit_lens])
    mappings = dict()
    one = two = three = four = five = six = seven = eight = nine = None
    for p in patterns:
        if len(p) == 2:
            one = norm(p)
        if len(p) == 3:
            seven = norm(p)
        if len(p) == 4:
            four = norm(p)
        if len(p) == 7:
            eight = norm(p)

    five_lens = list(filter(lambda n: len(n) == 5, patterns))
    three = norm(next(filter(lambda n: contains_segs(n, seven), five_lens), None))
    ch = four
    for s in one:
        ch = ch.replace(s, "")
    five = norm(next(filter(lambda n: contains_segs(n, ch), five_lens), None))
    two = norm(next(filter(lambda n: norm(n) != three and norm(n) != five, five_lens), None))

    six_lens = list(filter(lambda n: len(n) == 6, patterns))
    nine = norm(next(filter(lambda n: contains_segs(n, four), six_lens), None))
    zero = norm(next(filter(lambda n: contains_segs(n, one) and norm(n) != nine, six_lens), None))
    six = norm(next(filter(lambda n: norm(n) != nine and norm(n) != zero, six_lens), None))
    mappings[zero] = 0
    mappings[one] = 1
    mappings[two] = 2
    mappings[three] = 3
    mappings[four] = 4
    mappings[five] = 5
    mappings[six] = 6
    mappings[seven] = 7
    mappings[eight] = 8
    mappings[nine] = 9

    total_output += 1000 * mappings[norm(output[0])]
    total_output += 100 * mappings[norm(output[1])]
    total_output += 10 * mappings[norm(output[2])]
    total_output += 1 * mappings[norm(output[3])]

print(checksum)
print(total_output)
