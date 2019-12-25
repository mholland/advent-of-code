package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strings"
)

func main() {
	file, err := os.Open("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	orbits := []string(nil)
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		orbits = append(orbits, scanner.Text())
	}

	fmt.Printf("Number of orbits %v\n", CountOrbits(append([]string(nil), orbits...)))
	transfers, err := CalculateOrbitalTransfers(append([]string(nil), orbits...), "YOU", "SAN")
	if err != nil {
		panic(err)
	}
	fmt.Printf("Number of orbital transfers %v", transfers)
}

func MapOutPlanets(orbits []string) map[string]*Planet {
	planets := map[string]*Planet{}
	for _, orbit := range orbits {
		split := strings.Split(orbit, ")")
		parent := split[0]
		child := split[1]
		if _, ok := planets[parent]; !ok {
			planets[parent] = NewPlanet(parent)
		}
		if _, ok := planets[child]; !ok {
			planets[child] = NewPlanet(child)
		}
		planets[child].parent = planets[parent]
	}

	return planets
}

func CountOrbits(orbits []string) int {
	planets := MapOutPlanets(orbits)

	sum := 0
	for _, v := range planets {
		sum += v.Distance()
	}
	return sum
}

func CalculateOrbitalTransfers(orbits []string, o1 string, o2 string) (int, error) {
	planets := MapOutPlanets(orbits)

	myAncestors := getAncestors(planets, o1)
	santasAncestors := getAncestors(planets, o2)

	for i, u := range myAncestors {
		for j, v := range santasAncestors {
			if u == v {
				return i + j, nil
			}
		}
	}

	return -1, fmt.Errorf("Umable to find path between %v and %v", o1, o2)
}

func getAncestors(planets map[string]*Planet, planetName string) []*Planet {
	planet := planets[planetName]
	ancestors := []*Planet(nil)
	for cur := planet.parent; cur != nil; cur = cur.parent {
		ancestors = append(ancestors, cur)
	}

	return ancestors
}

func NewPlanet(name string) *Planet {
	return &Planet{
		name: name,
	}
}

type Planet struct {
	name   string
	parent *Planet
}

func (p *Planet) Distance() int {
	if p.parent == nil {
		return 0
	}

	return p.parent.Distance() + 1
}
