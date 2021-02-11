package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
)

func main() {
	file, err := os.Open("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	input := make([]int, 0)
	for scanner.Scan() {
		for _, v := range scanner.Text() {
			i, err := strconv.Atoi(string(v))
			if err != nil {
				log.Fatal(err)
			}
			input = append(input, i)
		}
	}

	partOneInput := make([]int, len(input))
	copy(partOneInput, input)
	result := applyFFT(partOneInput, 100)
	for _, x := range result {
		fmt.Print(x)
	}
	fmt.Println()
	partTwoInput := make([]int, len(input))
	copy(partTwoInput, input)
	partTwoResult := findEmbeddedMessage(partTwoInput)
	for _, x := range partTwoResult {
		fmt.Print(x)
	}
	fmt.Println()
}

func findEmbeddedMessage(input []int) []int {
	offset := messageOffset(input)
	repeatedInput := repeat(input, 10000)
	fft := applyFFTLatterHalf(repeatedInput, 100)

	return fft[offset : offset+8]
}

func applyFFTLatterHalf(input []int, phases int) []int {
	for p := 0; p < phases; p++ {
		prev := 0
		for i := len(input) - 1; i >= len(input)/2; i-- {
			prev += input[i]
			prev %= 10
			input[i] = prev
		}
	}

	return input
}

func repeat(input []int, times int) []int {
	l := len(input)
	result := make([]int, l*times)
	for n := 0; n < times; n++ {
		copy(result[n*l:(n+1)*l], input)
	}

	return result
}

func messageOffset(input []int) int {
	offset := input[:7]
	multiplier := 1
	result := 0
	for i := len(offset) - 1; i >= 0; i-- {
		result += offset[i] * multiplier
		multiplier *= 10
	}

	return result
}

func applyFFT(input []int, phases int) []int {
	phasePatterns := make([][]int, len(input))
	for i := 0; i < len(input); i++ {
		phasePatterns[i] = generatePattern(len(input), i+1)
	}

	for j := 0; j < phases; j++ {
		newSignal := make([]int, len(input))
		for x, p := range phasePatterns {
			sum := 0
			for y := 0; y < len(p); y++ {
				sum += input[y] * p[y]
			}
			newSignal[x] = abs(sum % 10)
		}
		input = newSignal
	}

	return input[:8]
}

func abs(val int) int {
	if val < 0 {
		return -val
	}

	return val
}

func generatePattern(inputLen, phase int) []int {
	basePattern := [4]int{0, 1, 0, -1}

	pattern := make([]int, 0)
	repeat := phase - 1
	baseIndex := 0
	for i := 0; i < inputLen; i++ {
		if repeat <= 0 {
			repeat = phase
			baseIndex = (baseIndex + 1) % len(basePattern)
		}
		repeat--
		pattern = append(pattern, basePattern[baseIndex])
	}

	return pattern
}
