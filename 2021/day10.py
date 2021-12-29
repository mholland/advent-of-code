L = None
with open("inputs/day10.input", encoding = "utf-8") as f:
    L = [list(l.strip()) for l in f.readlines()]

ES = 0
CS = []
P = {
    "(": (")", 3, 1),
    "[": ("]", 57, 2),
    "{": ("}", 1197, 3),
    "<": (">", 25137, 4)
}

for l in L:
    S = []
    error = False
    for c in l:
        if c in ["{", "[", "(", "<"]:
            S.append(c)
            continue
        pop = S.pop()
        if c != P[pop][0]:
            ES += P[pop][1]
            error = True
            break

    if error:
        continue

    cs = 0
    while S:
        x = S.pop()
        cs = cs * 5 + P[x][2]
    CS.append(cs)

CS.sort()

print(ES)
print(CS[len(CS)//2])