namespace AOC._2021;

public class Day04
{
    private readonly string _input = File.ReadAllText("Inputs/Day04.txt")
        .Replace("\r", "");

    [Fact]
    public void Part1()
    {
        BoardsInOrderOfCompletion(_input).First().Score
            .Should().Be(41503);
    }

    [Fact]
    public void Part2()
    {
        BoardsInOrderOfCompletion(_input).Last().Score
            .Should().Be(3178);
    }

    private static IEnumerable<BingoBoard> BoardsInOrderOfCompletion(string input)
    {
        var blocks = input.Split("\n\n");
        
        // first block contains the numbers to be drawn
        var numbers = blocks[0].Split(",");
        var boards = (from block in blocks.Skip(1) select new BingoBoard(block)).ToHashSet();

        foreach (var number in numbers)
        {
            foreach (var board in boards.ToArray())
            {
                board.AddNumber(number);
                if (board.Score > 0)
                {
                    yield return board;
                    boards.Remove(board);
                }
            }
        }
    }

    private record Cell(string number, bool marked = false);

    private class BingoBoard
    {
        public int Score { get; private set; }
        private readonly List<Cell> _cells;

        private IEnumerable<Cell> CellsInRow(int iRow) =>
            from iCol in Enumerable.Range(0, 5)
            select _cells[iRow * 5 + iCol];

        private IEnumerable<Cell> CellsInCol(int iCol) =>
            from iRow in Enumerable.Range(0, 5)
            select _cells[iRow * 5 + iCol];

        public BingoBoard(string input)
        {
            _cells = (
                from word in input.Split(" \n".ToArray(), StringSplitOptions.RemoveEmptyEntries)
                select new Cell(word)
            ).ToList();
        }

        public void AddNumber(string number)
        {
            var iCell = _cells.FindIndex(cell => cell.number == number);
            if (iCell >= 0)
            {
                // mark the cell
                _cells[iCell] = _cells[iCell] with { marked = true };
                
                // if the board is completed, compute score
                for (var i = 0; i < 5; i++)
                {
                    if (
                        CellsInRow(i).All(cell => cell.marked) ||
                        CellsInCol(i).All(cell => cell.marked)
                    ) {

                        var unmarkedNumbers =
                            from cell in _cells where !cell.marked select int.Parse(cell.number);

                        Score = int.Parse(number) * unmarkedNumbers.Sum();
                    }
                }
            }
        }
    }
}