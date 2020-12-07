package main

import (
	"fmt"

	"github.com/mholland/advent-of-code/intcode"
)

func main() {
	gameState := runGame(1)
	blocks := 0
	for _, tile := range gameState {
		if tile == block {
			blocks++
		}
	}

	fmt.Printf("Number of starting blocks: %v\n", blocks)

	finishedGame := runGame(2)
	printGame(finishedGame)
	fmt.Printf("Highscore: %v", finishedGame[pos{-1, 0}])
}

func printGame(tiles map[pos]tileType) {
	for y := 0; y < 25; y++ {
		for x := 0; x < 42; x++ {
			switch tiles[pos{x, y}] {
			case empty:
				fmt.Print(" ")
			case wall:
				fmt.Print("#")
			case block:
				fmt.Print("□")
			case paddle:
				fmt.Print("_")
			case ball:
				fmt.Print("o")
			}
		}
		fmt.Println()
	}
	for x := 0; x < 42; x++ {
		if x%2 == 0 {
			fmt.Print("~")
			continue
		}

		fmt.Print("-")
	}
	fmt.Println()
}

func runGame(mode int) map[pos]tileType {
	program := intcode.ReadProgram("input.txt")
	machine := intcode.NewIntcodeMachine()
	machine.LoadMemory(program)
	machine.SetMemoryValue(0, mode)

	input := 0
	for machine.State != intcode.Halted {
		machine.SetInput(input)
		machine.RunProgram()

		ballPos := findTile(mapOutput(machine.GetOutput()), ball)
		paddlePos := findTile(mapOutput(machine.GetOutput()), paddle)

		if ballPos.x > paddlePos.x {
			input = 1
			continue
		}

		if ballPos.x < paddlePos.x {
			input = -1
			continue
		}

		input = 0
	}

	return mapOutput(machine.GetOutput())
}

func findTile(gameState map[pos]tileType, tileType tileType) pos {
	for pos, tt := range gameState {
		if tt == tileType {
			return pos
		}
	}

	return pos{-1, -1}
}

func mapOutput(output []int) map[pos]tileType {
	gameState := make(map[pos]tileType)
	for i := 0; i < len(output); i += 3 {
		x, y, tile := output[i], output[i+1], output[i+2]
		gameState[pos{x, y}] = tileType(tile)
	}

	return gameState
}

type pos struct {
	x, y int
}

type tileType int

const (
	empty tileType = iota
	wall
	block
	paddle
	ball
)
