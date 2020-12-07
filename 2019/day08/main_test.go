package main

import (
	"reflect"
	"testing"
)

func TestParseLayers_Example(t *testing.T) {
	actual := parseLayers("123456789012", 3, 2)
	expected := [][]string{
		{"123", "456"},
		{"789", "012"},
	}

	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected %v but actual %v", expected, actual)
	}
}

func TestCalculateChecksum(t *testing.T) {
	actual := CalculateChecksum("114222000000", 3, 2)
	if actual != 6 {
		t.Errorf("Expected: %v actual: %v", 6, actual)
	}
}

func TestDecodeImage(t *testing.T) {
	actual := DecodeImage("0222112222120000", 2, 2)
	expected := []string{" #", "# "}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected %v actual %v", expected, actual)
	}
}
