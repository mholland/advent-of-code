package main

import "testing"

func TestPasswordMatches_ConsecutiveNumbers(t *testing.T) {
	password := 111111
	matches := PasswordMatches(password)
	if !matches {
		t.Errorf("Expected %v to match", password)
	}
}

func TestPasswordMatches_AscendingNumbers(t *testing.T) {
	password := 551234
	matches := PasswordMatches(password)
	if matches {
		t.Errorf("Expected %v not to match", password)
	}
}
