package main

import (
	"fmt"

	"github.com/mholland/advent-of-code/2019/intcode"
)

func main() {
	memory := intcode.ReadProgram("input.txt")

	machine := intcode.NewIntcodeMachine()
	machine.LoadMemory(append([]int(nil), memory...))
	machine.SetInput(1)
	machine.RunProgram()
	output := machine.GetOutput()

	fmt.Printf("Diagnostic code from TEST input 1: %v\n", output[len(output)-1])

	machine.LoadMemory(append([]int(nil), memory...))
	machine.SetInput(5)
	machine.RunProgram()
	output = machine.GetOutput()
	fmt.Printf("Diagnostic code from TEST input 5: %v\n", output[len(output)-1])
}
