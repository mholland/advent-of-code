DOTS = set()
FOLDS = []
with open("inputs/day13.input", encoding = "utf-8") as f:
    for line in f:
        if line.startswith("fold"):
            axis, val = line.split(" ")[-1].split("=")
            val = int(val)
            FOLDS.append((axis, val))
            continue
        if line == "\n":
            continue
        x,y = line.split(",")
        x,y = int(x), int(y)
        DOTS.add((x,y))

def print_dots(dots):
    mx = max(dots, key=lambda x: x[0])[0]
    my = max(dots, key=lambda x: x[1])[1]
    for y in range(my + 1):
        line = ""
        for x in range(mx + 1):
            line += "#" if (x,y) in dots else "."
        print(line)
    print("")

# print_dots(DOTS)
for i, fold in enumerate(FOLDS):
    axis, pos = fold
    NEW_DOTS = set()
    for dot in DOTS:
        x,y = dot
        if axis == "y" and y > pos:
            NEW_DOTS.add((x, pos + pos - y))
        elif axis == "x" and x > pos:
            NEW_DOTS.add((pos + pos - x, y))
        else:
            NEW_DOTS.add(dot)
    DOTS = NEW_DOTS
    if i == 0:
        print(len(DOTS))

print_dots(DOTS)
