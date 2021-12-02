instructions = [l.split() for l in open("inputs/day02.input")]
depth_init = 0
horizontal = 0
depth = 0
aim = 0

for plane, dist in instructions:
    dist = int(dist)
    if plane == "forward":
        horizontal += dist
        depth += aim * dist
    if plane == "up":
        depth_init -= dist
        aim -= dist
    if plane == "down":
        depth_init += dist
        aim += dist

print(f"{horizontal*depth_init}")
print(f"{horizontal*depth}")
