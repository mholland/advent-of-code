package intcode

import (
	"reflect"
	"testing"
)

func TestRunProgram_SingleStatementAddition(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{1, 0, 0, 0, 99})
	machine.RunProgram()
	actual := machine.GetMemory()
	expected := []int{2, 0, 0, 0, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_SingleStatementMultiplication(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{2, 3, 0, 3, 99})
	machine.RunProgram()
	actual := machine.GetMemory()
	expected := []int{2, 3, 0, 6, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_LongerStatementMultiplication(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{2, 4, 4, 5, 99, 0})
	machine.RunProgram()
	actual := machine.GetMemory()
	expected := []int{2, 4, 4, 5, 99, 9801}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_LongerStatementAddition(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{1, 1, 1, 4, 99, 5, 6, 0, 99})
	machine.RunProgram()
	actual := machine.GetMemory()
	expected := []int{30, 1, 1, 4, 2, 5, 6, 0, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_ExampleProgramOne(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{1, 9, 10, 3, 2, 3, 11, 0, 99, 30, 40, 50})
	machine.RunProgram()
	actual := machine.GetMemory()
	expected := []int{3500, 9, 10, 70, 2, 3, 11, 0, 99, 30, 40, 50}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_ParameterModeInstructions(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{1002, 4, 3, 4, 33})
	machine.RunProgram()
	actual := machine.GetMemory()
	expected := []int{1002, 4, 3, 4, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_InputOutputOpCodes(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 0, 4, 0, 99})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{42}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_NegativeParameterValues(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{1101, 100, -1, 4, 0})
	machine.RunProgram()
	actual := machine.GetMemory()
	expected := []int{1101, 100, -1, 4, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}
