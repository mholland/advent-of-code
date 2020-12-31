using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace AdventOfCode.Day20
{
    public sealed class Day20Tests : TestBase
    {
        protected override string Day { get; } = "Day20";

        public Day20Tests(ITestOutputHelper output)
            : base(output)
        {
        }

        private readonly string[] _exampleOneInput = new[]
        {
            "Tile 2311:",
            "..##.#..#.",
            "##..#.....",
            "#...##..#.",
            "####.#...#",
            "##.##.###.",
            "##...#.###",
            ".#.#.#..##",
            "..#....#..",
            "###...#.#.",
            "..###..###",
            "",
            "Tile 1951:",
            "#.##...##.",
            "#.####...#",
            ".....#..##",
            "#...######",
            ".##.#....#",
            ".###.#####",
            "###.##.##.",
            ".###....#.",
            "..#.#..#.#",
            "#...##.#..",
            "",
            "Tile 1171:",
            "####...##.",
            "#..##.#..#",
            "##.#..#.#.",
            ".###.####.",
            "..###.####",
            ".##....##.",
            ".#...####.",
            "#.##.####.",
            "####..#...",
            ".....##...",
            "",
            "Tile 1427:",
            "###.##.#..",
            ".#..#.##..",
            ".#.##.#..#",
            "#.#.#.##.#",
            "....#...##",
            "...##..##.",
            "...#.#####",
            ".#.####.#.",
            "..#..###.#",
            "..##.#..#.",
            "",
            "Tile 1489:",
            "##.#.#....",
            "..##...#..",
            ".##..##...",
            "..#...#...",
            "#####...#.",
            "#..#.#.#.#",
            "...#.#.#..",
            "##.#...##.",
            "..##.##.##",
            "###.##.#..",
            "",
            "Tile 2473:",
            "#....####.",
            "#..#.##...",
            "#.##..#...",
            "######.#.#",
            ".#...#.#.#",
            ".#########",
            ".###.#..#.",
            "########.#",
            "##...##.#.",
            "..###.#.#.",
            "",
            "Tile 2971:",
            "..#.#....#",
            "#...###...",
            "#.#.###...",
            "##.##..#..",
            ".#####..##",
            ".#..####.#",
            "#..#.#..#.",
            "..####.###",
            "..#.#.###.",
            "...#.#.#.#",
            "",
            "Tile 2729:",
            "...#.#.#.#",
            "####.#....",
            "..#.#.....",
            "....#..#.#",
            ".##..##.#.",
            ".#.####...",
            "####.#.#..",
            "##.####...",
            "##..#.##..",
            "#.##...##.",
            "",
            "Tile 3079:",
            "#.#.#####.",
            ".#..######",
            "..#.......",
            "######....",
            "####.#..#.",
            ".#...#.##.",
            "#.#####.##",
            "..#.###...",
            "..#.......",
            "..#.###...",
        };

        [Fact]
        public void ExampleOne() =>
            CalculateCornerChecksum(_exampleOneInput)
                .Should()
                .Be(20899048083289);

        [Fact]
        public void PartOne() =>
            Output
                .WriteLine($"Corner checksum: {CalculateCornerChecksum(Input)}");

        [Fact]
        public void ExampleTwo() =>
            CalculateWaterRoughness(_exampleOneInput)
                .Should()
                .Be(273);

        [Fact]
        public void PartTwo() =>
            Output
                .WriteLine($"Water roughness: {CalculateWaterRoughness(Input)}");

        private int CalculateWaterRoughness(string[] input)
        {
            var tiles = ParseTiles(input);
            var tileLookup = tiles.ToDictionary(t => t.Id, t => t);
            var adjoinableTiles = FindAdjoinableEdges(tiles);
            var corners = adjoinableTiles
                .ToLookup(kvp => kvp.Key.TileId, kvp => kvp.Value)
                .ToDictionary(l => l.Key, g => g.ToArray())
                .Where(e => e.Value.Count() / 2 == 2)
                .ToArray();

            var size = (int)Math.Sqrt(tiles.Count());
            var arrangedTiles = new Tile[size, size];

            arrangedTiles[0, 0] = FindTopLeft(corners, tileLookup);

            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                if ((x, y) == (0, 0))
                    continue;

                var previous = x == 0 ? arrangedTiles[x, y - 1].BottomEdge() : arrangedTiles[x - 1, y].RightEdge();
                var adjoinable = adjoinableTiles.First(a => a.Key.Checksum == previous.Checksum && a.Key.TileId == previous.TileId);

                arrangedTiles[x, y] = OrientTile(tileLookup[adjoinable.Value.TileId], adjoinable.Value, previous);
            }

            var image = Image.Extract(arrangedTiles);
        #if DEBUG
            image.Print();
        #endif
            return image.CalculateWaterRoughness();

            Tile FindTopLeft(KeyValuePair<int, Tile.Edge[]>[] corners, IDictionary<int, Tile> tileLookup)
            {
                // In case there happens to be a tile that fits in the top left corner like in the example
                // Otherwise rotate one until it did.
                var topLeftCandidates = corners.Where(corner => adjoinableTiles
                    .Where(a => a.Key.TileId == corner.Key)
                    .All(a => a.Key.Type == Tile.EdgeType.Bottom || a.Key.Type == Tile.EdgeType.Right));

                if (topLeftCandidates.Any())
                    return tileLookup[topLeftCandidates.First().Key];

                var designated = corners.First();
                var topLeft = tileLookup[designated.Key];
                var adjoinableEdgeChecksums = designated.Value.Select(f => f.Checksum);
                do
                {
                    topLeft = topLeft.Rotate();
                } while (!adjoinableEdgeChecksums.Contains(topLeft.RightEdge().Checksum)
                    && !adjoinableEdgeChecksums.Contains(topLeft.BottomEdge().Checksum));

                return topLeft;
            }
        }

        Tile OrientTile(Tile toPlace, Tile.Edge matchingEdge, Tile.Edge toMatch)
        {
            while (true)
            {
                if (CurrentEdge(toMatch, toPlace).Checksum == toMatch.Checksum)
                    return toPlace;

                toPlace = toPlace.Rotate();
                if (CurrentEdge(toMatch, toPlace).Checksum == toMatch.Checksum)
                    return toPlace;

                var flipped = toMatch.Type == Tile.EdgeType.Bottom ? toPlace.FlipHorizontal() : toPlace.FlipVertical();
                if (CurrentEdge(toMatch, flipped).Checksum == toMatch.Checksum)
                    return flipped;
            }

            Tile.Edge CurrentEdge(Tile.Edge toMatch, Tile toPlace) =>
                toMatch.Type == Tile.EdgeType.Bottom ? toPlace.TopEdge() : toPlace.LeftEdge();
        }

        private sealed class Image
        {
            private Image(bool[,] image) => Pixels = image;

            public bool[,] Pixels { get; }

            public static Image Extract(Tile[,] arrangedTiles)
            {
                const int tileSize = 8;
                var size = arrangedTiles.GetLength(0) * tileSize;
                var image = new bool[size, size];

                for (var x = 0; x < arrangedTiles.GetLength(0); x++)
                for (var y = 0; y < arrangedTiles.GetLength(1); y++)
                for (var xx = 1; xx <= tileSize; xx++)
                for (var yy = 1; yy <= tileSize; yy++)
                    image[x * tileSize + xx - 1, y * tileSize + yy - 1] = arrangedTiles[x, y].Image[xx, yy];

                return new Image(image);
            }

            private (int X, int Y)[] MonsterPattern()
            {
                var pattern = new[]
                {
                    "                  # ",
                    "#    ##    ##    ###",
                    " #  #  #  #  #  #   "
                };

                var points = new List<(int X, int Y)>();

                for (var y = 0; y < pattern.Length; y++)
                for (var x = 0; x < pattern[y].Length; x++)
                {
                    if (pattern[y][x] == '#')
                        points.Add((x, y));
                }

                return points.ToArray();
            }

            public int CalculateWaterRoughness()
            {
                var totalWave = 0;
                for (var y = 0; y < Pixels.GetLength(0); y++)
                for (var x = 0; x < Pixels.GetLength(1); x++)
                    if (Pixels[x, y]) totalWave++;

                var monsterPoints = new HashSet<(int X, int Y)>();
                var monsterPattern = MonsterPattern();

                var toCheck = this;
                for (var i = 0; i < 4; i++)
                {
                #if DEBUG
                    Console.WriteLine("Rotation: {0}", i);
                    toCheck.Print();
                    Console.WriteLine();
                #endif
                    CheckForMonsters(toCheck, monsterPattern, monsterPoints);

                    var flipped = toCheck.Flip();
                #if DEBUG
                    Console.WriteLine("Rotation: {0} flipped", i);
                    flipped.Print();
                    Console.WriteLine();
                #endif
                    CheckForMonsters(flipped, monsterPattern, monsterPoints);

                    toCheck = toCheck.Rotate();
                }

                return totalWave - monsterPoints.Count();

                void CheckForMonsters(Image image, (int X, int Y)[] pattern, HashSet<(int X, int Y)> points)
                {
                    for (var y = 0; y < image.Pixels.GetLength(0) - (pattern.Max(p => p.Y) + 1); y++)
                    for (var x = 0; x < image.Pixels.GetLength(1) - (pattern.Max(p => p.X) + 1); x++)
                    {
                        var pts = pattern.Select(m => (X: m.X + x, Y: m.Y + y));
                        if (pts.All(p => image.Pixels[p.X, p.Y]))
                            foreach (var point in pts)
                                points.Add(point);
                    }
                }
            }

            public Image Rotate()
            {
                var newImage = new bool[Pixels.GetLength(0), Pixels.GetLength(1)];
                for (var y = 0; y < Pixels.GetLength(0); y++)
                for (var x = 0; x < Pixels.GetLength(1); x++)
                {
                    newImage[x, y] = Pixels[y, Pixels.GetLength(0) - 1 - x];
                }

                return new Image(newImage);
            }

            public Image Flip()
            {
                var newImage = new bool[Pixels.GetLength(0), Pixels.GetLength(1)];
                for (var y = 0; y < Pixels.GetLength(0); y++)
                for (var x = 0; x < Pixels.GetLength(1); x++)
                {
                    newImage[x, y] = Pixels[Pixels.GetLength(1) - 1 - x, y];
                }

                return new Image(newImage);
            }

            public void Print()
            {
                for (var y = 0; y < Pixels.GetLength(1); y++)
                {
                    for (var x = 0; x < Pixels.GetLength(0); x++)
                    {
                        Console.Write(Pixels[x, y] ? '#' : '.');
                    }
                    Console.WriteLine();
                }
            }
        }

        private long CalculateCornerChecksum(string[] input)
        {
            var adjoinableEdges = FindAdjoinableEdges(ParseTiles(input));

            // Edge tiles only have two adjoinable edges,
            // the division is to account for the flips which will also match on the same edge
            return adjoinableEdges
                .ToLookup(kvp => kvp.Key.TileId, kvp => kvp.Value)
                .ToDictionary(l => l.Key, g => g.ToArray())
                .Where(e => e.Value.Count() / 2 == 2)
                .Aggregate(1L, (agg, cur) => agg * cur.Key);
        }

        private IDictionary<Tile.Edge, Tile.Edge> FindAdjoinableEdges(IEnumerable<Tile> tiles)
        {
            var adjoinableEdges = new Dictionary<Tile.Edge, Tile.Edge>();
            foreach (var tile in tiles)
            {
                var edges = tile.AllEdges();
                var otherTiles = tiles.Where(t => t != tile);
                foreach (var edge in edges)
                foreach (var other in otherTiles)
                foreach (var otherEdge in other.AllEdges())
                {
                    if (edge.Checksum == otherEdge.Checksum)
                        adjoinableEdges[edge] = otherEdge;
                }
            }

            return adjoinableEdges;
        }

        private IEnumerable<Tile> ParseTiles(string[] input)
        {
            var tileInput = input.Where(l => !string.IsNullOrEmpty(l));
            var tiles = new List<Tile>();
            do
            {
                var tile = Tile.Parse(tileInput.Take(11).ToArray());
                tiles.Add(tile);
                tileInput = tileInput.Skip(11);
            }
            while (tileInput.Any());

            return tiles;
        }

        private sealed class Tile
        {
            private Tile(int id, bool[,] image)
            {
                Id = id;
                Image = image;
            }

            public int Id { get; }
            public bool[,] Image { get; }

            public static Tile Parse(string[] input)
            {
                var id = int.Parse(Regex.Match(input[0], @"(\d+)").Groups[1].Value);
                var size = 10;
                var image = new bool[size, size];
                for (var x = 0; x < size; x++)
                for (var y = 0; y < size; y++)
                    image[x, y] = input[y + 1][x] == '#';

                return new Tile(id, image);
            }

            public Tile Rotate()
            {
                var width = Image.GetLength(0);
                var height = Image.GetLength(1);
                var newImage = new bool[width, height];
                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                    newImage[x, y] = Image[9 - y, x];

                return new Tile(Id, newImage);
            }

            public Tile FlipHorizontal()
            {
                var width = Image.GetLength(0);
                var height = Image.GetLength(1);
                var newImage = new bool[width, height];
                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    newImage[x, y] = Image[9 - x, y];
                }

                return new Tile(Id, newImage);
            }

            public Tile FlipVertical()
            {
                var width = Image.GetLength(0);
                var height = Image.GetLength(1);
                var newImage = new bool[width, height];
                for (var x = 0; x < width; x++)
                for (var y = 0; y < height; y++)
                {
                    newImage[x, y] = Image[x, 9 - y];
                }

                return new Tile(Id, newImage);
            }

            public Edge RightEdge()
            {
                var edge = 0;
                for (var i = 0; i < Image.GetLength(1); i++)
                    if (Image[9, i])
                        edge |= 1 << i;

                return new Edge(Id, EdgeType.Right, edge, false);
            }

            public Edge LeftEdge()
            {
                var edge = 0;
                for (var i = 0; i < Image.GetLength(1); i++)
                    if (Image[0, i])
                        edge |= 1 << i;

                return new Edge(Id, EdgeType.Left, edge, false);
            }

            public Edge TopEdge()
            {
                var edge = 0;
                for (var i = 0; i < Image.GetLength(0); i++)
                    if (Image[i, 0])
                        edge |= 1 << i;

                return new Edge(Id, EdgeType.Top, edge, false);
            }

            public Edge BottomEdge()
            {
                var edge = 0;
                for (var i = 0; i < Image.GetLength(0); i++)
                    if (Image[i, 9])
                        edge |= 1 << i;

                return new Edge(Id, EdgeType.Bottom, edge, false);
            }

            public IEnumerable<Edge> AllEdges()
            {
                int topFlipped = default, rightFlipped = default, bottomFlipped = default, leftFlipped = default;
                for (var i = 0; i < Image.GetLength(0); i++)
                {
                    if (Image[i, 0])
                        topFlipped |= 1 << (9 - i);

                    if (Image[i, 9])
                        bottomFlipped |= 1 << (9 - i);

                    if (Image[0, i])
                        leftFlipped |= 1 << (9 - i);

                    if (Image[9, i])
                        rightFlipped |= 1 << (9 - i);
                }

                return new[]
                {
                    TopEdge(),
                    new Edge(Id, EdgeType.Top, topFlipped, true),
                    RightEdge(),
                    new Edge(Id, EdgeType.Right, rightFlipped, true),
                    BottomEdge(),
                    new Edge(Id, EdgeType.Bottom, bottomFlipped, true),
                    LeftEdge(),
                    new Edge(Id, EdgeType.Left, leftFlipped, true)
                };
            }

            public record Edge(int TileId, EdgeType Type, int Checksum, bool Flipped);

            public enum EdgeType
            {
                Top,
                Right,
                Bottom,
                Left
            }
        }
    }
}