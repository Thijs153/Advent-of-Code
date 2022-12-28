using System.Drawing;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day05
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day05.txt");

    [Test]
    public void Part1()
    {
        GetIntersections(Parse(_input, true)).Count()
            .Should().Be(6267);
    }

    [Test]
    public void Part2()
    {
        GetIntersections(Parse(_input, false)).Count()
            .Should().Be(20196);
    }
    
    private static IEnumerable<Point> GetIntersections(IEnumerable<IEnumerable<Point>> lines) =>
        lines.SelectMany(p => p)
            .GroupBy(p => p)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

    private static IEnumerable<IEnumerable<Point>> Parse(string[] input, bool skipDiagonals) =>
        from line in input

        let ns = (
            from st in line.Split(", ->".ToArray(), StringSplitOptions.RemoveEmptyEntries)
            select int.Parse(st)
        ).ToArray()

        let start = new Point(ns[0], ns[1])
        let end = new Point(ns[2], ns[3])
        let displacement = new Point(end.X - start.X, end.Y - start.Y)
        let length = 1 + Math.Max(Math.Abs(displacement.X), Math.Abs(displacement.Y))
        let dir = new Point(Math.Sign(displacement.X), Math.Sign(displacement.Y))

        let points =
            from i in Enumerable.Range(0, length)
            select new Point(start.X + i * dir.X, start.Y + i * dir.Y)

        // skip diagonals in p1
        where !skipDiagonals || dir.X == 0 || dir.Y == 0

        select points;
    
}