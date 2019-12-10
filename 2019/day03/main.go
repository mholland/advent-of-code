package main

import (
	"bufio"
	"log"
	"math"
	"os"
	"strconv"
	"strings"
	"unicode"

	"github.com/pkg/errors"
)

func main() {
	file, err := os.Open("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	scanner.Scan()
	lineOne := scanner.Text()
	scanner.Scan()
	lineTwo := scanner.Text()

	wireOne, _ := ParsePathSegments(lineOne)
	wireTwo, _ := ParsePathSegments(lineTwo)

	shortestDistance, shortestPath := FindClosestDistances(wireOne, wireTwo)
	log.Printf("Shortest distance: %v\n", shortestDistance)
	log.Printf("Shortest path: %v", shortestPath)
}

type Vertex struct {
	x, y int
}

type PathSegment struct {
	Direction Vertex
	Length    int
}

var (
	up    Vertex = Vertex{0, +1}
	down  Vertex = Vertex{0, -1}
	left  Vertex = Vertex{-1, 0}
	right Vertex = Vertex{+1, 0}
)

func ParsePathSegments(input string) ([]PathSegment, error) {

	split := strings.FieldsFunc(input, func(r rune) bool {
		return !unicode.IsDigit(r) && !unicode.IsLetter(r)
	})

	result := make([]PathSegment, 0, len(split))
	for _, segment := range split {
		dir := segment[0]
		length := segment[1:]
		l, err := strconv.Atoi(length)
		if err != nil {
			return nil, errors.Wrap(err, "failed to convert string to int")
		}

		direction, ok := getDirectionFromInput(dir)
		if !ok {
			return nil, errors.New("could not parse direction")
		}

		result = append(result, PathSegment{direction, l})
	}

	return result, nil
}

func getDirectionFromInput(dir byte) (Vertex, bool) {
	directions := map[byte]Vertex{
		'U': up,
		'D': down,
		'L': left,
		'R': right,
	}

	d, ok := directions[dir]
	return d, ok
}

func FindClosestDistances(w1 []PathSegment, w2 []PathSegment) (int, int) {
	seen := map[Vertex]int{}
	intersections := make([]Vertex, 0)
	cur := Vertex{0, 0}
	steps := 0
	for _, s := range w1 {
		for i := 0; i < s.Length; i++ {
			new := Vertex{cur.x + s.Direction.x, cur.y + s.Direction.y}
			steps++
			if seen[new] == 0 {
				seen[new] = steps
			}

			cur = new
		}
	}
	cur = Vertex{0, 0}
	steps = 0
	for _, s := range w2 {
		for i := 0; i < s.Length; i++ {
			new := Vertex{cur.x + s.Direction.x, cur.y + s.Direction.y}
			steps++
			if seen[new] != 0 {
				intersections = append(intersections, new)
				seen[new] = seen[new] + steps
			}
			cur = new
		}
	}
	shortestDistance := math.MaxInt32
	shortestPath := math.MaxInt32
	for _, intersection := range intersections {
		distance := abs(intersection.x) + abs(intersection.y)
		if distance < shortestDistance {
			shortestDistance = distance
		}
		path := seen[intersection]
		if path < shortestPath {
			shortestPath = path
		}
	}

	return shortestDistance, shortestPath
}

func abs(x int) int {
	if x < 0 {
		return -x
	}
	return x
}
