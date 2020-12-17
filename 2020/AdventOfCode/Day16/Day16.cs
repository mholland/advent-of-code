using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day16
{
    public sealed class Day16Tests : TestBase
    {
        protected override string Day { get; } = "Day16";

        public Day16Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            CalculateTicketScanningErrorRate(new[]
            {
                "class: 1-3 or 5-7",
                "row: 6-11 or 33-44",
                "seat: 13-40 or 45-50",
                "",
                "your ticket:",
                "7,1,14",
                "",
                "nearby tickets:",
                "7,3,47",
                "40,4,50",
                "55,2,20",
                "38,6,12"
            })
            .Should()
            .Be(71);

        [Fact]
        public void PartOne() =>
            Output.WriteLine($"Error rate: {CalculateTicketScanningErrorRate(Input)}");

        [Fact]
        public void ExampleTwo() =>
            CalculateMyTicketChecksum(new[]
            {
                "class: 0-1 or 4-19",
                "row: 0-5 or 8-19",
                "seat: 0-13 or 16-19",
                "",
                "your ticket:",
                "11,12,13",
                "",
                "nearby tickets:",
                "3,9,18",
                "15,1,5",
                "5,14,9"
            }, f => f == "class" || f == "row")
            .Should()
            .Be(132);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Ticket checksum: {CalculateMyTicketChecksum(Input, f => f.StartsWith("departure"))}");


        private int CalculateTicketScanningErrorRate(string[] input)
        {
            var (constraints, _, nearbyTickets) = ParseInput(input);

            return nearbyTickets
                .SelectMany(t => t)
                .Where(f => !constraints.Values.SelectMany(c => c).Any(c => c.IsValueWithin(f)))
                .Sum();
        }

        private long CalculateMyTicketChecksum(string[] input, Func<string, bool> fieldSelector)
        {
            var (constraints, myTicket, nearbyTickets) = ParseInput(input);
            var validTickets = nearbyTickets
                .Where(fields => fields
                    .All(f => constraints.Values
                        .SelectMany(c => c)
                        .Any(c => c.IsValueWithin(f))))
                .ToArray();

            var validMappings = new List<(string Field, int Index)>();
            foreach (var (fieldName, cons) in constraints) 
            {
                for (var i = 0; i < nearbyTickets.Max(x => x.Length); i++) 
                {
                    var satisfiesConstraint = validTickets
                        .Select(f => f[i])
                        .All(f => cons.Any(c => c.IsValueWithin(f)));
                    if (satisfiesConstraint)
                        validMappings.Add((fieldName, i));
                }
            }

            var finalMappings = new Dictionary<string, int>();
            while (validMappings.Count() > 0)
            {
                foreach (var field in validMappings.GroupBy(m => m.Field).Where(g => g.Count() == 1))
                {
                    var mapping = field.First();
                    finalMappings.Add(mapping.Field, mapping.Index);
                    validMappings = validMappings.Where(m => m.Field != mapping.Field && m.Index != mapping.Index).ToList();
                }
            }

            return finalMappings
                .Keys
                .Where(fieldSelector)
                .Select(m => finalMappings[m])
                .Select(i => myTicket[i])
                .Aggregate(1L, (agg, cur) => agg * cur);
        }

        (IDictionary<string, Constraint[]> Constraints, int[] MyTicket, IList<int[]> NearbyTickets) ParseInput(string[] input)
        {
            var constraints = new Dictionary<string, Constraint[]>();
            var i = 0;
            while (input[i] != string.Empty)
            {
                var fieldName = input[i].Substring(0, input[i].IndexOf(":"));
                var matches = Regex.Matches(input[i], @"(\d+\-\d+)");

                constraints.Add(fieldName, new[] { Constraint.Parse(matches[0].Value), Constraint.Parse(matches[1].Value) });

                i++;
            }

            var myTicketIndex = Array.IndexOf(input, "your ticket:") + 1;
            var myTicket = input[myTicketIndex].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

            var nearbyTickets = new List<int[]>();
            i = Array.IndexOf(input, "nearby tickets:") + 1;
            while (i < input.Length)
            {
                nearbyTickets.Add(input[i].Split(',').Select(int.Parse).ToArray());
                i++;
            }

            return (constraints, myTicket, nearbyTickets);
        }


        public record Constraint(int Low, int High)
        {
            public bool IsValueWithin(int value) =>
                value >= Low && value <= High;

            public static Constraint Parse(string range) =>
                new Constraint(int.Parse(range.Split('-')[0]), int.Parse(range.Split('-')[1]));
        }
    }
}