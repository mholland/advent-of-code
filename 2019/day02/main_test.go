package main

import (
	"reflect"
	"testing"
)

func TestRunProgram_SingleStatementAddition(t *testing.T) {
	actual := RunProgram([]int{1, 0, 0, 0, 99})
	expected := []int{2, 0, 0, 0, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_SingleStatementMultiplication(t *testing.T) {
	actual := RunProgram([]int{2, 3, 0, 3, 99})
	expected := []int{2, 3, 0, 6, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_LongerStatementMultiplication(t *testing.T) {
	actual := RunProgram([]int{2, 4, 4, 5, 99, 0})
	expected := []int{2, 4, 4, 5, 99, 9801}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_LongerStatementAddition(t *testing.T) {
	actual := RunProgram([]int{1, 1, 1, 4, 99, 5, 6, 0, 99})
	expected := []int{30, 1, 1, 4, 2, 5, 6, 0, 99}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}

func TestRunProgram_ExampleProgramOne(t *testing.T) {
	actual := RunProgram([]int{1, 9, 10, 3, 2, 3, 11, 0, 99, 30, 40, 50})
	expected := []int{3500, 9, 10, 70, 2, 3, 11, 0, 99, 30, 40, 50}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Output incorrect, expected: %v but actual: %v", expected, actual)
	}
}
