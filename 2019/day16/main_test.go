package main

import (
	"reflect"
	"testing"
)

func TestGeneratePatternOnePhase(t *testing.T) {
	actual := generatePattern(10, 1)
	expected := []int{1, 0, -1, 0, 1, 0, -1, 0, 1, 0}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestGeneratePatternTwoPhase(t *testing.T) {
	actual := generatePattern(10, 2)
	expected := []int{0, 1, 1, 0, 0, -1, -1, 0, 0, 1}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestGeneratePatternThreePhase(t *testing.T) {
	actual := generatePattern(10, 3)
	expected := []int{0, 0, 1, 1, 1, 0, 0, 0, -1, -1}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestGeneratePatternSimpleExample(t *testing.T) {
	actual := applyFFT([]int{1, 2, 3, 4, 5, 6, 7, 8}, 4)[:8]

	expected := []int{0, 1, 0, 2, 9, 4, 9, 8}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestGeneratePatternLargerExampleOne(t *testing.T) {
	input := make([]int, 0)
	i := "80871224585914546619083218645595"
	for _, v := range i {
		input = append(input, int(v)-48)
	}
	actual := applyFFT(input, 100)

	expected := []int{2, 4, 1, 7, 6, 1, 7, 6}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestGeneratePatternLargerExampleTwo(t *testing.T) {
	input := make([]int, 0)
	i := "19617804207202209144916044189917"
	for _, v := range i {
		input = append(input, int(v)-48)
	}
	actual := applyFFT(input, 100)

	expected := []int{7, 3, 7, 4, 5, 4, 1, 8}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestGeneratePatternLargerExampleThree(t *testing.T) {
	input := make([]int, 0)
	i := "69317163492948606335995924319873"
	for _, v := range i {
		input = append(input, int(v)-48)
	}
	actual := applyFFT(input, 100)

	expected := []int{5, 2, 4, 3, 2, 1, 3, 3}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestMessageOffset(t *testing.T) {
	input := []int{1, 2, 3, 4, 5, 6, 7, 8, 9}
	actual := messageOffset(input)
	expected := 1234567

	if actual != expected {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestRepeat(t *testing.T) {
	input := []int{1, 2, 3}
	actual := repeat(input, 3)
	expected := []int{1, 2, 3, 1, 2, 3, 1, 2, 3}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestFindEmbeddedMessageExampleOne(t *testing.T) {
	input := make([]int, 0)
	i := "03036732577212944063491565474664"
	for _, v := range i {
		input = append(input, int(v)-48)
	}

	actual := findEmbeddedMessage(input)
	expected := []int{8, 4, 4, 6, 2, 0, 2, 6}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestFindEmbeddedMessageExampleTwo(t *testing.T) {
	input := make([]int, 0)
	i := "02935109699940807407585447034323"
	for _, v := range i {
		input = append(input, int(v)-48)
	}

	actual := findEmbeddedMessage(input)
	expected := []int{7, 8, 7, 2, 5, 2, 7, 0}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}

func TestFindEmbeddedMessageExampleThree(t *testing.T) {
	input := make([]int, 0)
	i := "03081770884921959731165446850517"
	for _, v := range i {
		input = append(input, int(v)-48)
	}

	actual := findEmbeddedMessage(input)
	expected := []int{5, 3, 5, 5, 3, 7, 3, 1}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected: %v, actually: %v", expected, actual)
	}
}
