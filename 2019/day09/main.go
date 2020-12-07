package main

import (
	"fmt"

	"github.com/mholland/advent-of-code/intcode"
)

func main() {
	program := intcode.ReadProgram("input.txt")

	machine := intcode.NewIntcodeMachine()

	machine.LoadMemory(append([]int(nil), program...))
	machine.SetInput(1)
	machine.RunProgram()

	fmt.Printf("BOOST output in test mode: %v\n", machine.GetOutput())

	machine.LoadMemory(append([]int(nil), program...))
	machine.SetInput(2)
	machine.RunProgram()
	fmt.Printf("BOOST output in sensor boost mode: %v\n", machine.GetOutput())
}
