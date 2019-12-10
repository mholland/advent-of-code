package main

import (
	"fmt"
	"github.com/mholland/advent-of-code/2019/intcode"
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
	fmt.Println("Part 2 noun", noun, "verb", verb, ",", 100*noun+verb)
}

func run1202AlarmState(memory []int) int {
	programInput := append([]int(nil), memory...)
	machine := intcode.NewIntcodeMachine()
	machine.LoadMemory(programInput)
	machine.SetNounAndVerb(12, 2)

	machine.RunProgram()
	return machine.GetMemory()[0]
}

func findMatchingNounAndVerb(memory []int) (int, int) {
	machine := intcode.NewIntcodeMachine()
	for i := 0; i <= 99; i++ {
		for j := 0; j <= 99; j++ {
			input := append([]int(nil), memory...)
			machine.LoadMemory(input)
			machine.SetNounAndVerb(i, j)
			machine.RunProgram()
			if machine.GetMemory()[0] == 19690720 {
				return i, j
			}
		}
	}

	return -1, -1
}
