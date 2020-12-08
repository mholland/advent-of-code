using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day07
{
    public sealed class Day07Tests : TestBase
    {
        protected override string Day { get; } = "Day07";

        public Day07Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            Day07.CountBagsContainingShinyGold(new[]
            {
                "light red bags contain 1 bright white bag, 2 muted yellow bags.",
                "dark orange bags contain 3 bright white bags, 4 muted yellow bags.",
                "bright white bags contain 1 shiny gold bag.",
                "muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.",
                "shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.",
                "dark olive bags contain 3 faded blue bags, 4 dotted black bags.",
                "vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.",
                "faded blue bags contain no other bags.",
                "dotted black bags contain no other bags."
            })
            .Should()
            .Be(4);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Number of bags containing at least one shiny gold: {Day07.CountBagsContainingShinyGold(Input)}");

        [Fact]
        public void ExampleTwo() =>
            Day07.CountBagsInsideShinyGold(new[]
            {
                "light red bags contain 1 bright white bag, 2 muted yellow bags.",
                "dark orange bags contain 3 bright white bags, 4 muted yellow bags.",
                "bright white bags contain 1 shiny gold bag.",
                "muted yellow bags contain 2 shiny gold bags, 9 faded blue bags.",
                "shiny gold bags contain 1 dark olive bag, 2 vibrant plum bags.",
                "dark olive bags contain 3 faded blue bags, 4 dotted black bags.",
                "vibrant plum bags contain 5 faded blue bags, 6 dotted black bags.",
                "faded blue bags contain no other bags.",
                "dotted black bags contain no other bags."
            })
            .Should()
            .Be(32);
    
        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Bags inside shiny gold: {Day07.CountBagsInsideShinyGold(Input)}");
    }

    public static class Day07
    {
        public static int CountBagsInsideShinyGold(string[] input)
        {
            var bags = input.Select(Bag.Create);
            var amount = 0;
            return CountBags("shiny gold", bags) - 1;

            int CountBags(string colour, IEnumerable<Bag> bags)
            {
                foreach (var b in bags.Where(b => b.Name == colour))
                {
                    amount = b.Amount;
                    foreach (var c in b.ChildBags)
                    {
                        amount += (c.Amount * CountBags(c.Name, bags));
                    }
                }
                return amount;
            }
        }

        public static int CountBagsContainingShinyGold(string[] input)
        {
            return FindMatchingBags("shiny gold", input.Select(Bag.Create))
                .Select(x => x.Name)
                .Distinct()
                .Count();

            IEnumerable<Bag> FindMatchingBags(string colour, IEnumerable<Bag> bags)
            {
                foreach (var bag in bags)
                {
                    if (!bag.ChildBags.Any(c => c.Name == colour))
                        continue;
                    
                    yield return bag;
                    
                    foreach (var child in FindMatchingBags(bag.Name, bags))
                    {
                        yield return child;
                    }
                }
            }
        }

        public sealed class Bag
        {
            private Bag(string name, int amount)
                : this(name, amount, Array.Empty<Bag>())
            {
            }

            private Bag(string name, int amount, Bag[] childBags)
            {
                Name = name;
                Amount = amount;
                ChildBags = childBags;
            }

            public string Name { get; }
            public int Amount { get; set; }
            public Bag[] ChildBags { get; }

            public static Bag Create(string definition)
            {
                var split = definition.Split("contain").Select(x => x.Trim()).ToArray();
                var outerPattern = new Regex(@"(?<colour>.*) bags");
                var childPattern = new Regex(@"(?<amount>\d) (?<colour>[\w\s]+) bag");
                var name = outerPattern.Match(split[0]).Groups["colour"].Value;

                var children = Array.Empty<Bag>();
                if (split[1] != "no other bags.")
                {
                    children = split[1]
                        .Split(",")
                        .Select(b => childPattern.Match(b))
                        .Select(m => new Bag(m.Groups["colour"].Value, int.Parse(m.Groups["amount"].Value)))
                        .ToArray();
                }

                return new Bag(name, 1, children);
            }
        }
    }
}