O = []
with open("inputs/day11.input", encoding = "utf-8") as f:
    for line in f:
        O.append([int(x) for x in line.strip()])

def above_energy(o: list) -> bool:
    for r in range(len(o)):
        for c in range(len(o[r])):
            if o[r][c] != None and o[r][c] > 9:
                return True
    return False

R = len(O)
C = len(O[0])
D = [
    (-1, 0), # Up
    (1, 0), # Down
    (0, -1), # Left
    (0, 1), # Right
    (-1, -1), # Up-left
    (-1, 1), # Up-right
    (1, 1), # Down-right
    (1, -1), # Down-left
]
F = 0
T = 0
while True:
    synced = True
    T += 1
    for r in range(R):
        for c in range(C):
            O[r][c] += 1

    while above_energy(O):
        for r in range(R):
            for c in range(C):
                if O[r][c] is not None and O[r][c] > 9:
                    O[r][c] = None
                    for d in D:
                        rr = r + d[0]
                        cc = c + d[1]
                        if 0 <= rr < R and 0 <= cc < C and O[rr][cc] != None:
                            O[rr][cc] += 1

    for r in range(R):
        for c in range(C):
            if O[r][c] == None:
                if T <= 100:
                    F += 1
                O[r][c] = 0
            else:
                synced = False

    # print("\n".join(["".join([str(y) for y in x]) for x in O]) + "\n")
    if synced:
        break

print(F)
print(T)