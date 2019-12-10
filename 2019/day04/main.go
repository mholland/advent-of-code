package main

import "fmt"

func main() {
	firstMatches := 0
	secondMatches := 0
	start := 172930
	end := 683082
	for i := start; i <= end; i++ {
		f, s := PasswordMatches(i)
		if f {
			firstMatches++
		}
		if s {
			secondMatches++
		}
	}

	fmt.Printf("Number of matches with first criteria: %v, with second: %v", firstMatches, secondMatches)
}

func PasswordMatches(password int) (bool, bool) {
	first := (password / 100000) % 10
	second := (password / 10000) % 10
	third := (password / 1000) % 10
	fourth := (password / 100) % 10
	fifth := (password / 10) % 10
	sixth := password % 10

	digits := [6]int{first, second, third, fourth, fifth, sixth}
	containsDouble := false
	identicalRuns := map[int]int{}
	ascends := true
	for i := range digits {
		if i > 4 {
			continue
		}
		if digits[i] == digits[i+1] {
			containsDouble = true
			identicalRuns[digits[i]] = identicalRuns[digits[i]] + 1
		}

		if digits[i] > digits[i+1] {
			ascends = false
		}
	}
	containsAtLeastOneDouble := false
	for _, v := range identicalRuns {
		if v+1 == 2 {
			containsAtLeastOneDouble = true
		}
	}

	return containsDouble && ascends, ascends && containsAtLeastOneDouble
}
