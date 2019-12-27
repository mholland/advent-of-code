package main

import (
	"testing"
)

func TestFindBestMonitoringStationVisibleAsteroids_ExampleOne(t *testing.T) {
	pos, vis := findBestMonitoringStationVisibleAsteroids([]string{
		".#..#",
		".....",
		"#####",
		"....#",
		"...##",
	})

	if pos.x != 3 || pos.y != 4 || vis != 8 {
		t.Errorf("Expected 8 asteroids at 3, 4, but actuallpos.y: %v at %v, %v", vis, pos.x, pos.y)
	}
}

func TestFindBestMonitoringStationVisibleAsteroids_ExampleTwo(t *testing.T) {
	pos, vis := findBestMonitoringStationVisibleAsteroids([]string{
		"......#.#.",
		"#..#.#....",
		"..#######.",
		".#.#.###..",
		".#..#.....",
		"..#....#.#",
		"#..#....#.",
		".##.#..###",
		"##...#..#.",
		".#....####",
	})

	if pos.x != 5 || pos.y != 8 || vis != 33 {
		t.Errorf("Expected 33 asteroids at 5, 8, but actuallpos.y: %v at %v, %v", vis, pos.x, pos.y)
	}
}

func TestFindBestMonitoringStationVisibleAsteroids_ExampleThree(t *testing.T) {
	pos, vis := findBestMonitoringStationVisibleAsteroids([]string{
		"#.#...#.#.",
		".###....#.",
		".#....#...",
		"##.#.#.#.#",
		"....#.#.#.",
		".##..###.#",
		"..#...##..",
		"..##....##",
		"......#...",
		".####.###.",
	})

	if pos.x != 1 || pos.y != 2 || vis != 35 {
		t.Errorf("Expected 35 asteroids at 1, 2, but actuallpos.y: %v at %v, %v", vis, pos.x, pos.y)
	}
}

func TestFindBestMonitoringStationVisibleAsteroids_ExampleFour(t *testing.T) {
	pos, vis := findBestMonitoringStationVisibleAsteroids([]string{
		".#..#..###",
		"####.###.#",
		"....###.#.",
		"..###.##.#",
		"##.##.#.#.",
		"....###..#",
		"..#.#..#.#",
		"#..#.#.###",
		".##...##.#",
		".....#.#..",
	})

	if pos.x != 6 || pos.y != 3 || vis != 41 {
		t.Errorf("Expected 41 asteroids at 6, 3, but actuallpos.y: %v at %v, %v", vis, pos.x, pos.y)
	}
}

func TestFindBestMonitoringStationVisibleAsteroids_ExampleFive(t *testing.T) {
	pos, vis := findBestMonitoringStationVisibleAsteroids([]string{
		".#..##.###...#######",
		"##.############..##.",
		".#.######.########.#",
		".###.#######.####.#.",
		"#####.##.#.##.###.##",
		"..#####..#.#########",
		"####################",
		"#.####....###.#.#.##",
		"##.#################",
		"#####.##.###..####..",
		"..######..##.#######",
		"####.##.####...##..#",
		".#####..#.######.###",
		"##...#.##########...",
		"#.##########.#######",
		".####.#.###.###.#.##",
		"....##.##.###..#####",
		".#.#.###########.###",
		"#.#.#.#####.####.###",
		"###.##.####.##.#..##",
	})

	if pos.x != 11 || pos.y != 13 || vis != 210 {
		t.Errorf("Expected 210 asteroids at 11, 13, but actuallpos.y: %v at %v, %v", vis, pos.x, pos.y)
	}
}

func TestFindCoordinatesOf200thDestroyedAsteroid_Example(t *testing.T) {
	actual, _ := findCoordinatesOf200thDestroyedAsteroid([]string{
		".#..##.###...#######",
		"##.############..##.",
		".#.######.########.#",
		".###.#######.####.#.",
		"#####.##.#.##.###.##",
		"..#####..#.#########",
		"####################",
		"#.####....###.#.#.##",
		"##.#################",
		"#####.##.###..####..",
		"..######..##.#######",
		"####.##.####...##..#",
		".#####..#.######.###",
		"##...#.##########...",
		"#.##########.#######",
		".####.#.###.###.#.##",
		"....##.##.###..#####",
		".#.#.###########.###",
		"#.#.#.#####.####.###",
		"###.##.####.##.#..##",
	}, Pos{11, 13})

	if actual != 802 {
		t.Errorf("Expected coord: 802 actually %v", actual)
	}
}
