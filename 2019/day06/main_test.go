package main

import "testing"

func TestCountOrbits_Example(t *testing.T) {
	actual := CountOrbits([]string{
		"COM)B",
		"B)C",
		"C)D",
		"D)E",
		"E)F",
		"B)G",
		"G)H",
		"D)I",
		"E)J",
		"J)K",
		"K)L",
	})

	if actual != 42 {
		t.Errorf("Expected %v but actual %v", 42, actual)
	}
}

func TestCalculateOrbitalTransfers_Example(t *testing.T) {
	actual, _ := CalculateOrbitalTransfers([]string{
		"COM)B",
		"B)C",
		"C)D",
		"D)E",
		"E)F",
		"B)G",
		"G)H",
		"D)I",
		"E)J",
		"J)K",
		"K)L",
		"K)YOU",
		"I)SAN",
	}, "YOU", "SAN")

	if actual != 4 {
		t.Errorf("Expected %v but actual %v", 4, actual)
	}
}
