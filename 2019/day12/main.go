package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"regexp"
	"strconv"
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

	fmt.Printf("System energy after 1000 steps: %v\n", calculateSystemEnergy(input, 1000))
	fmt.Printf("Steps until the moons repeat a state: %v", calculateStepsToPreviousPositions(input))
}

func calculateSystemEnergy(input []string, steps int) int {
	moons := parseMoons(input)

	for i := 0; i < steps; i++ {
		stepMoons(moons)
	}
	totalEnergy := 0
	for _, moon := range moons {
		totalEnergy += moon.energy()
	}

	return totalEnergy
}

func stepMoons(moons []Moon) {
	for j := 0; j < len(moons); j++ {
		for k := 0; k < len(moons); k++ {
			if j == k {
				continue
			}
			moon := &moons[j]
			otherMoon := moons[k]

			moon.Vel.X += calculateDiff(moon.Pos.X, otherMoon.Pos.X)
			moon.Vel.Y += calculateDiff(moon.Pos.Y, otherMoon.Pos.Y)
			moon.Vel.Z += calculateDiff(moon.Pos.Z, otherMoon.Pos.Z)
		}
	}
	for x := 0; x < len(moons); x++ {
		moons[x].step()
	}
}

func calculateStepsToPreviousPositions(input []string) int {
	moonsStart := parseMoons(input)
	moons := make([]Moon, len(moonsStart))
	copy(moons, moonsStart)

	back := make([]bool, 3)
	steps := make([]int, 3)
	dims := []func(vec vec3) int{
		func(vec vec3) int { return vec.X },
		func(vec vec3) int { return vec.Y },
		func(vec vec3) int { return vec.Z },
	}

	for !back[0] || !back[1] || !back[2] {
		stepMoons(moons)
		for i, s := range back {
			if s {
				continue
			}

			steps[i]++

			b := true
			for j := range moons {
				if !moonsBackAtStart(moons[j], moonsStart[j], dims[i]) {
					b = false
				}
			}

			back[i] = b
		}
	}

	return lcm(steps[0], steps[1], steps[2])
}

func lcm(x, y int, rest ...int) int {
	result := x / gcd(x, y) * y

	for i := 0; i < len(rest); i++ {
		result = lcm(result, rest[i])
	}

	return result
}

func gcd(x, y int) int {
	for y != 0 {
		x, y = y, x%y
	}

	return x
}

func moonsBackAtStart(moon Moon, initial Moon, dimSelector func(v vec3) int) bool {
	return dimSelector(moon.Pos) == dimSelector(initial.Pos) &&
		dimSelector(moon.Vel) == dimSelector(initial.Vel)
}

func calculateDiff(moon int, other int) int {
	diff := other - moon
	if diff > 0 {
		return 1
	}

	if diff < 0 {
		return -1
	}

	return 0
}

func parseMoons(input []string) []Moon {
	r := regexp.MustCompile("-?[0-9]+")
	moons := make([]Moon, len(input))
	for i, moon := range input {
		coords := r.FindAllString(moon, -1)
		var m Moon
		m.Pos.X, _ = strconv.Atoi(coords[0])
		m.Pos.Y, _ = strconv.Atoi(coords[1])
		m.Pos.Z, _ = strconv.Atoi(coords[2])
		moons[i] = m
	}

	return moons
}

type Moon struct {
	Pos, Vel vec3
}

func (m *Moon) step() {
	m.Pos.X += m.Vel.X
	m.Pos.Y += m.Vel.Y
	m.Pos.Z += m.Vel.Z
}

func (m *Moon) energy() int {
	return m.potentialEnergy() * m.kineticEnergy()
}

func (m *Moon) potentialEnergy() int {
	return abs(m.Pos.X) + abs(m.Pos.Y) + abs(m.Pos.Z)
}

func (m *Moon) kineticEnergy() int {
	return abs(m.Vel.X) + abs(m.Vel.Y) + abs(m.Vel.Z)
}

func abs(val int) int {
	if val < 0 {
		return -val
	}

	return val
}

type vec3 struct {
	X, Y, Z int
}
