package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"strconv"
	"strings"
	"unicode"
)

func main() {
	data, err := ioutil.ReadFile("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	split := strings.FieldsFunc(string(data), func(r rune) bool {
		return !unicode.IsDigit(r)
	})

	memory := make([]int, 0, len(split))
	for _, val := range split {
		intVal, err := strconv.Atoi(val)
		if err != nil {
			log.Fatal(err)
		}

		memory = append(memory, intVal)
	}

	fmt.Println("Part 1 End program position 0:", run1202AlarmState(memory))
	noun, verb := findMatchingNounAndVerb(memory)
	fmt.Println("Part 2 noun", noun, "verb", verb)
}

func findMatchingNounAndVerb(memory []int) (int, int) {
	for i := 0; i <= 99; i++ {
		for j := 0; j <= 99; j++ {
			input := append([]int(nil), memory...)
			input = setNounAndVerb(input, i, j)
			output := RunProgram(input)
			if output[0] == 19690720 {
				return i, j
			}
		}
	}

	return -1, -1
}

func run1202AlarmState(memory []int) int {
	programInput := append([]int(nil), memory...)
	programInput = setNounAndVerb(programInput, 12, 2)

	output := RunProgram(programInput)

	return output[0]
}

func setNounAndVerb(memory []int, noun int, verb int) []int {
	memory[1] = noun
	memory[2] = verb

	return memory
}

/*
RunProgram accepts program input, running the IntCode algorithm against it and returns the output
*/
func RunProgram(vm []int) []int {
	cursor := 0
	for {
		opCode := vm[cursor]
		if opCode == 99 {
			break
		}
		operand1Location := vm[cursor+1]
		operand2Location := vm[cursor+2]
		targetLocation := vm[cursor+3]
		if opCode == 1 {
			vm[targetLocation] = vm[operand1Location] + vm[operand2Location]
		} else {
			vm[targetLocation] = vm[operand1Location] * vm[operand2Location]
		}
		cursor += 4
	}

	return vm
}
