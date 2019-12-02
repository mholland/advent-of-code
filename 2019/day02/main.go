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

	vm := make([]int, 0, len(split))
	for _, val := range split {
		intVal, err := strconv.Atoi(val)
		if err != nil {
			log.Fatal(err)
		}

		vm = append(vm, intVal)
	}

	vm[1] = 12
	vm[2] = 2

	output := RunProgram(vm)

	fmt.Println("End program position 0:", output[0])
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
