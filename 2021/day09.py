from functools import reduce
from typing import Deque

L = []
with open("inputs/day09.input", encoding = "utf-8") as f:
    L = []
    for line in f.readlines():
        L.append([int(x) for x in line.strip()])

D = [(-1, 0), (1, 0), (0, -1), (0, 1)]
LP = []
RL = 0

R = len(L)
C = len(L[0])
for r in range(R):
    for c in range(C):
        low = True
        for d in D:
            rr = r + d[0]
            cc = c + d[1]
            if 0 <= rr < R and 0 <= cc < C and L[r][c] >= L[rr][cc]:
                low = False
        if low:
            LP.append((r, c))
            RL += L[r][c] + 1

print(RL)
V = set()
S = []
for lp in LP:
    basin_size = 0
    Q = Deque()
    Q.append(lp)
    while Q:
        rc = Q.popleft()
        if rc in V:
            continue
        V.add(rc)
        basin_size += 1
        for d in D:
            rr = rc[0] + d[0]
            cc = rc[1] + d[1]
            if 0 <= rr < R and 0 <= cc < C and L[rr][cc] != 9:
                Q.append((rr, cc))
    S.append(basin_size)

S.sort(reverse=True)
print(reduce(lambda x, y: x * y, S[:3], 1))
