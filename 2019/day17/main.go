package main

import (
	"fmt"
	"strings"

	"github.com/mholland/advent-of-code/2019/intcode"
)

const (
	north int = iota
	east
	south
	west
)

var directions = map[int]vec2{
	north: {0, -1},
	east:  {1, 0},
	south: {0, 1},
	west:  {-1, 0},
}

type robot struct {
	direction int
	pos       vec2
}

func (r *robot) moveForward(scaffold map[vec2]bool) bool {
	forward := directions[r.direction]
	forwardPos := vec2{r.pos.x + forward.x, r.pos.y + forward.y}
	if !scaffold[forwardPos] {
		return false
	}

	r.pos = forwardPos
	return true
}

func (r *robot) turnRight() {
	r.direction = (r.direction + 1) % 4
}

func (r *robot) turnLeft() {
	r.direction = (r.direction + 3) % 4
}

func (r *robot) canMoveRight(scaffold map[vec2]bool) bool {
	dir := directions[(r.direction+1)%4]
	right := vec2{r.pos.x + dir.x, r.pos.y + dir.y}
	return scaffold[right]
}

func (r *robot) canMoveLeft(scaffold map[vec2]bool) bool {
	dir := directions[(r.direction+3)%4]
	left := vec2{r.pos.x + dir.x, r.pos.y + dir.y}
	return scaffold[left]
}

func main() {
	machine := intcode.NewIntcodeMachine()
	program := intcode.ReadProgram("input.txt")
	initial := make([]int, len(program))
	copy(initial, program)
	machine.LoadMemory(initial)
	machine.RunProgram()

	output := machine.GetOutput()
	// fmt.Print(output)
	x, y := 0, 0
	scaffold := make(map[vec2]bool)
	var vacuum robot
	for _, char := range output {
		fmt.Print(string(rune(char)))
		c := byte(char)
		switch c {
		case '#':
			scaffold[vec2{x, y}] = true
			break
		case '^':
			scaffold[vec2{x, y}] = true
			vacuum = robot{direction: north, pos: vec2{x, y}}
			break
		case '>':
			scaffold[vec2{x, y}] = true
			vacuum = robot{direction: east, pos: vec2{x, y}}
			break
		case 'v':
			scaffold[vec2{x, y}] = true
			vacuum = robot{direction: south, pos: vec2{x, y}}
			break
		case '<':
			scaffold[vec2{x, y}] = true
			vacuum = robot{direction: west, pos: vec2{x, y}}
			break
		case '\n':
			y++
			x = -1
			break
		}
		x++
	}
	intersections := make([]int, 0)
	dirs := make([]vec2, 4)
	for _, d := range directions {
		dirs = append(dirs, d)
	}

	for vec := range scaffold {
		intersection := true
		for _, dir := range dirs {
			adj := vec2{vec.x + dir.x, vec.y + dir.y}
			if _, ok := scaffold[adj]; !ok {
				intersection = false
				break
			}
		}
		if intersection {
			intersections = append(intersections, vec.x*vec.y)
		}
	}
	sum := 0
	for _, val := range intersections {
		sum += val
	}

	fmt.Printf("Intersection checksum: %v\n", sum)
	fmt.Printf("Robot dir: %v\n", vacuum.direction)

	instructions := make([]string, 0)
	n := 0
	for {
		left := vacuum.canMoveLeft(scaffold)
		right := vacuum.canMoveRight(scaffold)

		if !left && !right {
			break
		}

		if left {
			vacuum.turnLeft()
			instructions = append(instructions, "L")
		}

		if right {
			vacuum.turnRight()
			instructions = append(instructions, "R")
		}

		for vacuum.moveForward(scaffold) {
			n++
		}
		instructions = append(instructions, fmt.Sprint(n))
		n = 0
	}
	fmt.Printf("%v\n", strings.Join(instructions, ","))
	// L,12,R,4,R,4,R,12,R,4,L,12,R,12,R,4,L,12,R,12,R,4,L,6,L,8,L,8,R,12,R,4,L,6,L,8,L,8,L,12,R,4,R,4,L,12,R,4,R,4,R,12,R,4,L,12,R,12,R,4,L,12,R,12,R,4,L,6,L,8,L,8

	traverse := make([]int, len(program))
	copy(traverse, program)
	machine.LoadMemory(traverse)
	machine.SetMemoryValue(0, 2)

	prog := make([]int, 0)
	prog = append(prog, generateInput("A,B,B,C,C,A,A,B,B,C")...)
	prog = append(prog, generateInput("L,12,R,4,R,4")...)
	prog = append(prog, generateInput("R,12,R,4,L,12")...)
	prog = append(prog, generateInput("R,12,R,4,L,6,L,8,L,8")...)
	prog = append(prog, int('n'), 10)

	machine.RunProgram()
	machine.SetInputs(prog)
	machine.RunProgram()

	fmt.Printf("Dust collected: %v\n", machine.GetLastOutput())
}

func generateInput(routine string) []int {
	input := make([]int, 0)
	for _, cmd := range routine {
		input = append(input, int(cmd))
	}
	input = append(input, 10)
	return input
}

type vec2 struct {
	x, y int
}
