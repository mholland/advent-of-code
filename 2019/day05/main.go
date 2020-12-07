package main

import (
	"fmt"

	"github.com/mholland/advent-of-code/intcode"
)

func main() {
	memory := intcode.ReadProgram("input.txt")

	machine := intcode.NewIntcodeMachine()
	machine.LoadMemory(append([]int(nil), memory...))
	machine.SetInput(1)
	machine.RunProgram()

	fmt.Printf("Diagnostic code from TEST input 1: %v\n", machine.GetLastOutput())

	machine.LoadMemory(append([]int(nil), memory...))
	machine.SetInput(5)
	machine.RunProgram()
	fmt.Printf("Diagnostic code from TEST input 5: %v\n", machine.GetLastOutput())
}
