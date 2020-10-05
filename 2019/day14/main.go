package main

import (
	"bufio"
	"log"
	"os"
	"sort"
	"strconv"
	"strings"
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

	reactions := parseInputIntoReactions(input)
	ore := calculateRequiredOre(reactions)

	log.Printf("Ore required to make 1 FUEL: %v\n", ore)

	maxFuel := maxFuelProducedBy(1e12, reactions)
	log.Printf("Fuel producable with 1e12 ORE: %v\n", maxFuel)
}

func maxFuelProducedBy(ore int, reactions map[string]reaction) int {
	return sort.Search(ore, func(n int) bool {
		return processRequirements(reactions, map[string]int{
			"FUEL": n + 1,
		}) >= ore
	})
}

func processRequirements(reactions map[string]reaction, requiredElements map[string]int) int {
	for hasUnprocessedRequirements(requiredElements) {
		for element, amountRequired := range requiredElements {
			if element == "ORE" || amountRequired <= 0 {
				continue
			}
			recipes := reactions[element].recipe
			amountProduced := reactions[element].amountProduced
			reactionsRequired := ((amountRequired-1)/amountProduced + 1)

			requiredElements[element] -= amountProduced * reactionsRequired

			for _, compound := range recipes {
				requiredElements[compound.name] += reactionsRequired * compound.amount
			}
		}
	}

	return requiredElements["ORE"]
}

func calculateRequiredOre(reactions map[string]reaction) int {
	requiredElements := map[string]int{
		"FUEL": 1,
	}

	return processRequirements(reactions, requiredElements)
}

func hasUnprocessedRequirements(requiredElements map[string]int) bool {
	for name, amount := range requiredElements {
		if name != "ORE" && amount > 0 {
			return true
		}
	}

	return false
}

func parseInputIntoReactions(input []string) map[string]reaction {
	reactions := make(map[string]reaction, len(input))
	for _, val := range input {
		split := strings.Split(val, " => ")
		compoundsInput := strings.Split(split[0], ", ")
		result := newCompound(split[1])
		reac := reaction{
			amountProduced: result.amount,
			recipe:         []compound(nil),
		}
		for _, v := range compoundsInput {
			c := newCompound(v)
			reac.recipe = append(reac.recipe, c)
			reactions[result.name] = reac
		}
	}
	return reactions
}

type reaction struct {
	amountProduced int
	recipe         []compound
}

type compound struct {
	name   string
	amount int
}

func newCompound(compoundInput string) compound {
	split := strings.Split(compoundInput, " ")
	amount, err := strconv.Atoi(split[0])
	if err != nil {
		panic(err)
	}
	return compound{
		name:   split[1],
		amount: amount,
	}
}
