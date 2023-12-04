using Hand = System.Collections.Generic.Dictionary<string, int>;

namespace AdventOfCode.Day02;

public sealed class Day02(ITestOutputHelper output) : TestBase(output)
{
    protected override string Day => "Day02";

    private readonly string[] _example =
    [
        "Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green",
        "Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue",
        "Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red",
        "Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red",
        "Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green"
    ];

    [Fact]
    public void ExampleOne() => DeterminePossibleGameSum(_example).Should().Be(8);
    
    [Fact]
    public void ExampleTwo() => GamePowerSum(_example).Should().Be(2286);

    [Fact]
    public void PartOne() =>
        Output.WriteLine("Possible game sum: " + DeterminePossibleGameSum(Input));

    [Fact]
    public void PartTwo() =>
        Output.WriteLine("Game power sum: " + GamePowerSum(Input));

    private static int DeterminePossibleGameSum(string[] input) => 
        input.Select((i, id) => Game.Create(id + 1, i))
            .Where(g => g.IsPossible(new Hand()
            {
                ["red"] = 12,
                ["green"] = 13,
                ["blue"] = 14
            }))
            .Sum(g => g.Id);

    private static int GamePowerSum(string[] input) =>
        input.Select((i, id) => Game.Create(id + 1, i))
            .Select(g => g.Power())
            .Sum();

    private sealed class Game
    {   
        private Game(int id, Hand[] hands)
        {
            Id = id;
            Hands = hands;
        }

        public int Id { get; }
        public Hand[] Hands { get; }

        public static Game Create(int id, string input)
        {
            var hand = input.Split(":");
            var subsets = hand[1].Split(";");
            var cubes = subsets
                .Select(r => r.Split(","))
                .Select(b => b
                    .Select(x => x.Split(" ", StringSplitOptions.RemoveEmptyEntries))
                    .ToDictionary(x => x[1], x => int.Parse(x[0])))
                .ToArray();
            return new Game(id, cubes);
        }

        public bool IsPossible(Hand hand)
        {
            foreach (var (colour, num) in hand)
            {
                if (Hands.Any(h => h.TryGetValue(colour, out var n) && n > num))
                    return false;
            }

            return true;
        }

        public int Power()
        {
            var colours = new Hand
            {
                ["red"] = 0,
                ["blue"] = 0,
                ["green"] = 0
            };
            foreach (var hand in Hands)
            foreach (var (col, _) in colours)
            {
                if (hand.TryGetValue(col, out var num))
                    colours[col] = Math.Max(colours[col], num);
            }

            return colours["red"] * colours["green"] * colours["blue"];
        }
    }
}
