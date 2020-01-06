package main

import "testing"

func TestCalculateSystemEnergy(t *testing.T) {
	actual := calculateSystemEnergy([]string{
		"<x=-1, y=0, z=2>",
		"<x=2, y=-10, z=-7>",
		"<x=4, y=-8, z=8>",
		"<x=3, y=5, z=-1>",
	}, 10)

	expected := 179

	if expected != actual {
		t.Errorf("Expected energy %v, but actual %v", expected, actual)
	}
}

func TestCalculateStepsToPreviousPositions(t *testing.T) {
	actual := calculateStepsToPreviousPositions([]string{
		"<x=-1, y=0, z=2>",
		"<x=2, y=-10, z=-7>",
		"<x=4, y=-8, z=8>",
		"<x=3, y=5, z=-1>",
	})

	expected := 2772

	if expected != actual {
		t.Errorf("Expected steps %v, but actual %v", expected, actual)
	}
}

func TestCalculateStepsToPreviousPositions_LargerExample(t *testing.T) {
	actual := calculateStepsToPreviousPositions([]string{
		"<x=-8, y=-10, z=0>",
		"<x=5, y=5, z=10>",
		"<x=2, y=-7, z=3>",
		"<x=9, y=-8, z=-3>",
	})

	expected := 4686774924

	if expected != actual {
		t.Errorf("Expected steps %v, but actual %v", expected, actual)
	}
}

func TestCalculateSystemEnergy_LargerExample(t *testing.T) {
	actual := calculateSystemEnergy([]string{
		"<x=-8, y=-10, z=0>",
		"<x=5, y=5, z=10>",
		"<x=2, y=-7, z=3>",
		"<x=9, y=-8, z=-3>",
	}, 100)

	expected := 1940

	if expected != actual {
		t.Errorf("Expected energy %v, but actual %v", expected, actual)
	}
}
