using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day11
{
    public sealed class Day11Tests : TestBase
    {
        protected override string Day { get; } = "Day11";

        public Day11Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            SimulateOccupiedSeatsPartOne(new[]
            {
                "L.LL.LL.LL",
                "LLLLLLL.LL",
                "L.L.L..L..",
                "LLLL.LL.LL",
                "L.LL.LL.LL",
                "L.LLLLL.LL",
                "..L.L.....",
                "LLLLLLLLLL",
                "L.LLLLLL.L",
                "L.LLLLL.LL"
            })
            .Should()
            .Be(37);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Occupied seats: {SimulateOccupiedSeatsPartOne(Input)}");

        [Fact]
        public void ExampleTwo() =>
            SimulateOccupiedSeatsPartTwo(new[]
            {
                "L.LL.LL.LL",
                "LLLLLLL.LL",
                "L.L.L..L..",
                "LLLL.LL.LL",
                "L.LL.LL.LL",
                "L.LLLLL.LL",
                "..L.L.....",
                "LLLLLLLLLL",
                "L.LLLLLL.L",
                "L.LLLLL.LL"
            })
            .Should()
            .Be(26);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Occupied seats: {SimulateOccupiedSeatsPartTwo(Input)}");

        private static readonly Vec2[] AdjacentVectors = new[]
        {
            new Vec2(0, 1),
            new Vec2(1, 1),
            new Vec2(1, 0),
            new Vec2(1, -1),
            new Vec2(0, -1),
            new Vec2(-1, -1),
            new Vec2(-1, 0),
            new Vec2(-1, 1)
        };

        private enum Position
        {
            Empty,
            Occupied,
            Floor
        }

        private int SimulateOccupiedSeatsPartOne(string[] input) =>
            SimulateOccupiedSeats(
                input,
                (seats, adjacent, current) => seats.TryGetValue(current.Add(adjacent), out var pos) && pos == Position.Occupied,
                4
            );

        private int SimulateOccupiedSeatsPartTwo(string[] input) =>
            SimulateOccupiedSeats(
                input,
                (seats, adjacent, current) =>
                {
                    while (seats.TryGetValue(current = current.Add(adjacent), out var pos))
                    {
                        if (pos == Position.Floor)
                            continue;

                        return pos == Position.Occupied;
                    }

                    return false;
                },
                5
            );

        private int SimulateOccupiedSeats(string[] input, Func<Dictionary<Vec2, Position>, Vec2, Vec2, bool> isVisible, int visibleThreshold)
        {
            var seats = new Dictionary<Vec2, Position>();
            for (var y = 0; y < input.Length; y++)
            {
                for (var x = 0; x < input[y].Length; x++)
                {
                    seats.Add(new Vec2(x, y), input[y][x] switch
                    {
                        'L' => Position.Empty,
                        '#' => Position.Occupied,
                        '.' => Position.Floor,
                        _ => throw new Exception("Unexpected character")
                    });
                }
            }

            var seatsChanged = 0;

            do
            {
                (seatsChanged, seats) = SimulateRound(seats, isVisible, visibleThreshold);
                PrintSeats(seats);
            } while (seatsChanged > 0);

            return seats.Values.Count(x => x == Position.Occupied);

            (int SeatsChanged, Dictionary<Vec2, Position> Seats) SimulateRound(
                Dictionary<Vec2, Position> seats,
                Func<Dictionary<Vec2, Position>, Vec2, Vec2, bool> isVisible,
                int visibleThreshold
            )
            {
                var result = new Dictionary<Vec2, Position>();
                var seatsChanged = 0;
                foreach (var (current, currentPosition) in seats)
                {
                    var visible =
                        AdjacentVectors
                            .Count(adjacent => isVisible.Invoke(seats, adjacent, current));

                    switch ((currentPosition, visible))
                    {
                        case (Position.Empty, 0):
                            seatsChanged++;
                            result.Add(current, Position.Occupied);
                            break;
                        case (Position.Occupied, var vis) when vis >= visibleThreshold:
                            seatsChanged++;
                            result.Add(current, Position.Empty);
                            break;
                        default:
                            result.Add(current, currentPosition);
                            break;
                    }
                }

                return (seatsChanged, result);
            }
        }

        private string PrintSeats(Dictionary<Vec2, Position> seats)
        {
            var width = seats.Keys.Max(v => v.X) + 1;
            var height = seats.Keys.Max(v => v.Y) + 1;
            var sb = new StringBuilder();

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    sb.Append(seats[new Vec2(x, y)] switch
                    {
                        Position.Empty => 'L',
                        Position.Occupied => '#',
                        Position.Floor => '.',
                        _ => ' '
                    });
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        private record Vec2(int X, int Y)
        {
            public Vec2 Add(Vec2 other) => new Vec2(X + other.X, Y + other.Y);
        }
    }
}