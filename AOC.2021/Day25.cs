using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day25
{
    
    private readonly string[] _input = File.ReadAllLines("Inputs/Day25.txt");

    [Test]
    public void Part1()
    {
        Solve(_input).Should().Be(471);
    }

    private static int Solve(string[] map)
    {
        var (cCol, cRow) = (map[0].Length, map.Length);

        int Right(int iCol) => (iCol + 1) % cCol;
        int Left(int iCol) => (iCol - 1 + cCol) % cCol;
        int Up(int iRow) => (iRow - 1 + cRow) % cRow;
        int Down(int iRow) => (iRow + 1) % cRow;

        bool MovesRight(int iRow, int iCol) =>
            map[iRow][iCol] == '>' && map[iRow][Right(iCol)] == '.';
        bool MovesDown(int iRow, int iCol) =>
            map[iRow][iCol] == 'v' && map[Down(iRow)][iCol] == '.';

        for (var steps = 1;; steps++)
        {
            var anyMoves = false;

            var newMap = new List<string>();
            for (var iRow = 0; iRow < cRow; iRow++)
            {
                var st = "";
                for (var iCol = 0; iCol < cCol; iCol++)
                {
                    anyMoves |= MovesRight(iRow, iCol);
                    st +=
                        MovesRight(iRow, iCol) ? '.' :
                        MovesRight(iRow, Left(iCol)) ? '>' :
                        map[iRow][iCol];
                }

                newMap.Add(st);
            }

            map = newMap.ToArray();
            newMap.Clear();

            for (var iRow = 0; iRow < cRow; iRow++)
            {
                var st = "";
                for (var iCol = 0; iCol < cCol; iCol++) {
                    anyMoves |= MovesDown(iRow, iCol);
                    st +=
                        MovesDown(iRow, iCol) ? '.' :
                        MovesDown(Up(iRow), iCol) ? 'v' :
                        map[iRow][iCol];
                }
                newMap.Add(st);
            }

            map = newMap.ToArray();

            if (!anyMoves)
            {
                return steps;
            }
        }
    }
}