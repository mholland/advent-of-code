package main

import (
	"fmt"
	"math"

	"github.com/mholland/advent-of-code/2019/intcode"
)

func main() {
	machine := intcode.NewIntcodeMachine()
	masterProgram := intcode.ReadProgram("input.txt")

	beamMap := make(map[vec2]bool)
	beamLocs := 0
	for y := 0; y < 50; y++ {
		for x := 0; x < 50; x++ {
			if isWithinBeam(machine, masterProgram, x, y) {
				beamMap[vec2{x, y}] = true
				beamLocs++
			}
		}
	}
	fmt.Printf("Locations within beam: %v\n", beamLocs)

	shipSize := 100
	dx := 0
	dy := 48
	for x := 0; x < 50; x++ {
		if beamMap[vec2{x, dy}] && x > dx {
			dx = x
		}
	}

	// Roughly follow the line of the right side of the beam
	grad := float64(dx) / float64(dy)
	for y := 0; y < 10000; y++ {
		topRight := int(math.Ceil(float64(y) * grad))
		for !isWithinBeam(machine, masterProgram, topRight, y) && topRight > 0 {
			topRight--
		}
		topLeftX := topRight - (shipSize - 1)
		bottomY := shipSize - 1
		if !isWithinBeam(machine, masterProgram, topLeftX, y) {
			continue
		}
		if !isWithinBeam(machine, masterProgram, topLeftX, y+bottomY) {
			continue
		}
		if !isWithinBeam(machine, masterProgram, topRight, y+bottomY) {
			continue
		}
		fmt.Printf("%d, %d, %d\n", topLeftX, y, topLeftX*10000+y)
		break
	}
}

func isWithinBeam(machine *intcode.Machine, program []int, x, y int) bool {
	if x < 0 || y < 0 {
		return false
	}
	prog := make([]int, len(program))
	copy(prog, program)

	machine.LoadMemory(prog)
	machine.SetInputs([]int{x, y})
	machine.RunProgram()
	return machine.GetLastOutput() == 1
}

type vec2 struct {
	x, y int
}
