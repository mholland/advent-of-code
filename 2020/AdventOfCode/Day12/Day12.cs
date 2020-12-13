using System;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day12
{
    public sealed class Day12Tests : TestBase
    {
        protected override string Day { get; } = "Day12";

        public Day12Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            FindFinalDistance(new[]
            {
                "F10",
                "N3",
                "F7",
                "R90",
                "F11"
            })
            .Should()
            .Be(25);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Distance from start: {FindFinalDistance(Input)}");

        [Fact]
        public void ExampleTwo() =>
            FindFinalDistanceWithWaypoint(new[]
            {
                "F10",
                "N3",
                "F7",
                "R90",
                "F11"
            })
            .Should()
            .Be(286);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Distance from start: {FindFinalDistanceWithWaypoint(Input)}");

        private int FindFinalDistanceWithWaypoint(string[] input)
        {
            var steps = input.Select(ParseStep);

            (int X, int Y) currentPosition = (0, 0);
            (int X, int Y) waypointPosition = (10, 1);

            foreach (var step in steps)
            {
                switch ((step.Command, step.Value))
                {
                    case (Command.North, var v):
                        waypointPosition.Y += v;
                        break;
                    case (Command.East, var v):
                        waypointPosition.X += v;
                        break;
                    case (Command.South, var v):
                        waypointPosition.Y -= v;
                        break;
                    case (Command.West, var v):
                        waypointPosition.X -= v;
                        break;
                    case (Command.Forward, var v):
                        currentPosition.X += v * waypointPosition.X;
                        currentPosition.Y += v * waypointPosition.Y;
                        break;
                    case (Command.Right, 90):
                    case (Command.Left, 270):
                        waypointPosition = (waypointPosition.Y, -waypointPosition.X);
                        break;
                    case (Command.Right, 180):
                    case (Command.Left, 180):
                        waypointPosition = (-waypointPosition.X, -waypointPosition.Y);
                        break;
                    case (Command.Right, 270):
                    case (Command.Left, 90):
                        waypointPosition = (-waypointPosition.Y, waypointPosition.X);
                        break;
                }
            }

            return Math.Abs(currentPosition.X) + Math.Abs(currentPosition.Y);
        }

        private int FindFinalDistance(string[] input)
        {
            var steps = input.Select(ParseStep);

            var direction = 90;
            (int X, int Y) currentPosition = (0, 0);

            foreach (var step in steps)
            {
                switch ((step.Command, direction))
                {
                    case (Command.Left, _):
                        direction = ((direction - step.Value) % 360 + 360) % 360;
                        break;
                    case (Command.Right, _):
                        direction = (direction + step.Value) % 360;
                        break;
                    case (Command.North, _):
                    case (Command.Forward, 0):
                        currentPosition.Y += step.Value;
                        break;
                    case (Command.East, _):
                    case (Command.Forward, 90):
                        currentPosition.X += step.Value;
                        break;
                    case (Command.South, _):
                    case (Command.Forward, 180):
                        currentPosition.Y -= step.Value;
                        break;
                    case (Command.West, _):
                    case (Command.Forward, 270):
                        currentPosition.X -= step.Value;
                        break;
                }
            }

            return Math.Abs(currentPosition.X) + Math.Abs(currentPosition.Y);
        }

        private (Command Command, int Value) ParseStep(string step)
        {
            var command = step[0] switch
            {
                'F' => Command.Forward,
                'L' => Command.Left,
                'R' => Command.Right,
                'N' => Command.North,
                'E' => Command.East,
                'S' => Command.South,
                'W' => Command.West,
                _ => throw new Exception("Unknown command")
            };

            var value = int.Parse(step[1..]);

            return (command, value);
        }

        private enum Command
        {
            Forward,
            Left,
            Right,
            North,
            East,
            South,
            West
        }
    }
}