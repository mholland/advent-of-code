using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day18
{
    public sealed class Day18Tests : TestBase
    {
        protected override string Day { get; } = "Day18";

        public Day18Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Theory]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 71)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 26)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632)]
        public void PartOneExamples(string input, int expectedValue) =>
            SumExpressions(new[] { input }, _basicPrecedenceRules)
                .Should()
                .Be(expectedValue);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Sum of expressions with basic rules: {SumExpressions(Input, _basicPrecedenceRules)}");

        [Theory]
        [InlineData("1 + 2 * 3 + 4 * 5 + 6", 231)]
        [InlineData("1 + (2 * 3) + (4 * (5 + 6))", 51)]
        [InlineData("2 * 3 + (4 * 5)", 46)]
        [InlineData("5 + (8 * 3 + 9 + 3 * 4 * 3)", 1445)]
        [InlineData("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 669060)]
        [InlineData("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 23340)]
        public void PartTwoExamples(string input, int expectedValue) =>
            SumExpressions(new[] { input }, _advancedPrecedenceRules)
                .Should()
                .Be(expectedValue);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Sum of expressions with advanced rules: {SumExpressions(Input, _advancedPrecedenceRules)}");

        public long SumExpressions(string[] input, IReadOnlyDictionary<char, int> precedenceRules) =>
            input.Select(i => EvaluateExpression(i, precedenceRules)).Sum();

        private readonly IReadOnlyDictionary<char, Func<long, long, long>> _operators = new Dictionary<char, Func<long, long, long>>
        {
            ['*'] = (a, b) => a * b,
            ['+'] = (a, b) => a + b
        };

        private readonly IReadOnlyDictionary<char, int> _basicPrecedenceRules = new Dictionary<char, int>
        {
            ['*'] = 1,
            ['+'] = 1
        };

        private readonly IReadOnlyDictionary<char, int> _advancedPrecedenceRules = new Dictionary<char, int>
        {
            ['*'] = 1,
            ['+'] = 2,
        };

        /// Shunting-yard Algorithm
        /// Based on: https://en.wikipedia.org/wiki/Shunting-yard_algorithm#The_algorithm_in_detail
        /// With adjustments from: https://stackoverflow.com/questions/23858073/shunting-yard-algorithm-for-immediate-evaluation
        /// and: http://wcipeg.com/wiki/Shunting_yard_algorithm#Evaluation
        public long EvaluateExpression(string expression, IReadOnlyDictionary<char, int> precedenceRules)
        {
            var output = new Stack<long>();
            var operators = new Stack<char>();
            foreach (var token in expression)
            {
                switch (token)
                {
                    case ' ':
                        continue;
                    case '(':
                        operators.Push('(');
                        continue;
                    case ')':
                        while (operators.Any() && operators.Peek() != '(')
                            ApplyOperator(output, operators);

                        if (operators.Any())
                            operators.Pop();

                        continue;
                    case '+' or '*' :
                        while (operators.Any() && operators.Peek() != '(' && precedenceRules[operators.Peek()] >= precedenceRules[token])
                            ApplyOperator(output, operators);

                        operators.Push(token);
                        continue;
                    default:
                        output.Push((long)Char.GetNumericValue(token));
                        continue;
                }
            }

            while (operators.Any())
                ApplyOperator(output, operators);

            return output.Peek();

            void ApplyOperator(Stack<long> output, Stack<char> operators)
                => output.Push(_operators[operators.Pop()].Invoke(output.Pop(), output.Pop()));
        }
    }
}