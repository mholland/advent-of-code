using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day04
{
    public sealed class Day04Tests
    {
        private readonly ITestOutputHelper _output;

        public Day04Tests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void PassportParseDeserializesCorrectly() =>
            Day04.Passport.Parse(@"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm")
                .Fields
                .Should()
                .HaveCount(8)
                .And
                .BeEquivalentTo(new Dictionary<string, string>
                {
                    ["ecl"] = "gry",
                    ["pid"] = "860033327",
                    ["eyr"] = "2020",
                    ["hcl"] = "#fffffd",
                    ["byr"] = "1937",
                    ["iyr"] = "2017",
                    ["cid"] = "147",
                    ["hgt"] = "183cm"
                });

        [Fact]
        public void ValidPassportValidatesSuccessfully() =>
            Day04.Passport.Parse(@"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm")
                .IsValid()
                .Should()
                .BeTrue();

        [Fact]
        public void InvalidPassportValidatesUnsuccessfully() =>
            Day04.Passport.Parse(@"iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929")
                .IsValid()
                .Should()
                .BeFalse("it is missing the hgt field.");

        [Fact]
        public void ExampleOne() =>
            Day04.CountValidPasswords(@"ecl:gry pid:860033327 eyr:2020 hcl:#fffffd
byr:1937 iyr:2017 cid:147 hgt:183cm

iyr:2013 ecl:amb cid:350 eyr:2023 pid:028048884
hcl:#cfa07d byr:1929

hcl:#ae17e1 iyr:2013
eyr:2024
ecl:brn pid:760753108 byr:1931
hgt:179cm

hcl:#cfa07d eyr:2025 pid:166559648
iyr:2011 ecl:brn hgt:59in")
                .Should()
                .Be(2);

        public static IEnumerable<object[]> ValidPassports =>
            new List<object[]>
            {
                new object[] { @"pid:087499704 hgt:74in ecl:grn iyr:2012 eyr:2030 byr:1980
hcl:#623a2f" },
                new object[] { @"eyr:2029 ecl:blu cid:129 byr:1989
iyr:2014 pid:896056539 hcl:#a97842 hgt:165cm" },
                new object[] { @"hcl:#888785
hgt:164cm byr:2001 iyr:2015 cid:88
pid:545766238 ecl:hzl
eyr:2022" },
                new object[] { @"iyr:2010 hgt:158cm hcl:#b6652a ecl:blu byr:1944 eyr:2021 pid:093154719" }
            };

        [Theory]
        [MemberData(nameof(ValidPassports))]
        public void ValidPassportsValidateSuccessfullyWithAdditionalRules(string passport) =>
            Day04
                .Passport
                .Parse(passport)
                .IsValid()
                .Should()
                .BeTrue();

        public static IEnumerable<object[]> InvalidPassports =>
            new List<object[]>
            {
                new object[] { @"eyr:1972 cid:100
hcl:#18171d ecl:amb hgt:170 pid:186cm iyr:2018 byr:1926" },
                new object[] { @"iyr:2019
hcl:#602927 eyr:1967 hgt:170cm
ecl:grn pid:012533040 byr:1946" },
                new object[] { @"hcl:dab227 iyr:2012
ecl:brn hgt:182cm pid:021572410 eyr:2020 byr:1992 cid:277" },
                new object[] { @"hgt:59cm ecl:zzz
eyr:2038 hcl:74454a iyr:2023
pid:3556412378 byr:2007" }
            };

        [Theory]
        [MemberData(nameof(InvalidPassports))]
        public void InvalidPassportsValidateUnsuccessfullyWithAdditionalRules(string passport) =>
            Day04
                .Passport
                .Parse(passport)
                .IsValid()
                .Should()
                .BeFalse();

        [Fact]
        public void CountValidPasswords()
        {
            var input = File.ReadAllText(Path.Combine("Day04", "input.txt"));
            _output.WriteLine($"# of valid passports: {Day04.CountValidPasswords(input)}");
        }
    }

    public static class Day04
    {
        public static int CountValidPasswords(string input) =>
            input
                .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Select(Passport.Parse)
                .Count(p => p.IsValid());

        public sealed class Passport
        {
            private static readonly IReadOnlyDictionary<string, Func<string, bool>> _fieldValidators =
                new Dictionary<string, Func<string, bool>>
                {
                    ["byr"] = birthYear => IsWithinRange(birthYear, 1920, 2002),
                    ["iyr"] = issueYear => IsWithinRange(issueYear, 2010, 2020),
                    ["eyr"] = expirationYear => IsWithinRange(expirationYear, 2020, 2030),
                    ["hgt"] = IsHeightValid,
                    ["hcl"] = hairColour => MatchesPattern(hairColour, @"^#[a-f0-9]{6}$"),
                    ["ecl"] = IsEyeColourValid,
                    ["pid"] = passportId => MatchesPattern(passportId, @"^[0-9]{9}$"),
                    ["cid"] = _ => true
                };

            private static readonly string[] _requiredFields =
                _fieldValidators
                    .Keys
                    .Except(new[]{"cid"})
                    .ToArray();

            public static Passport Parse(string passport)
            {
                var fields = passport.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .SelectMany(l => l.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                    .Select(f => f.Split(':'))
                    .ToDictionary(f => f[0], f => f[1]);

                return new Passport(fields);
            }

            private Passport(Dictionary<string, string> fields) => Fields = fields;

            public IReadOnlyDictionary<string, string> Fields { get; } = new Dictionary<string, string>();

            private static bool IsWithinRange(string value, int lower, int upper)
            {
                var number = int.Parse(value);
                return number >= lower && number <= upper;
            }

            private static bool MatchesPattern(string input, string pattern) =>
                Regex.IsMatch(input, pattern);

            private static bool IsEyeColourValid(string eyeColour) =>
                new[] { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" }.Contains(eyeColour);

            private static bool IsHeightValid(string height)
            {
                var pattern = new Regex(@"^(\d{2,3})(cm|in)$");
                var match = pattern.Match(height);
                if (!match.Success) return false;

                var value = int.Parse(match.Groups[1].Value);
                var unit = match.Groups[2].Value;

                return (unit, value) switch
                {
                    ("cm", >= 150 and <= 193) or 
                    ("in", >= 59 and <= 76) => true,
                    _ => false
                };
            }

            public bool IsValid()
            {
                if (!_requiredFields.All(k => Fields.ContainsKey(k)))
                    return false;

                return Fields.All(kvp =>
                {
                    var (field, value) = kvp;
                    if (_fieldValidators.TryGetValue(field, out var validator))
                        return validator.Invoke(value);

                    return false;
                });
            }
        }
    }
}