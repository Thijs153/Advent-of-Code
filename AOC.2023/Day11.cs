using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

public class Day11
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day11.txt");

    [Test]
    public void Part1() =>
        Solve(_input, 1).Should().Be(9418609);
    
    [Test]
    public void Part2() =>
        Solve(_input, 999999).Should().Be(593821230983);

    private static long Solve(string[] map, int expansion)
    {
        Func<int, bool> isRowEmpty = EmptyRows(map).ToHashSet().Contains;
        Func<int, bool> isColEmpty = EmptyCols(map).ToHashSet().Contains;

        var galaxies = FindAll(map, '#');
        return (
            from g1 in galaxies
            from g2 in galaxies
            select
                Distance(g1.iRow, g2.iRow, expansion, isRowEmpty) +
                Distance(g1.iCol, g2.iCol, expansion, isColEmpty)
        ).Sum() / 2;
    }
    
    private static long Distance(int i1, int i2, int expansion, Func<int, bool> isEmpty)
    {
        var a = Math.Min(i1, i2);
        var d = Math.Abs(i1 - i2);
        return d + expansion * Enumerable.Range(a, d).Count(isEmpty);
    }
    
    private static IEnumerable<int> EmptyRows(string[] map) =>
        from iRow in Enumerable.Range(0, map.Length)
        where map[iRow].All(ch => ch == '.')
        select iRow;
    
    private static IEnumerable<int> EmptyCols(string[] map) =>
        from iCol in Enumerable.Range(0, map[0].Length)
        where map.All(row => row[iCol] == '.')
        select iCol;
    
    private static IEnumerable<Position> FindAll(string[] map, char ch) =>
        from iRow in Enumerable.Range(0, map.Length)
        from iCol in Enumerable.Range(0, map[0].Length)
        where map[iRow][iCol] == ch
        select new Position(iRow, iCol);
    
    private record Position(int iRow, int iCol);
}