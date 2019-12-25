package main

import (
	"fmt"
	"io/ioutil"
	"log"
	"math"
)

func main() {
	input, err := ioutil.ReadFile("input.txt")
	if err != nil {
		log.Fatal(err)
	}

	fmt.Printf("Checksum for image: %v\n", CalculateChecksum(string(input), 25, 6))

	image := DecodeImage(string(input), 25, 6)
	for _, r := range image {
		fmt.Println(r)
	}
}

func parseLayers(input string, width int, height int) [][]string {
	result := [][]string(nil)
	size := width * height
	for i := 0; i < len(input)/size; i++ {
		layer := input[i*size : i*size+size]
		res := []string(nil)
		for j := 0; j < len(layer)/width; j++ {
			res = append(res, layer[j*width:j*width+width])
		}
		result = append(result, res)
	}

	return result
}

func CalculateChecksum(input string, width int, height int) int {
	layers := parseLayers(input, width, height)
	minZeros := math.MaxInt32
	checksumLayer := 0

	for i, layer := range layers {
		numZeros := 0
		for _, row := range layer {
			for _, pixel := range row {
				if pixel == '0' {
					numZeros++
				}
			}
		}
		if numZeros < minZeros {
			minZeros = numZeros
			checksumLayer = i
		}
	}

	frequencies := make(map[rune]int)
	for _, l := range layers[checksumLayer] {
		for _, r := range l {
			frequencies[r]++
		}
	}

	return frequencies['1'] * frequencies['2']
}

func DecodeImage(input string, width int, height int) []string {
	image := []string(nil)
	layers := parseLayers(input, width, height)

	for y := 0; y < height; y++ {
		var row string
		for x := 0; x < width; x++ {
			for l := 0; l < len(layers); l++ {
				pixelValue := layers[l][y][x]
				if pixelValue == '1' {
					row += "#"
					break
				}
				if pixelValue == '0' {
					row += " "
					break
				}
			}
		}
		image = append(image, row)
	}

	return image
}
