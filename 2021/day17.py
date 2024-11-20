import sys

def parse_input(input):
    coords = input.split(": ")[1]
    x,y = coords.split(", ")
    x1, x2 = x.split("..")
    x1 = int(x1[2:])
    x2 = int(x2)
    y1, y2 = y.split("..")
    y1 = int(y1[2:])
    y2 = int(y2)

    if abs(y1) > abs(y2):
        y1, y2 = y2, y1

    if x1 > x2:
        x1, x2 = x2, x1
    return x1, x2, y1, y2

file = open("inputs/" + sys.argv[1], encoding="utf-8")

x1, x2, y1, y2 = parse_input(file.read())

maximal_x_steps = set()

for x in range(1, x2 + 1):
    dest = (x*(x+1)) // 2
    if x1<=dest<=x2:
        maximal_x_steps.add(x)

assert len(maximal_x_steps) > 0

max_init_y = abs(y2 + 1)
print(max_init_y*(max_init_y + 1) // 2)

min_x_vel = 0
max_x_vel = x2
min_y_vel = y2
max_y_vel = max_init_y

i = 1
while True:
    n = i*(i+1)//2
    if x1<=n<=x2:
        min_x_vel = i
        break
    i += 1

hits = 0
for x in range(min_x_vel, max_x_vel + 1):
    for y in range(min_y_vel, max_y_vel + 1):
        vx, vy = x, y
        px, py = 0,0
        while py >= y2:
            px += vx
            vx -= 1 if vx > 0 else 0
            py += vy
            vy -= 1
            if x1<=px<=x2 and y1>=py>=y2:
                hits += 1
                break

print(hits)

