package intcode

type Machine struct {
	memory []int
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
}

func (m *Machine) RunProgram() []int {
	instructionPointer := 0
	for {
		opCode := m.memory[instructionPointer]
		if opCode == 99 {
			break
		}
		parameterOne := m.memory[instructionPointer+1]
		parameterTwo := m.memory[instructionPointer+2]
		parameterThree := m.memory[instructionPointer+3]
		if opCode == 1 {
			m.memory[parameterThree] = m.memory[parameterOne] + m.memory[parameterTwo]
		} else {
			m.memory[parameterThree] = m.memory[parameterOne] * m.memory[parameterTwo]
		}
		instructionPointer += 4
	}

	return m.memory
}
