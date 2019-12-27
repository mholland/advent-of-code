package main

import (
	"bufio"
	"errors"
	"fmt"
	"log"
	"math"
	"os"
	"sort"
)

func main() {
	file, err := os.Open("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	input := []string(nil)
	for scanner.Scan() {
		input = append(input, scanner.Text())
	}

	pos, visible := findBestMonitoringStationVisibleAsteroids(input)
	fmt.Printf("Most asteroids visible at coords: %v, %v with visible: %v\n", pos.x, pos.y, visible)

	coords, err := findCoordinatesOf200thDestroyedAsteroid(input, pos)
	if err != nil {
		panic(err)
	}

	fmt.Printf("Coords of 200th asteroid vaporised %v", coords)
}

func findBestMonitoringStationVisibleAsteroids(input []string) (Pos, int) {
	asteroids := mapAsteroids(input)

	max, pos := 0, Pos{0, 0}
	for _, potentialStation := range asteroids {
		distinctGrads := make(map[Pos]int)
		for _, other := range asteroids {
			if other == potentialStation {
				continue
			}

			diff := Pos{potentialStation.pos.x - other.pos.x,
				potentialStation.pos.y - other.pos.y}

			distinctGrads[diff.getDiff()]++
		}

		if len(distinctGrads) > max {
			max = len(distinctGrads)
			pos = potentialStation.pos
		}
	}

	return pos, max
}

func findCoordinatesOf200thDestroyedAsteroid(input []string, turretPos Pos) (int, error) {
	asteroids := mapAsteroids(input)
	byAngle := make(map[float64][]Asteroid)

	for _, asteroid := range asteroids {
		if asteroid.pos == turretPos {
			continue
		}

		angle := asteroid.angle(turretPos)
		byAngle[angle] = append(byAngle[angle], asteroid)
	}

	for _, as := range byAngle {
		sort.Slice(as, func(i, j int) bool {
			return as[i].distanceFrom(turretPos) < as[j].distanceFrom(turretPos)
		})
	}

	angles := make([]float64, 0, len(byAngle))
	for k := range byAngle {
		angles = append(angles, k)
	}

	sort.Float64s(angles)

	destroyed := 0
	for destroyed < 200 {
		for _, a := range angles {
			if len(byAngle[a]) == 0 {
				continue
			}
			destroyed++
			if destroyed == 200 {
				return byAngle[a][0].pos.x*100 + byAngle[a][0].pos.y, nil
			}
			byAngle[a] = byAngle[a][1:]
		}
	}

	return -1, errors.New("Failed to find the 200th asteroid")
}

func gcd(x, y int) int {
	for y != 0 {
		x, y = y, x%y
	}

	return x
}

func abs(x int) int {
	if x < 0 {
		return -x
	}

	return x
}

func mapAsteroids(input []string) []Asteroid {
	asteroids := []Asteroid(nil)
	for y, v := range input {
		for x, a := range v {
			if a == '#' {
				asteroids = append(asteroids, Asteroid{Pos{x, y}})
			}
		}
	}

	return asteroids
}

// Asteroid models an asteroid location
type Asteroid struct {
	pos Pos
}

func (a Asteroid) distanceFrom(other Pos) float64 {
	return math.Sqrt(math.Pow(float64(a.pos.x-other.x), 2) + math.Pow(float64(a.pos.y-other.y), 2))
}

func (a Asteroid) angle(turret Pos) float64 {
	angle := math.Atan2(float64(a.pos.y-turret.y), float64(a.pos.x-turret.x)) + (math.Pi / 2)
	if angle < 0 {
		angle += 2 * math.Pi
	}

	return angle
}

//Pos models a coordinate
type Pos struct {
	x, y int
}

func (p Pos) getDiff() Pos {
	gcd := abs(gcd(p.x, p.y))

	return Pos{p.x / gcd, p.y / gcd}
}
