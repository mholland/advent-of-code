L = None
with open("inputs/day10.input", encoding = "utf-8") as f:
    L = [list(l.strip()) for l in f.readlines()]

ES = 0
CS = []
for l in L:
    S = []
    error = False
    for c in l:
        if c in ["{", "[", "(", "<"]:
            S.append(c)
            continue
        pop = S.pop()
        if c == ")" and pop != "(":
            ES += 3
            error = True
            break
        if c == "]" and pop != "[":
            ES += 57
            error = True
            break
        if c == "}" and pop != "{":
            ES += 1197
            error = True
            break
        if c == ">" and pop != "<":
            ES += 25137
            error = True
            break

    if error:
        continue

    cs = 0
    while S:
        x = S.pop()
        cs *= 5
        if x == "(":
            cs += 1
        if x == "[":
            cs += 2
        if x == "{":
            cs += 3
        if x == "<":
            cs += 4
    
    CS.append(cs)
CS.sort()

print(ES)
print(CS[len(CS)//2])