using System.Text.RegularExpressions;

namespace AOC._2020;

public class Day20
{
    private readonly string _input = File.ReadAllText("Inputs/Day20.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        var tiles = AssemblePuzzle(_input);
        (
            tiles.First().First().Id *
            tiles.First().Last().Id *
            tiles.Last().First().Id *
            tiles.Last().Last().Id
        ).Should().Be(27803643063307L);
    }

    [Fact]
    public void Part2()
    {
        var image = MergeTiles(AssemblePuzzle(_input));
        
        var monster = new[]{
            "                  # ",
            "#    ##    ##    ###",
            " #  #  #  #  #  #   "
        };

        var res = 0;
        while (res == 0)
        {
            var monsterCount = MatchCount(image, monster);
            if (monsterCount > 0)
            {
                var hashCountInImage = image.ToString().Count(ch => ch == '#');
                var hashCountInMonster = string.Join("\n", monster).Count(ch => ch == '#');
                res = hashCountInImage - monsterCount * hashCountInMonster;
            }
            
            image.ChangeOrientation();
        }

        res.Should().Be(1644);
    }

    private static Tile[][] AssemblePuzzle(string input)
    {
        var tiles = ParseTiles(input);
        
        // Collects tiles sharing a common edge
        // Due to the way the input is created, the list contains
        // - one item for tiles on the edge or
        // - tow for inner pieces.
        var pairs = new Dictionary<string, List<Tile>>();
        foreach (var tile in tiles)
        {
            for (var i = 0; i < 8; i++)
            {
                var pattern = tile.Top();
                if (!pairs.ContainsKey(pattern))
                {
                    pairs[pattern] = [];
                }
                pairs[pattern].Add(tile);
                tile.ChangeOrientation();
            }
        }

        // once the corner is fixed we can always find a unique tile that matches the on to the left & above
        // just fill up the tile set one by one
        var size = (int)Math.Sqrt(tiles.Length);
        var puzzle = new Tile[size][];
        for (var iRow = 0; iRow < size; iRow++)
        {
            puzzle[iRow] = new Tile[size];
            for (var iCol = 0; iCol < size; iCol++)
            {
                var above = iRow == 0 ? null : puzzle[iRow - 1][iCol];
                var left = iCol == 0 ? null : puzzle[iRow][iCol - 1];
                puzzle[iRow][iCol] = PutTileInPlace(above, left);
            }
        }

        return puzzle;

        bool IsEdge(string pattern) => pairs[pattern].Count == 1;
        Tile GetNeighbour(Tile tile, string pattern) => pairs[pattern].SingleOrDefault(other => other != tile);
        Tile PutTileInPlace(Tile above, Tile left)
        {
            if (above is null && left is null)
            {
                // find top-left corner
                foreach (var tile in tiles)
                {
                    for (var i = 0; i < 8; i++)
                    {
                        if (IsEdge(tile.Top()) && IsEdge(tile.Left()))
                        {
                            return tile;
                        }
                        tile.ChangeOrientation();
                    }
                }
            }
            else
            {
                // we know the tile from the inversion structure, just need to find its orientation.
                var tile = above is not null ? GetNeighbour(above, above.Bottom()) : GetNeighbour(left, left.Right());
                while (true)
                {
                    var topMatch = above is null ? IsEdge(tile.Top()) : tile.Top() == above.Bottom();
                    var leftMatch = left is null ? IsEdge(tile.Left()) : tile.Left() == left.Right();

                    if (topMatch && leftMatch)
                    {
                        return tile;
                    }
                    tile.ChangeOrientation();
                }
            }

            throw new Exception();
        }
    }

    private static Tile[] ParseTiles(string input) => (
        from block in input.Split("\n\n")
        let lines = block.Split("\n")
        let id = Regex.Match(lines[0], "\\d+").Value
        let image = lines.Skip(1).Where(x => x != "").ToArray()
        select new Tile(int.Parse(id), image)
    ).ToArray();
    
    private static Tile MergeTiles(Tile[][] tiles)
    {
        // create a big tile leaving out the borders
        var image = new List<string>();
        var tileSize = tiles[0][0].Size;
        var tileCount = tiles.Length;
        for (var iRow = 0; iRow < tileCount; iRow++) {
            for (var i = 1; i < tileSize - 1; i++) {
                var st = "";
                for (var iCol = 0; iCol < tileCount; iCol++) {
                    st += tiles[iRow][iCol].Row(i).Substring(1, tileSize - 2);
                }
                image.Add(st);
            }
        }
        return new Tile(42, image.ToArray());
    }

    private static int MatchCount(Tile image, params string[] pattern)
    {
        var res = 0;
        var (cColP, cRowP) = (pattern[0].Length, pattern.Length);
        for (var iRow = 0; iRow < image.Size - cRowP; iRow++) 
        for (var iCol = 0; iCol < image.Size - cColP ; iCol++) {
            if(Match()) {
                res++;
            }

            continue;

            bool Match() {
                for (var iColP = 0; iColP < cColP; iColP++)
                for (var iRowP = 0; iRowP < cRowP; iRowP++) {
                    if (pattern[iRowP][iColP] == '#' && image[iRow + iRowP, iCol + iColP] != '#') {
                        return false;
                    }
                }
                return true;
            }
        }
        return res;
    }

    private class Tile(long id, string[] image)
    {
        public readonly long Id = id;
        public readonly int Size = image.Length;

        /*
         * This is a bit tricky, but makes operations fast and easy to implement
         *
         * - orientation % 4 specifies the rotation of the tile
         * - orientation % 8 >= 4 means the tile is flipped.
         *
         * The actual rotation and flipping happens in the indexer,
         * where the input coordinates are adjusted accordingly.
         *
         * Checking each 8 possible orientations for a tile requires just 7 incrementations of this value.
         */
        private int _orientation;

        public void ChangeOrientation()
        {
            _orientation++;
        }

        public char this[int iRow, int iCol]
        {
            get
            {
                for (var i = 0; i < _orientation % 4; i++)
                {
                    (iRow, iCol) = (iCol, Size - 1 - iRow); // rotate
                }

                if (_orientation % 8 >= 4)
                {
                    iCol = Size - 1 - iCol; // flip vertical axis
                }

                return image[iRow][iCol];
            }
        }
        
        public string Row(int iRow) => GetSlice(iRow, 0, 0, 1);
        public string Col(int iCol) => GetSlice(0, iCol, 1, 0);
        public string Top() => Row(0);
        public string Bottom() => Row(Size - 1);
        public string Left() => Col(0);
        public string Right() => Col(Size - 1);
        
        public override string ToString() {
            return $"Tile {Id}:\n" + string.Join("\n", Enumerable.Range(0, Size).Select(i => Row(i)));
        }
        
        private string GetSlice(int iRow, int iCol, int dRow, int dCol) {
            var st = "";
            for (var i = 0; i < Size; i++) {
                st += this[iRow, iCol];
                iRow += dRow;
                iCol += dCol;
            }
            
            return st;
        }
    }
}