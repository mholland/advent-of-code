package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
)

func main() {
	fmt.Printf("Part 1 mass: %d\n", calculateFuelForModules())
	fmt.Printf("Part 2 mass: %d", calculateFuelAccountingForFuel())
}

func calculateFuelAccountingForFuel() int {
	file, err := os.Open("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	sum := 0
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		mass, err := strconv.Atoi(scanner.Text())
		if err != nil {
			log.Fatal(err)
		}
		for {
			mass = mass/3 - 2
			if mass <= 0 {
				break
			}
			sum += mass
		}
	}

	return sum
}

func calculateFuelForModules() int {
	file, err := os.Open("input.txt")
	if err != nil {
		log.Fatal(err)
	}
	defer file.Close()

	sum := 0
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		mass, err := strconv.Atoi(scanner.Text())
		if err != nil {
			log.Fatal(err)
		}
		sum += mass/3 - 2
	}

	return sum
}
