package main

import (
	"testing"
)

func TestExampleOne(t *testing.T) {
	actual := collectAllKeys([]string{
		"#########",
		"#b.A.@.a#",
		"#########",
	})

	if actual != 8 {
		t.Errorf("Expected 8, actually: %d", actual)
	}
}

func TestExampleTwo(t *testing.T) {
	actual := collectAllKeys([]string{
		"########################",
		"#f.D.E.e.C.b.A.@.a.B.c.#",
		"######################.#",
		"#d.....................#",
		"########################",
	})

	if actual != 86 {
		t.Errorf("Expected 86, actually: %d", actual)
	}
}

func TestExampleThree(t *testing.T) {
	actual := collectAllKeys([]string{
		"########################",
		"#...............b.C.D.f#",
		"#.######################",
		"#.....@.a.B.c.d.A.e.F.g#",
		"########################",
	})

	if actual != 132 {
		t.Errorf("Expected 132, actually: %d", actual)
	}
}

func TestExampleFour(t *testing.T) {
	actual := collectAllKeys([]string{
		"#################",
		"#i.G..c...e..H.p#",
		"########.########",
		"#j.A..b...f..D.o#",
		"########@########",
		"#k.E..a...g..B.n#",
		"########.########",
		"#l.F..d...h..C.m#",
		"#################",
	})

	if actual != 136 {
		t.Errorf("Expected 136, actually: %d", actual)
	}
}

func TestExampleFive(t *testing.T) {
	actual := collectAllKeys([]string{
		"########################",
		"#@..............ac.GI.b#",
		"###d#e#f################",
		"###A#B#C################",
		"###g#h#i################",
		"########################",
	})

	if actual != 81 {
		t.Errorf("Expected 81, actually: %d", actual)
	}
}
