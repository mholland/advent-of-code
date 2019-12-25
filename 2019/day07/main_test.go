package main

import (
	"reflect"
	"testing"
)

func TestMaximiseAmplification_ExampleTwo(t *testing.T) {
	actual := MaximiseAmplification([]int{3, 52, 1001, 52, -5, 52, 3, 53, 1, 52, 56, 54, 1007, 54, 5, 55, 1005, 55, 26, 1001, 54,
		-5, 54, 1105, 1, 12, 1, 53, 54, 53, 1008, 54, 0, 55, 1001, 55, 1, 55, 2, 53, 55, 53, 4,
		53, 1001, 56, -1, 56, 1005, 56, 6, 99, 0, 0, 0, 0, 10})
	expected := 18216
	if actual != expected {
		t.Errorf("Expected %v actual %v", expected, actual)
	}
}

func TestMaximiseAmplification_ExampleOne(t *testing.T) {
	actual := MaximiseAmplification([]int{3, 26, 1001, 26, -4, 26, 3, 27, 1002, 27, 2, 27, 1, 27, 26,
		27, 4, 27, 1001, 28, -1, 28, 1005, 28, 6, 99, 0, 0, 5})
	expected := 139629729
	if actual != expected {
		t.Errorf("Expected %v actual %v", expected, actual)
	}
}

func TestMaximisePhaseSettingSequence_ExampleOne(t *testing.T) {
	actual := MaximisePhaseSettingSequence([]int{3, 15, 3, 16, 1002, 16, 10, 16, 1, 16, 15, 15, 4, 15, 99, 0, 0})

	if actual != 43210 {
		t.Errorf("Expected %v actually %v", 43210, actual)
	}
}

func TestMaximisePhaseSettingSequence_ExampleTwo(t *testing.T) {
	actual := MaximisePhaseSettingSequence([]int{3, 23, 3, 24, 1002, 24, 10, 24, 1002, 23, -1, 23,
		101, 5, 23, 23, 1, 24, 23, 23, 4, 23, 99, 0, 0})

	if actual != 54321 {
		t.Errorf("Expected %v actually %v", 54321, actual)
	}
}

func TestMaximisePhaseSettingSequence_ExampleThree(t *testing.T) {
	actual := MaximisePhaseSettingSequence([]int{3, 31, 3, 32, 1002, 32, 10, 32, 1001, 31, -2, 31, 1007, 31, 0, 33,
		1002, 33, 7, 33, 1, 33, 31, 31, 1, 32, 31, 31, 4, 31, 99, 0, 0, 0})

	if actual != 65210 {
		t.Errorf("Expected %v actually %v", 54321, actual)
	}
}

func TestPhaseSettingCombinations_ThreeDigits(t *testing.T) {
	actual := phaseSettingCombinations(3, []int{1, 2, 3})

	if len(actual) != 6 {
		t.Errorf("Expected %v combinations actually %v", 6, actual)
	}

	expected := [][]int{
		[]int{1, 2, 3},
		[]int{2, 1, 3},
		[]int{3, 1, 2},
		[]int{1, 3, 2},
		[]int{2, 3, 1},
		[]int{3, 2, 1},
	}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected permutations %v but actual %v", expected, actual)
	}
}
