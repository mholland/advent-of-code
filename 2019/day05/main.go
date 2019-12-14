package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"strconv"
	"strings"
	"unicode"

	"github.com/mholland/advent-of-code/2019/intcode"
)

func main() {
	data, err := ioutil.ReadFile("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	split := strings.FieldsFunc(string(data), func(r rune) bool {
		return !unicode.IsDigit(r) && r != '-'
	})

	memory := make([]int, 0, len(split))
	for _, val := range split {
		intVal, err := strconv.Atoi(val)
		if err != nil {
			log.Fatal(err)
		}

		memory = append(memory, intVal)
	}

	machine := intcode.NewIntcodeMachine()
	machine.LoadMemory(memory)
	machine.SetInput(1)
	machine.RunProgram()
	output := machine.GetOutput()

	fmt.Printf("Diagnostic code from TEST: %v\n", output[len(output)-1])
}
