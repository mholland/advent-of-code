using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day17
{
    public sealed class Day17Tests : TestBase
    {
        protected override string Day { get; } = "Day17";

        public Day17Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            SimulateCycles(new[]
            {
                ".#.",
                "..#",
                "###"
            }, 3)
            .Should()
            .Be(112);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Active cubes after 6 rounds: {SimulateCycles(Input, 3)}");

        [Fact]
        public void ExampleTwo() =>
            SimulateCycles(new[]
            {
                ".#.",
                "..#",
                "###"
            }, 4)
            .Should()
            .Be(848);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Active cubes after 6 rounds: {SimulateCycles(Input, 4)}");

        private long SimulateCycles(string[] input, int dimensions)
        {
            var cubes = new Dictionary<Coord, bool>();
            for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input.Max(r => r.Length); x++)
            {
                cubes.Add(new Coord(x, y, 0, 0), input[y][x] == '#');
            }

            for (var i = 0; i < 6; i++)
            {
                cubes = RunSimulation(cubes, dimensions);
                PrintCubes(i + 1, cubes);
            }

            return cubes.Values.Count(active => active);
        }

        Dictionary<Coord, bool> RunSimulation(Dictionary<Coord, bool> cubes, int dimensions)
        {
            var result = new Dictionary<Coord, bool>(cubes);
            var minW = cubes.Keys.Min(c => c.W) - 1;
            var maxW = cubes.Keys.Max(c => c.W) + 1;
            var minZ = cubes.Keys.Min(c => c.Z) - 1;
            var maxZ = cubes.Keys.Max(c => c.Z) + 1;
            var minY = cubes.Keys.Min(c => c.Y) - 1;
            var maxY = cubes.Keys.Max(c => c.Y) + 1;
            var minX = cubes.Keys.Min(c => c.X) - 1;
            var maxX = cubes.Keys.Max(c => c.X) + 1;

            if (dimensions == 3)
                minW = maxW = 0;

            for (var w = minW; w <= maxW; w++)
            for (var z = minZ; z <= maxZ; z++)
            for (var y = minY; y <= maxY; y++)
            for (var x = minX; x <= maxX; x++)
            {
                var currentCube = new Coord(x, y, z, w);
                var isActive = cubes.TryGetValue(currentCube, out var active) && active;
                var activeNeighbours = currentCube.Adjacent(dimensions).Count(c => cubes.TryGetValue(c, out var ac) && ac);
                if (isActive && activeNeighbours != 2 && activeNeighbours != 3)
                {
                    result[currentCube] = false;
                    continue;
                }

                if (!isActive && activeNeighbours == 3)
                    result[currentCube] = true;
            }

            return result;
        }

        private void PrintCubes(int cycle, Dictionary<Coord, bool> cubes)
        {
            var minW = cubes.Keys.Where(k => cubes[k]).Min(c => c.W);
            var maxW = cubes.Keys.Where(k => cubes[k]).Max(c => c.W);
            var minZ = cubes.Keys.Where(k => cubes[k]).Min(c => c.Z);
            var maxZ = cubes.Keys.Where(k => cubes[k]).Max(c => c.Z);
            var minY = cubes.Keys.Where(k => cubes[k]).Min(c => c.Y);
            var maxY = cubes.Keys.Where(k => cubes[k]).Max(c => c.Y);
            var minX = cubes.Keys.Where(k => cubes[k]).Min(c => c.X);
            var maxX = cubes.Keys.Where(k => cubes[k]).Max(c => c.X);

            Console.WriteLine($"After {cycle} cycle(s)");
            for (var w = minW; w <= maxW; w++)
            for (var z = minZ; z <= maxZ; z++)
            {
                Console.WriteLine($"z = {z}, w = {w}");
                for (var y = minY; y <= maxY; y++)
                {
                    for (var x = minX; x <= maxX; x++)
                    {
                        if (cubes.TryGetValue(new Coord(x, y, z, 0), out var c) && c)
                        {
                            Console.Write("#");
                            continue;
                        }

                        Console.Write(".");
                    }
                    Console.WriteLine();
                }
            }
        }

        public record Coord(int X, int Y, int Z, int W)
        {
            public IEnumerable<Coord> Adjacent(int dimensions)
            {
                var minW = W - 1;
                var maxW = W + 1;
                if (dimensions == 3)
                    minW = maxW = 0;

                for (var w = minW; w <= maxW; w++)
                for (var z = Z - 1; z <= Z + 1; z++)
                for (var y = Y - 1; y <= Y + 1; y++)
                for (var x = X - 1; x <= X + 1; x++)
                {
                    if (x == X && y == Y && z == Z && w == W)
                        continue;

                    yield return new Coord(x, y, z, w);
                }
            }
        }
    }
}