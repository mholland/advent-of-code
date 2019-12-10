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
	ascends := digitsAscend(digits)
	containsConsecutive := containsConsecutive(digits)
	containsDouble := containsDouble(digits)

	return containsConsecutive && ascends, ascends && containsDouble
}

func digitsAscend(digits [6]int) bool {
	for i := 0; i < len(digits)-1; i++ {
		if digits[i] > digits[i+1] {
			return false
		}
	}

	return true
}

func containsConsecutive(digits [6]int) bool {
	for i := 0; i < len(digits)-1; i++ {
		if digits[i] == digits[i+1] {
			return true
		}
	}

	return false
}

func containsDouble(digits [6]int) bool {
	for i := 0; i < len(digits)-1; i++ {
		count := 1
		for j := i + 1; j < len(digits); j++ {
			if digits[i] == digits[j] {
				count++
				i++
			}
		}
		if count == 2 {
			return true
		}

	}

	return false
}
