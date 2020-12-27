using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day21
{
    public sealed class Day21Tests : TestBase
    {
        protected override string Day { get; } = "Day21";

        public Day21Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            AnalyseIngredients(new[]
            {
                "mxmxvkd kfcds sqjhc nhms (contains dairy, fish)",
                "trh fvjkl sbzzf mxmxvkd (contains dairy)",
                "sqjhc fvjkl (contains soy)",
                "sqjhc mxmxvkd sbzzf (contains fish)"
            })
            .AllergenFree
            .Should()
            .Be(5);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Occurrences of allergen free ingredients: {AnalyseIngredients(Input).AllergenFree}");

        [Fact]
        public void ExampleTwo() =>
            AnalyseIngredients(new[]
            {
                "mxmxvkd kfcds sqjhc nhms (contains dairy, fish)",
                "trh fvjkl sbzzf mxmxvkd (contains dairy)",
                "sqjhc fvjkl (contains soy)",
                "sqjhc mxmxvkd sbzzf (contains fish)"
            })
            .Dangerzone
            .Should()
            .Be("mxmxvkd,sqjhc,fvjkl");

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Dangerzone ingredients: {AnalyseIngredients(Input).Dangerzone}");

        private (int AllergenFree, string Dangerzone) AnalyseIngredients(string[] input)
        {
            var ingredientLists = ParseIngredients(input);

            var allergens = ingredientLists
                .SelectMany(i => i.Allergens)
                .Distinct();
            var candidates = new List<(string Allergen, IList<string> Ingredients)>();
            foreach (var allergen in allergens)
            {
                var candidateLists = ingredientLists
                    .Where(l => l.Allergens.Contains(allergen));

                var ingredientsInAllLists = candidateLists
                    .Skip(1)
                    .Aggregate(candidateLists.First().Ingredients, (agg, cur) => agg.Intersect(cur.Ingredients));

                candidates.Add((allergen, ingredientsInAllLists.ToList()));
            }

            var allergenIngredientMap = new Dictionary<string, string>();
            while (candidates.Any(l => l.Ingredients.Count() > 1))
            {
                var identifiedAllergens = candidates.Where(a => a.Ingredients.Count() == 1);
                foreach (var item in identifiedAllergens)
                {
                    var identifiedIngredient = item.Ingredients.Single();
                    allergenIngredientMap.Add(item.Allergen, identifiedIngredient);
                    candidates.ForEach(c => c.Ingredients.Remove(identifiedIngredient));
                    candidates = candidates.Where(c => c.Ingredients.Count() > 0).ToList();
                }
            }

            var withoutAllergens =
                ingredientLists
                    .SelectMany(l => l.Ingredients)
                    .Except(allergenIngredientMap.Values)
                    .Distinct()
                    .ToArray();

            var allergenFreeCount = ingredientLists.Aggregate(0, (agg, cur) => agg + cur.Ingredients.Count(withoutAllergens.Contains));
            var dangerzone = string.Join(",", allergenIngredientMap.OrderBy(x => x.Key).Select(x => x.Value));
            return (allergenFreeCount, dangerzone);
        }

        private IReadOnlyList<IngredientList> ParseIngredients(string[] input)
        {
            var regex = new Regex(@"(\w+)+", RegexOptions.Compiled);
            var matchCollections = input.Select(i => regex.Matches(i));
            var ingredientLists = new List<IngredientList>();
            foreach (var collection in matchCollections)
            {
                var listValues = collection.Select(m => m.Value).ToArray();
                var idx = Array.IndexOf(listValues, "contains");
                var ingredients = listValues[..idx];
                var allergens = listValues[(idx+1)..];
                ingredientLists.Add(new IngredientList(ingredients, allergens));
            }

            return ingredientLists;
        }

        private record IngredientList(IEnumerable<string> Ingredients, IEnumerable<string> Allergens);
    }
}