package intcode

import (
	"io/ioutil"
	"log"
	"strconv"
	"strings"
	"unicode"
)

/*Machine represents an entire Intcode machine*/
type Machine struct {
	memory             []int
	inputs             []int
	inputPointer       int
	outputs            []int
	State              State
	ip                 int
	relativeBaseOffset int
}

/*NewIntcodeMachine creates a newly initialised machine to perform operations on*/
func NewIntcodeMachine() *Machine {
	return &Machine{
		memory:             []int(nil),
		inputs:             []int(nil),
		inputPointer:       0,
		outputs:            []int(nil),
		State:              Initialised,
		ip:                 0,
		relativeBaseOffset: 0,
	}
}

/*SetNounAndVerb initialised the second and third memory values to provided arguments*/
func (m *Machine) SetNounAndVerb(noun int, verb int) {
	m.memory[1] = noun
	m.memory[2] = verb
}

/*LoadMemory initialises the machine memory to the provided program*/
func (m *Machine) LoadMemory(memory []int) {
	m.memory = memory
	m.outputs = []int(nil)
	m.inputs = []int(nil)
	m.inputPointer = 0
	m.ip = 0
	m.State = Initialised
	m.relativeBaseOffset = 0
}

/*RunProgram executes the program currently loaded into memory*/
func (m *Machine) RunProgram() {
	m.State = Running
	for {
		instruction := parseInstruction(m.memory[m.ip])
		switch instruction.opCode {
		case 1:
			*m.getTargetAddress(m.ip+3, instruction.parameterThreeMode) = m.getOperand(m.ip+1, instruction.parameterOneMode) + m.getOperand(m.ip+2, instruction.parameterTwoMode)
			m.ip += 4
		case 2:
			*m.getTargetAddress(m.ip+3, instruction.parameterThreeMode) = m.getOperand(m.ip+1, instruction.parameterOneMode) * m.getOperand(m.ip+2, instruction.parameterTwoMode)
			m.ip += 4
		case 3:
			if m.inputPointer >= len(m.inputs) {
				m.State = AwaitingInput
				return
			}
			*m.getTargetAddress(m.ip+1, instruction.parameterOneMode) = m.inputs[m.inputPointer]
			m.inputPointer++
			m.ip += 2
		case 4:
			m.outputs = append(m.outputs, m.getOperand(m.ip+1, instruction.parameterOneMode))
			m.ip += 2
		case 5:
			if m.getOperand(m.ip+1, instruction.parameterOneMode) != 0 {
				m.ip = m.getOperand(m.ip+2, instruction.parameterTwoMode)
				continue
			}
			m.ip += 3
		case 6:
			if m.getOperand(m.ip+1, instruction.parameterOneMode) == 0 {
				m.ip = m.getOperand(m.ip+2, instruction.parameterTwoMode)
				continue
			}
			m.ip += 3
		case 7:
			res := 0
			if m.getOperand(m.ip+1, instruction.parameterOneMode) < m.getOperand(m.ip+2, instruction.parameterTwoMode) {
				res = 1
			}
			*m.getTargetAddress(m.ip+3, instruction.parameterThreeMode) = res
			m.ip += 4
		case 8:
			res := 0
			if m.getOperand(m.ip+1, instruction.parameterOneMode) == m.getOperand(m.ip+2, instruction.parameterTwoMode) {
				res = 1
			}
			*m.getTargetAddress(m.ip+3, instruction.parameterThreeMode) = res
			m.ip += 4
		case 9:
			m.relativeBaseOffset += m.getOperand(m.ip+1, instruction.parameterOneMode)
			m.ip += 2
		case 99:
			m.State = Halted
			return
		}
	}
}

func (m *Machine) getTargetAddress(address int, parameterMode int) *int {

	addr := m.memory[address]

	if parameterMode == relativeMode {
		addr = m.relativeBaseOffset + m.memory[address]
	}

	if addr >= len(m.memory) {
		m.expandMemory(addr + 1)
	}

	return &m.memory[addr]
}

func (m *Machine) getOperand(address int, mode int) int {
	switch mode {
	case immediateMode:
		return m.memory[address]
	case relativeMode:
		return m.memory[m.relativeBaseOffset+m.memory[address]]
	default:
		return m.memory[m.memory[address]]
	}
}

func (m *Machine) expandMemory(size int) {
	newSlice := make([]int, size, size)
	copy(newSlice, m.memory)
	m.memory = newSlice
}

/*SetInput provides a single input to be used in conjunction with opcode 3*/
func (m *Machine) SetInput(input int) {
	m.inputs = append(m.inputs, input)
}

/*SetInputs provides multiple inputs to be used in conjunction with opcode 3*/
func (m *Machine) SetInputs(inputs []int) {
	m.inputs = inputs
}

/*GetOutput returns all outputs from the program using opcode 4*/
func (m *Machine) GetOutput() []int {
	return m.outputs
}

/*GetLastOutput returns the last output from the program using opcode 4*/
func (m *Machine) GetLastOutput() int {
	return m.outputs[len(m.outputs)-1]
}

func parseInstruction(inst int) *instruction {
	opCode := inst % 100
	p1Mode := (inst / 100) % 10
	p2Mode := (inst / 1000) % 10
	p3Mode := (inst / 10000) % 10

	return &instruction{
		opCode:             opCode,
		parameterOneMode:   p1Mode,
		parameterTwoMode:   p2Mode,
		parameterThreeMode: p3Mode,
	}
}

type instruction struct {
	opCode             int
	parameterOneMode   int
	parameterTwoMode   int
	parameterThreeMode int
}

const (
	positionMode int = iota
	immediateMode
	relativeMode
)

/*State represents the current states of the Intcode machine*/
type State int

/*All available states an Intcode machine can be in*/
const (
	Initialised State = iota
	Running
	AwaitingInput
	Halted
)

/*GetMemory returns the current state of memory within the Intcode machine*/
func (m *Machine) GetMemory() []int {
	return m.memory
}

/*ReadProgram accepts an input path on disk and reads the program into an int slice*/
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
