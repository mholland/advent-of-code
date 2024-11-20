from collections import defaultdict
import heapq
import sys

GRID = {}
X = 0
Y = 0
with open("inputs/" + sys.argv[1], encoding="utf-8") as f:
    lines = [l.strip() for l in f.readlines()]
    Y = len(lines)
    X = len(lines[0])
    for yy, l in enumerate(lines):
        for xx, c in enumerate(l):
            GRID[(xx, yy)] = int(c)

start = (0, 0)
dirs = [(1, 0), (0, 1), (-1, 0), (0, -1)]

def get_risk(grid, x, y):
    risk = grid[(x%X, y%Y)] + (x//X) + (y//Y)
    if risk > 9:
        risk -= 9
    return risk


def traverse(tiles):
    Q = []
    visited = defaultdict(lambda: sys.maxsize)
    heapq.heappush(Q, ((0,0), 0))
    max_x, max_y = (X*tiles - 1,Y*tiles - 1)
    while Q:
        (x, y), dist = heapq.heappop(Q)
        if (x, y) == (max_x, max_y):
            return dist

        for dx, dy in dirs:
            nx, ny = x + dx, y + dy
            if nx<0 or nx>max_x or ny<0 or ny>max_y:
                continue
            n_risk = get_risk(GRID, nx, ny)
            n_dist = dist + n_risk
            if n_dist < visited[(nx, ny)]:
                visited[(nx, ny)] = n_dist
                heapq.heappush(Q, [(nx, ny), n_dist])

print(traverse(1))
print(traverse(5))
