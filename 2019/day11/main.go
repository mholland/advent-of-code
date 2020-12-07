package main

import (
	"fmt"

	"github.com/mholland/advent-of-code/intcode"
)

func main() {
	program := intcode.ReadProgram("input.txt")
	painted := paintPanels(append([]int(nil), program...), make(map[pos]int))

	fmt.Printf("Painted number of panels: %v\n", len(painted))

	panels := make(map[pos]int)
	panels[pos{0, 0}] = white
	paintedReg := paintPanels(append([]int(nil), program...), panels)

	var minX, minY, maxX, maxY int
	for k := range paintedReg {
		if k.x < minX {
			minX = k.x
		}

		if k.x > maxX {
			maxX = k.x
		}

		if k.y < minY {
			minY = k.y
		}

		if k.y > maxY {
			maxY = k.y
		}
	}

	for y := maxY; y >= minY; y-- {
		var row string
		for x := minX; x <= maxX; x++ {
			if paintedReg[pos{x, y}] == 1 {
				row += "#"
				continue
			}
			row += " "
		}
		fmt.Println(row)
	}
}

func paintPanels(program []int, panels map[pos]int) map[pos]int {
	machine := intcode.NewIntcodeMachine()
	machine.LoadMemory(program)

	painted := panels
	currentDirection := up
	currentPos := pos{0, 0}

	for machine.State != intcode.Halted {

		machine.SetInput(painted[currentPos])
		machine.RunProgram()

		output := machine.GetOutput()
		colour := output[len(output)-2]
		turnDir := output[len(output)-1]
		painted[currentPos] = colour

		newDir := (currentDirection + 1) % 4
		if turnDir == 0 {
			newDir = (currentDirection + 3) % 4
		}

		currentDirection = newDir

		switch newDir {
		case up:
			currentPos.y++
		case right:
			currentPos.x++
		case down:
			currentPos.y--
		case left:
			currentPos.x--
		}
	}

	return painted
}

type pos struct {
	x, y int
}

const (
	black int = iota
	white
)

const (
	up int = iota
	right
	down
	left
)
