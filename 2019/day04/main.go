package main

import "fmt"

func main() {
	matches := 0
	start := 172930
	end := 683082
	for i := start; i <= end; i++ {
		if PasswordMatches(i) {
			matches++
		}
	}

	fmt.Printf("Number of matches: %v", matches)
}

func PasswordMatches(password int) bool {
	first := (password / 100000) % 10
	second := (password / 10000) % 10
	third := (password / 1000) % 10
	fourth := (password / 100) % 10
	fifth := (password / 10) % 10
	sixth := password % 10

	digits := [6]int{first, second, third, fourth, fifth, sixth}
	containsDouble := false
	ascends := true
	for i := range digits {
		if i > 4 {
			continue
		}
		if digits[i] == digits[i+1] {
			containsDouble = true
		}
		if digits[i] > digits[i+1] {
			ascends = false
		}
	}

	return containsDouble && ascends
}
