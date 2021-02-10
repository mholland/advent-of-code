package main

import (
	"bufio"
	"fmt"
	"log"
	"math"
	"os"
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
	grid := parseMaze(input)
	shortest := collectAllKeys(grid)
	fmt.Printf("Shortest path to all keys: %d\n", shortest)

	init := grid.players[0]
	grid.maze[init] = false
	grid.maze[vec2{init.x, init.y + 1}] = false
	grid.maze[vec2{init.x, init.y - 1}] = false
	grid.maze[vec2{init.x - 1, init.y}] = false
	grid.maze[vec2{init.x + 1, init.y}] = false
	grid.players = make([]vec2, 4)
	grid.players[0] = vec2{init.x - 1, init.y - 1}
	grid.players[1] = vec2{init.x + 1, init.y - 1}
	grid.players[2] = vec2{init.x - 1, init.y + 1}
	grid.players[3] = vec2{init.x + 1, init.y + 1}

	shortest = collectAllKeys(grid)
	fmt.Printf("Shortest path to all keys with 4 bots: %d\n", shortest)
}

type vec2 struct {
	x, y int
}

func (v vec2) neighbours() []vec2 {
	return []vec2{
		{v.x + 1, v.y},
		{v.x - 1, v.y},
		{v.x, v.y + 1},
		{v.x, v.y - 1},
	}
}

type grid struct {
	players []vec2
	maze    map[vec2]bool
	doors   map[vec2]rune
	keys    map[vec2]rune
}

type reachablePos struct {
	pos          vec2
	requiredKeys int
	dist         int
	key          int
}

type state struct {
	pos           []vec2
	collectedKeys int
	dist          int
}

func (s state) cacheKey() string {
	var res string
	for _, v := range s.pos {
		res += fmt.Sprintf("%d,%d,", v.x, v.y)
	}
	res += fmt.Sprint(s.collectedKeys)
	return res
}

func collectAllKeys(grid grid) int {
	reachablePositions := make(map[vec2][]reachablePos)
	for _, p := range grid.players {
		reachablePositions[p] = bfs(p, grid)
	}
	for pos := range grid.keys {
		reachablePositions[pos] = bfs(pos, grid)
	}

	cache := make(map[string]int)

	work := make([]state, 0)
	initialState := state{make([]vec2, len(grid.players)), 0, 0}
	copy(initialState.pos, grid.players)
	work = append(work, initialState)
	allKeys := (1 << len(grid.keys)) - 1
	shortest := math.MaxInt32
	for len(work) > 0 {
		current := work[0]
		work = work[1:]
		// fmt.Printf("Current: %d,%d, dist: %d, collectedKeys: %d\n", current.pos[0].x, current.pos[0].y, current.dist, current.collectedKeys)

		ck := current.cacheKey()
		if dist, ok := cache[ck]; ok {
			if current.dist >= dist {
				// fmt.Println("	A more optimal path has already been found.")
				continue
			}
		}
		cache[ck] = current.dist

		if current.collectedKeys == allKeys {
			if current.dist < shortest {
				// fmt.Printf("	New shortest found: %d\n", current.dist)
				shortest = current.dist
			}
			continue
		}

		for i, p := range current.pos {
			for _, r := range reachablePositions[p] {
				if current.collectedKeys&r.requiredKeys != r.requiredKeys || current.collectedKeys&(1<<r.key) == (1<<r.key) {
					// fmt.Printf("	Key: %c is unreachable or we already have it.\n", rune(r.key+97))
					continue
				}

				newState := current
				newState.dist += r.dist
				newState.collectedKeys |= (1 << r.key)
				newState.pos = make([]vec2, len(current.pos))
				copy(newState.pos, current.pos)
				newState.pos[i] = r.pos
				// fmt.Printf("	Collected %c for a total of %d so far\n", rune(r.key+97), newState.dist)

				work = append(work, newState)
			}
		}
	}

	return shortest
}

func bfs(start vec2, g grid) []reachablePos {
	reachable := make([]reachablePos, 0)
	visited := make(map[vec2]bool)
	visited[start] = true

	work := make([]reachablePos, 0)
	work = append(work, reachablePos{start, 0, 0, -1})
	for len(work) > 0 {
		current := work[0]
		work = work[1:]

		for _, n := range current.pos.neighbours() {
			if !g.maze[n] {
				continue
			}
			if visited[n] {
				continue
			}
			visited[n] = true
			newState := reachablePos{
				dist:         current.dist + 1,
				pos:          n,
				requiredKeys: current.requiredKeys,
			}
			if d, ok := g.doors[n]; ok {
				newState.requiredKeys |= (1 << (d - 65))
			}
			if k, ok := g.keys[n]; ok {
				newState.key = int(k - 'a')
				reachable = append(reachable, newState)
			}
			work = append(work, newState)
		}
	}

	return reachable
}

func parseMaze(input []string) grid {
	maze := make(map[vec2]bool)
	keys := make(map[vec2]rune)
	doors := make(map[vec2]rune)
	players := make([]vec2, 0)
	for y, line := range input {
		for x, char := range line {
			pos := vec2{x, y}
			if char == '#' {
				maze[pos] = false
				continue
			}
			maze[pos] = true
			if char == '@' {
				players = append(players, pos)
				continue
			}
			if char == '.' {
				maze[pos] = true
				continue
			}
			if char >= 'a' {
				keys[pos] = char
				continue
			}
			doors[pos] = char
		}
	}

	return grid{
		players: players,
		maze:    maze,
		doors:   doors,
		keys:    keys,
	}
}
