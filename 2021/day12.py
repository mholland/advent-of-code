from collections import defaultdict, deque

E = defaultdict(list)
with open("inputs/day12.input", encoding = "utf-8") as f:
    for line in f:
        start, end = line.strip().split("-")
        E[start].append(end)
        E[end].append(start)

def solve(p2: bool) -> int:
    Q = deque()
    init = ("start", ["start"], False)
    Q.append(init)
    P = 0
    while Q:
        cur, visited, twice = Q.popleft()
        for e in E[cur]:
            t = twice
            if e == "end":
                P += 1
                continue
            if e.islower() and e in visited and (twice or e == "start" and p2):
                continue
            if e.islower() and e in visited:
                if not p2:
                    continue
                t = True

            new_visited = visited.copy()
            new_visited.append(e)
            Q.append((e, new_visited, t))
    return P

print(solve(False))
print(solve(True))