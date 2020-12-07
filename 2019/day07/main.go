package main

import (
	"fmt"

	"github.com/mholland/advent-of-code/intcode"
)

func main() {
	program := intcode.ReadProgram("input.txt")
	max := maximisePhaseSettingSequence(program)

	fmt.Printf("Highest signal: %v\n", max)
	fmt.Printf("Highest amplified signal %v", maximiseAmplification(program))
}

// https://en.wikipedia.org/wiki/Heap%27s_algorithm
func phaseSettingCombinations(n int, arr []int) [][]int {
	var result [][]int

	c := make([]int, n)
	result = append(result, copy(arr))

	i := 0
	for i < n {
		if c[i] < i {
			if i%2 == 0 {
				arr[0], arr[i] = arr[i], arr[0]
			} else {
				arr[c[i]], arr[i] = arr[i], arr[c[i]]
			}
			result = append(result, copy(arr))

			c[i]++
			i = 0
		} else {
			c[i] = 0
			i++
		}
	}

	return result
}

func maximisePhaseSettingSequence(program []int) int {
	combinations := phaseSettingCombinations(5, []int{0, 1, 2, 3, 4})
	machine := intcode.NewIntcodeMachine()

	max := 0

	for _, phaseSettings := range combinations {
		output := 0
		for _, setting := range phaseSettings {
			machine.LoadMemory(copy(program))
			machine.SetInputs([]int{setting, output})
			machine.RunProgram()
			output = machine.GetOutput()[0]
		}
		if output > max {
			max = output
		}
	}

	return max
}

func maximiseAmplification(program []int) int {
	phaseSettingCombos := phaseSettingCombinations(5, []int{5, 6, 7, 8, 9})
	maxOutput := 0
	for _, phaseSettings := range phaseSettingCombos {
		var machines [5]*intcode.Machine
		for i, setting := range phaseSettings {
			machine := intcode.NewIntcodeMachine()
			machine.LoadMemory(copy(program))
			machine.SetInput(setting)
			machines[i] = machine
		}
		machines[0].SetInput(0)

		for machines[4].State != intcode.Halted {
			for i := 0; i < len(machines); i++ {
				m := machines[i]
				m.RunProgram()
				out := m.GetLastOutput()
				machines[(i+1)%5].SetInput(out)
			}
		}
		output := machines[4].GetLastOutput()

		if output > maxOutput {
			maxOutput = output
		}
	}

	return maxOutput
}

func copy(arr []int) []int {
	return append([]int(nil), arr...)
}
