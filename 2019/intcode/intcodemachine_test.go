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

func TestRunProgram_PositionModeEquals(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8})
	machine.SetInput(8)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_PositionModeNotEquals(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{0}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_ImmediateModeEquals(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 3, 1108, -1, 8, 3, 4, 3, 99})
	machine.SetInput(8)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_ImmediateModeNotEquals(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 3, 1108, -1, 8, 3, 4, 3, 99})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{0}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_PositionModeLessThan(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8})
	machine.SetInput(7)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_PositionModeNotLessThan(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{0}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_ImmediateModeLessThan(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 3, 1107, -1, 8, 3, 4, 3, 99})
	machine.SetInput(7)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_ImmediateModeNotLessThan(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 3, 1107, -1, 8, 3, 4, 3, 99})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{0}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_JumpPositionModeEqualZero(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9})
	machine.SetInput(0)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{0}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_JumpPositionModeNonZero(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_JumpImmediateModeEqualZero(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1})
	machine.SetInput(0)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{0}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_JumpImmediateModeNonZero(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_JumpEqualsLargeExampleBelowEight(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31,
		1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104,
		999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99})
	machine.SetInput(4)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{999}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_JumpEqualsLargeExampleEqualEight(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31,
		1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104,
		999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99})
	machine.SetInput(8)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1000}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_JumpEqualsLargeExampleGreaterThanEight(t *testing.T) {
	machine := NewIntcodeMachine()
	machine.LoadMemory([]int{3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31,
		1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104,
		999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99})
	machine.SetInput(42)
	machine.RunProgram()
	actual := machine.GetOutput()
	expected := []int{1001}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}
