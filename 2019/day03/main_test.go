package main

import (
	"reflect"
	"testing"
)

func TestParsePathSegments_UpSegment(t *testing.T) {
	actual, _ := ParsePathSegments("U5")
	expected := []PathSegment{PathSegment{Vertex{0, 1}, 5}}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected %v but actual %v", expected, actual)
	}
}

func TestParsePathSegments_DownSegment(t *testing.T) {
	actual, _ := ParsePathSegments("D8")
	expected := []PathSegment{PathSegment{Vertex{0, -1}, 8}}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected %v but actual %v", expected, actual)
	}
}

func TestParsePathSegments_RightSegment(t *testing.T) {
	actual, _ := ParsePathSegments("R3")
	expected := []PathSegment{PathSegment{Vertex{+1, 0}, 3}}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected %v but actual %v", expected, actual)
	}
}

func TestParsePathSegments_LeftSegment(t *testing.T) {
	actual, _ := ParsePathSegments("L4")
	expected := []PathSegment{PathSegment{Vertex{-1, 0}, 4}}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected %v but actual %v", expected, actual)
	}
}

func TestParsePathSegments_MultipleSegments(t *testing.T) {
	actual, _ := ParsePathSegments("R8,U5,L5,D3")
	expected := []PathSegment{
		PathSegment{right, 8},
		PathSegment{up, 5},
		PathSegment{left, 5},
		PathSegment{down, 3},
	}
	if !reflect.DeepEqual(actual, expected) {
		t.Errorf("Expected %v but actual %v", expected, actual)
	}
}

func TestFindClosestIntersectionDistance_SmallExample(t *testing.T) {
	segment1, _ := ParsePathSegments("R8,U5,L5,D3")
	segment2, _ := ParsePathSegments("U7,R6,D4,L4")

	actual, _ := FindClosestDistances(segment1, segment2)
	expected := 6

	if actual != expected {
		t.Errorf("Expected %v but actually %v", expected, actual)
	}
}

func TestFindClosestIntersectionDistance_LargerExample(t *testing.T) {
	segment1, _ := ParsePathSegments("R75,D30,R83,U83,L12,D49,R71,U7,L72")
	segment2, _ := ParsePathSegments("U62,R66,U55,R34,D71,R55,D58,R83")

	actual, _ := FindClosestDistances(segment1, segment2)
	expected := 159

	if actual != expected {
		t.Errorf("Expected %v but actually %v", expected, actual)
	}
}

func TestFindClosestIntersectionDistance_AnotherLargerExample(t *testing.T) {
	segment1, _ := ParsePathSegments("R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51")
	segment2, _ := ParsePathSegments("U98,R91,D20,R16,D67,R40,U7,R15,U6,R7")

	actual, _ := FindClosestDistances(segment1, segment2)
	expected := 135

	if actual != expected {
		t.Errorf("Expected %v but actually %v", expected, actual)
	}
}
