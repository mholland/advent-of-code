using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day03
{
    public sealed class Day03Tests : TestBase
    {
        protected override string Day => "Day03";

        public Day03Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne()
        {
            Day03.PartOne(new[]
            {
                "..##.......",
                "#...#...#..",
                ".#....#..#.",
                "..#.#...#.#",
                ".#...##..#.",
                "..#.##.....",
                ".#.#.#....#",
                ".#........#",
                "#.##...#...",
                "#...##....#",
                ".#..#...#.#"
            })
            .Should()
            .Be(7);
        }

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Trees encountered: {Day03.PartOne(Input)}");

        [Fact]
        public void ExampleTwo()
        {
            Day03.PartTwo(new[]
            {
                "..##.......",
                "#...#...#..",
                ".#....#..#.",
                "..#.#...#.#",
                ".#...##..#.",
                "..#.##.....",
                ".#.#.#....#",
                ".#........#",
                "#.##...#...",
                "#...##....#",
                ".#..#...#.#"
            })
            .Should()
            .Be(336);
        }

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Combined encountered: {Day03.PartTwo(Input)}");
    }

    public static class Day03
    {
        public static long PartOne(string[] input)
        {
            var (treeSet, width, height) = MapTerrain(input);
            return CalculateCollisions(treeSet, width, height, (3, 1));
        }

        public static long PartTwo(string[] input)
        {
            var (treeSet, width, height) = MapTerrain(input);
            var directions = new (int, int)[]
            {
                (1, 1),
                (3, 1),
                (5, 1),
                (7, 1),
                (1, 2)
            };

            return directions.Aggregate(1L, (cur, next) => CalculateCollisions(treeSet, width, height, next) * cur);
        }

        static long CalculateCollisions(HashSet<(int X, int Y)> treeSet, int width, int height, (int X, int Y) direction)
        {
            var location = (X: 0, Y: 0);
            var treesEncountered = 0L;
            do 
            {
                location = ((location.X + direction.X) % width, location.Y + direction.Y);
                if (treeSet.Contains(location))
                    treesEncountered++;
            } while (location.Y < height - 1);

            return treesEncountered;
        }

        static (HashSet<(int X, int Y)> Terrain, int Width, int Height) MapTerrain(string[] input)
        {
            var width = input.Max(l => l.Length);
            var height = input.Length;
            var treeSet = new HashSet<(int X, int Y)>();
            for (var y = 0; y < input.Length; y++)
            {
                var line = input[y];
                for (var x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                        treeSet.Add((x, y));
                }
            }

            return (treeSet, width, height);
        }
    }
}