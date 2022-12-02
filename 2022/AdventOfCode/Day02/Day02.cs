namespace AdventOfCode.Day02;

public sealed class Day02 : TestBase
{
    protected override string Day => "Day02";

    public Day02(ITestOutputHelper output)
        : base(output)
    {
    }

    [Fact]
    public void ExampleOne() =>
        Output.WriteLine(CalculateTotalScore(new[]
        {
            "A Y",
            "B X",
            "C Z"
        }).ToString());
    
    [Fact]
    public void CalculatePartOne() =>
        Output.WriteLine($"Total score: {CalculateTotalScore(Input)}");

    [Fact]
    public void ExampleTwo() =>
        Output.WriteLine(CalculateDesiredScore(new[]
        {
            "A Y",
            "B X",
            "C Z"
        }).ToString());

    [Fact]
    public void CalculatePartTwo() =>
        Output.WriteLine($"Total desired score: {CalculateDesiredScore(Input)}");

    private IEnumerable<Round> ParseInput(string[] input) =>
        input
            .Select(m => m.Split(" ")).Select(x => (Opponent: x[0], Response: x[1]))
            .Select(r => Round.Parse(r.Opponent, r.Response));

    private int CalculateTotalScore(string[] input) =>
        ParseInput(input).Select(r => r.CalculateScore()).Sum();

    private int CalculateDesiredScore(string[] input) =>
        ParseInput(input).Select(r => r.CalculateDesiredScore()).Sum();    

    private sealed class Round
    {
        private readonly Move _opponent;
        private readonly Move _response;
        private readonly DesiredResult _result;

        private Round(Move opponent, Move response, DesiredResult result)
        {
            _opponent = opponent;
            _response = response;
            _result = result;
        }

        public static Round Parse(string opponent, string response)
        {
            return new Round(
                ParseMove(opponent), 
                ParseMove(response), 
                response switch 
                {
                    "X" => DesiredResult.Loss,
                    "Y" => DesiredResult.Draw,
                    "Z" => DesiredResult.Win,
                    _ => throw new Exception("Bad result")
                });

            Move ParseMove(string move) =>
            move switch
            {
                "A" or "X" => Move.Rock,
                "B" or "Y" => Move.Paper,
                "C" or "Z" => Move.Scissors,
                _ => throw new Exception("Bad move")
            };
        }

        public int CalculateDesiredScore()
        {
            var result = _result switch
            {
                DesiredResult.Win => 6,
                DesiredResult.Draw => 3,
                DesiredResult.Loss => 0,
                _ => 0
            };

            var response = (_opponent, _result) switch
            {
                (Move.Rock, DesiredResult.Win) => 2,
                (Move.Rock, DesiredResult.Loss) => 3,
                (Move.Rock, DesiredResult.Draw) => 1,
                (Move.Paper, DesiredResult.Win) => 3,
                (Move.Paper, DesiredResult.Loss) => 1,
                (Move.Paper, DesiredResult.Draw) => 2,
                (Move.Scissors, DesiredResult.Win) => 1,
                (Move.Scissors, DesiredResult.Loss) => 2,
                (Move.Scissors, DesiredResult.Draw) => 3,
                _ => 0
            };

            return result + response;
        }

        public int CalculateScore()
        {
            var responseScore = CalculateResponseScore();
            var roundResult = CalculateResult();

            return responseScore + roundResult;
        }

        private int CalculateResult() => 
            (_opponent, _response) switch
                {
                    (Move.Rock, Move.Rock) => 3,
                    (Move.Paper, Move.Paper) => 3,
                    (Move.Scissors, Move.Scissors) => 3,
                    (Move.Rock, Move.Paper) => 6,
                    (Move.Rock, Move.Scissors) => 0,
                    (Move.Paper, Move.Scissors) => 6,
                    (Move.Paper, Move.Rock) => 0,
                    (Move.Scissors, Move.Rock) => 6,
                    (Move.Scissors, Move.Paper) => 0,
                    (_, _) => 0
                };

        private int CalculateResponseScore() =>
            _response switch
            {
                Move.Rock => 1,
                Move.Paper => 2,
                Move.Scissors => 3,
                _ => 0
            };

        private enum Move
        {
            Rock,
            Paper,
            Scissors
        }

        private enum DesiredResult
        {
            Win,
            Loss,
            Draw
        }
    }
}