package main

import (
	"fmt"
	"math"
	"time"

	"github.com/mholland/advent-of-code/intcode"
)

func main() {
	minPath, area := findDroid()
	var droidPos vec2
	for k := range area {
		if area[k] == droid {
			droidPos = k
		}
	}
	fmt.Printf("Steps to the droid %v\n", minPath)
	fmt.Printf("Droid pos %v,%v\n", droidPos.x, droidPos.y)
	drawArea(area)
	minutes := fillWithOxygen(area, droidPos)
	fmt.Printf("Minutes to fill with oxygen: %v\n", minutes)
}

func fillWithOxygen(area map[vec2]int, droidPos vec2) int {
	toFill := []vec2{droidPos}
	filled := map[vec2]bool{}
	minutes := 0

	for len(toFill) > 0 {
		nextToFill := []vec2(nil)
		for _, pos := range toFill {
			filled[pos] = true
			area[pos] = oxygen

			for d := range directions {
				adjacent := pos.add(directions[d])
				if !filled[adjacent] && area[adjacent] == movable {
					nextToFill = append(nextToFill, adjacent)
				}
			}
		}
		if len(nextToFill) > 0 {
			minutes++
		}
		toFill = nextToFill
		drawArea(area)
		time.Sleep(250 * time.Millisecond)
	}
	return minutes
}

func drawArea(area map[vec2]int) {
	var minHeight, maxHeight, minWidth, maxWidth int
	for pos := range area {
		minHeight = min(minHeight, pos.y)
		maxHeight = max(maxHeight, pos.y)
		minWidth = min(minWidth, pos.x)
		maxWidth = max(maxWidth, pos.x)
	}

	for y := maxHeight; y > minHeight-1; y-- {
		for x := minWidth; x < maxWidth+1; x++ {
			if x == 0 && y == 0 {
				fmt.Print("^")
				continue
			}
			if val, ok := area[vec2{x, y}]; ok {
				switch val {
				case wall:
					fmt.Print("#")
					break
				case movable:
					fmt.Print(".")
					break
				case droid:
				case oxygen:
					fmt.Print("O")
					break
				}
				continue
			}

			fmt.Print("#")
		}
		fmt.Println()
	}
}

func min(x, y int) int {
	if x < y {
		return x
	}
	return y
}

func max(x, y int) int {
	if x > y {
		return x
	}
	return y
}

func findDroid() (int, map[vec2]int) {
	program := intcode.ReadProgram("input.txt")
	machine := intcode.NewIntcodeMachine()

	path := newStack()
	path.push(0)

	origin := vec2{0, 0}
	currentPosition := origin
	visited := map[vec2]int{}

	minPath := math.MaxFloat64
	machine.LoadMemory(append([]int(nil), program...))
	machine.RunProgram()

	for {
		hasMoved := false
		for direction, vector := range directions {
			position := currentPosition.add(vector)
			if _, exists := visited[position]; exists {
				continue
			}
			moved, result, newPosition := tryMove(machine, currentPosition, direction)
			hasMoved = moved
			visited[position] = result
			if !hasMoved {
				continue
			}

			if result == droid {
				minPath = math.Min(float64(len(*path)), minPath)
			}

			currentPosition = newPosition
			path.push(direction)
			break
		}

		if hasMoved {
			continue
		}

		hasVal, d := path.pop()
		if !hasVal {
			break
		}
		_, _, currentPosition = tryMove(machine, currentPosition, reverse[d])
	}

	return int(minPath), visited
}

func tryMove(machine *intcode.Machine, currentPosition vec2, direction int) (moved bool, result int, newPosition vec2) {
	machine.SetInput(direction)
	machine.RunProgram()
	result = machine.GetLastOutput()
	moved = result != wall
	if moved {
		newPosition = currentPosition.add(directions[direction])
	}
	return
}

const (
	north int = 1
	south int = 2
	west  int = 3
	east  int = 4
)

const (
	wall = iota
	movable
	droid
	oxygen
)

var directions = map[int]vec2{
	north: {0, 1},
	south: {0, -1},
	west:  {-1, 0},
	east:  {1, 0},
}
var reverse = map[int]int{
	north: south,
	south: north,
	west:  east,
	east:  west,
}

type vec2 struct {
	x, y int
}

func (vec vec2) add(to vec2) vec2 {
	return vec2{
		vec.x + to.x,
		vec.y + to.y,
	}
}

type stack []int

func newStack() *stack {
	return &stack{}
}

func (s *stack) push(v int) {
	*s = append(*s, v)
}

func (s *stack) pop() (bool, int) {
	if len(*s) == 0 {
		return false, 0
	}
	idx := len(*s) - 1
	val := (*s)[idx]
	*s = (*s)[:idx]
	return true, val
}

func (s *stack) peek() (bool, int) {
	if len(*s) == 0 {
		return false, 0
	}
	return true, (*s)[len(*s)-1]
}
