using FluentAssertions;
using NUnit.Framework;

namespace AOC._2020;

[TestFixture]
public class Day11
{
    private readonly string _input = File.ReadAllText("Inputs/Day11.txt").ReplaceLineEndings("\n");

    [Test]
    public void Part1()
    {
        Solve(_input, 4, _ => true)
            .Should().Be(2359);
    }

    [Test]
    public void Part2()
    {
        Solve(_input, 5, place => place != '.')
            .Should().Be(2131);
    }

    private static int Solve(string input, int occupiedLimit, Func<char, bool> placeToCheck)
    {
        var (cRow, cCol) = (input.Split("\n").Length, input.IndexOf('\n'));

        char PlaceInDirection(char[] st, int idx, int dRow, int dCol)
        {
            var (iRow, iCol) = (idx / cCol, idx % cCol);
            while (true)
            {
                (iRow, iCol) = (iRow + dRow, iCol + dCol);
                var place =
                    iRow < 0 || iRow >= cRow ? 'L' :
                    iCol < 0 || iCol >= cCol ? 'L' :
                    st[iRow * cCol + iCol];

                if (placeToCheck(place))
                {
                    return place;
                }
            }
        }

        int OccupiedPlacesAround(char[] st, int idx)
        {
            var directions = new[] { (0, -1), (0, 1), (-1, 0), (1, 0), (-1, -1), (-1, 1), (1, -1), (1, 1) };
            var occupied = 0;
            foreach (var (dRow, dCol) in directions)
            {
                if (PlaceInDirection(st, idx, dRow, dCol) == '#')
                {
                    occupied++;
                }
            }

            return occupied;
        }

        var prevState = Array.Empty<char>();
        var state = input.Replace("\n", "").Replace("L", "#").ToArray();
        while (!prevState.SequenceEqual(state))
        {
            prevState = state;
            state = state.Select((place, i) =>
                    place == '#' && OccupiedPlacesAround(state, i) >= occupiedLimit ? 'L' :
                    place == 'L' && OccupiedPlacesAround(state, i) == 0             ? '#' :
                    place /*otherwise*/
            ).ToArray();
        }

        return state.Count(place => place == '#');
    }
}