package intcode

import (
	"io/ioutil"
	"log"
	"strconv"
	"strings"
	"unicode"
)

type Machine struct {
	memory []int
	input  int
	output []int
}

func NewIntcodeMachine() *Machine {
	return &Machine{
		memory: []int{},
	}
}

func (m *Machine) SetNounAndVerb(noun int, verb int) {
	m.memory[1] = noun
	m.memory[2] = verb
}

func (m *Machine) LoadMemory(memory []int) {
	m.memory = memory
	m.output = []int(nil)
	m.input = 0
}

func (m *Machine) RunProgram() {
	ip := 0
	for {
		instruction := parseInstruction(m.memory[ip])
		switch instruction.opCode {
		case 1:
			m.memory[m.memory[ip+3]] = m.getOperand(ip+1, instruction.parameterOneMode) + m.getOperand(ip+2, instruction.parameterTwoMode)
			ip += 4
		case 2:
			m.memory[m.memory[ip+3]] = m.getOperand(ip+1, instruction.parameterOneMode) * m.getOperand(ip+2, instruction.parameterTwoMode)
			ip += 4
		case 3:
			m.memory[m.memory[ip+1]] = m.input
			ip += 2
		case 4:
			m.output = append(m.output, m.getOperand(ip+1, instruction.parameterOneMode))
			ip += 2
		case 5:
			if m.getOperand(ip+1, instruction.parameterOneMode) != 0 {
				ip = m.getOperand(ip+2, instruction.parameterTwoMode)
				continue
			}
			ip += 3
		case 6:
			if m.getOperand(ip+1, instruction.parameterOneMode) == 0 {
				ip = m.getOperand(ip+2, instruction.parameterTwoMode)
				continue
			}
			ip += 3
		case 7:
			res := 0
			if m.getOperand(ip+1, instruction.parameterOneMode) < m.getOperand(ip+2, instruction.parameterTwoMode) {
				res = 1
			}
			m.memory[m.memory[ip+3]] = res
			ip += 4
		case 8:
			res := 0
			if m.getOperand(ip+1, instruction.parameterOneMode) == m.getOperand(ip+2, instruction.parameterTwoMode) {
				res = 1
			}
			m.memory[m.memory[ip+3]] = res
			ip += 4
		case 99:
			return
		}
	}
}

func (m *Machine) SetInput(input int) {
	m.input = input
}

func (m *Machine) GetOutput() []int {
	return m.output
}

func (m *Machine) getOperand(address int, mode int) int {
	switch mode {
	case immediateMode:
		return m.memory[address]
	default:
		return m.memory[m.memory[address]]
	}
}

func parseInstruction(instruction int) *Instruction {
	opCode := instruction % 100
	p1Mode := (instruction / 100) % 10
	p2Mode := (instruction / 1000) % 10
	p3Mode := (instruction / 10000) % 10

	return &Instruction{
		opCode:             opCode,
		parameterOneMode:   p1Mode,
		parameterTwoMode:   p2Mode,
		parameterThreeMode: p3Mode,
	}
}

type Instruction struct {
	opCode             int
	parameterOneMode   int
	parameterTwoMode   int
	parameterThreeMode int
}

const (
	positionMode int = iota
	immediateMode
)

func (m *Machine) GetMemory() []int {
	return m.memory
}

func ReadProgram(inputPath string) []int {
	data, err := ioutil.ReadFile(inputPath)
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

	return memory
}
