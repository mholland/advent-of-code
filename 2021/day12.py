from collections import defaultdict, deque

E = defaultdict(list)
with open("inputs/day12.input", encoding = "utf-8") as f:
    for line in f:
        start, end = line.strip().split("-")
        E[start].append(end)
        E[end].append(start)

Q = deque()
init = ("start", ["start"])
Q.append(init)
P = 0
while Q:
    cur, visited = Q.popleft()
    for e in E[cur]:
        if e == "end":
            P += 1
            continue
        if e.islower() and e in visited:
            continue
        new_visited = visited.copy()
        new_visited.append(e)
        Q.append((e, new_visited))

print(P)