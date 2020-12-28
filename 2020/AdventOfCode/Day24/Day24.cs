using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day24
{
    public sealed class Day24Tests : TestBase
    {
        protected override string Day { get; } = "Day24";

        public Day24Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        [Fact]
        public void ExampleOne() =>
            CountBlackTiles(new[]
            {
                "sesenwnenenewseeswwswswwnenewsewsw",
                "neeenesenwnwwswnenewnwwsewnenwseswesw",
                "seswneswswsenwwnwse",
                "nwnwneseeswswnenewneswwnewseswneseene",
                "swweswneswnenwsewnwneneseenw",
                "eesenwseswswnenwswnwnwsewwnwsene",
                "sewnenenenesenwsewnenwwwse",
                "wenwwweseeeweswwwnwwe",
                "wsweesenenewnwwnwsenewsenwwsesesenwne",
                "neeswseenwwswnwswswnw",
                "nenwswwsewswnenenewsenwsenwnesesenew",
                "enewnwewneswsewnwswenweswnenwsenwsw",
                "sweneswneswneneenwnewenewwneswswnese",
                "swwesenesewenwneswnwwneseswwne",
                "enesenwswwswneneswsenwnewswseenwsese",
                "wnwnesenesenenwwnenwsewesewsesesew",
                "nenewswnwewswnenesenwnesewesw",
                "eneswnwswnwsenenwnwnwwseeswneewsenese",
                "neswnwewnwnwseenwseesewsenwsweewe",
                "wseweeenwnesenwwwswnew"
            })
            .Should()
            .Be(10);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Black tile count: {CountBlackTiles(Input)}.");

        [Fact]
        public void ExampleTwo() =>
            SimulateDays(new[]
            {
                "sesenwnenenewseeswwswswwnenewsewsw",
                "neeenesenwnwwswnenewnwwsewnenwseswesw",
                "seswneswswsenwwnwse",
                "nwnwneseeswswnenewneswwnewseswneseene",
                "swweswneswnenwsewnwneneseenw",
                "eesenwseswswnenwswnwnwsewwnwsene",
                "sewnenenenesenwsewnenwwwse",
                "wenwwweseeeweswwwnwwe",
                "wsweesenenewnwwnwsenewsenwwsesesenwne",
                "neeswseenwwswnwswswnw",
                "nenwswwsewswnenenewsenwsenwnesesenew",
                "enewnwewneswsewnwswenweswnenwsenwsw",
                "sweneswneswneneenwnewenewwneswswnese",
                "swwesenesewenwneswnwwneseswwne",
                "enesenwswwswneneswsenwnewswseenwsese",
                "wnwnesenesenenwwnenwsewesewsesesew",
                "nenewswnwewswnenesenwnesewesw",
                "eneswnwswnwsenenwnwnwwseeswneewsenese",
                "neswnwewnwnwseenwseesewsenwsweewe",
                "wseweeenwnesenwwwswnew"
            })
            .Should()
            .Be(2208);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Black tile count after 100 days: {SimulateDays(Input)}.");

        private int SimulateDays(string[] input)
        {
            var blackTiles = DiscoverInitialTiles(input);
            var adjacentDeltas = new (int X, int Y)[] { (0, -1), (1, -1), (1, 0), (-1, 0), (-1, 1), (0, 1) };

            for (var i = 0; i < 100; i++)
            {
                var mightFlip = new HashSet<(int X, int Y)>(blackTiles);
                var newTiles = new HashSet<(int X, int Y)>();
                foreach (var t in blackTiles)
                foreach (var adj in adjacentDeltas)
                    mightFlip.Add((t.X + adj.X, t.Y + adj.Y));

                foreach (var tile in mightFlip)
                {
                    var adjacentBlackTiles = adjacentDeltas.Count(a => blackTiles.Contains((tile.X + a.X, tile.Y + a.Y)));
                    if (blackTiles.Contains(tile))
                    {
                        if (adjacentBlackTiles == 1 || adjacentBlackTiles == 2)
                            newTiles.Add(tile);
                        continue;
                    }

                    if (adjacentBlackTiles == 2)
                        newTiles.Add(tile);
                }
                blackTiles = newTiles;
            }

            return blackTiles.Count;
        }

        private int CountBlackTiles(string[] input) =>
            DiscoverInitialTiles(input)
                .Count();

        /// https://www.redblobgames.com/grids/hexagons/#coordinates-offset
        private static HashSet<(int X, int Y)> DiscoverInitialTiles(string[] input)
        {
            var tiles = new HashSet<(int, int)>();
            foreach (var tile in input)
            {
                var path = tile;
                var location = (X: 0, Y: 0);
                while (!string.IsNullOrEmpty(path))
                {
                    switch (path)
                    {
                        case var p when p.StartsWith("nw"):
                            location.Y--;
                            path = path[2..];
                            continue;
                        case var p when p.StartsWith("ne"):
                            location.Y--;
                            location.X++;
                            path = path[2..];
                            continue;
                        case var p when p.StartsWith("e"):
                            location.X++;
                            path = path[1..];
                            continue;
                        case var p when p.StartsWith("w"):
                            location.X--;
                            path = path[1..];
                            continue;
                        case var p when p.StartsWith("sw"):
                            location.X--;
                            location.Y++;
                            path = path[2..];
                            continue;
                        case var p when p.StartsWith("se"):
                            location.Y++;
                            path = path[2..];
                            continue;
                    }
                }

                if (tiles.Contains(location))
                {
                    tiles.Remove(location);
                    continue;
                }

                tiles.Add(location);
            }

            return tiles;
        }
    }
}