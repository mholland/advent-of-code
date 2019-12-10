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

func (m *Machine) RunProgram() {
	ip := 0
	for {
		opCode := m.memory[ip]
		switch opCode {
		case 1:
			m.memory[m.memory[ip+3]] = m.memory[m.memory[ip+1]] + m.memory[m.memory[ip+2]]
		case 2:
			m.memory[m.memory[ip+3]] = m.memory[m.memory[ip+1]] * m.memory[m.memory[ip+2]]
		case 99:
			return
		}
		ip += 4
	}
}

func (m *Machine) GetMemory() []int {
	return m.memory
}
