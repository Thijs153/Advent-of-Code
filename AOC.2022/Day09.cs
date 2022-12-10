using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day09
{
    private List<(string, int)> _input = default!;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _input = File.ReadAllLines("Inputs/Day09.txt").Select(x => (x.Split()[0], int.Parse(x.Split()[1]))).ToList();
    }

    [Test]
    public void Part1() => GoThroughMovements(2).Should().Be(6464);
    
    [Test]
    public void Part2() => GoThroughMovements(10).Should().Be(2604);
    
    private int GoThroughMovements(int ropeLenght)
    {
        HashSet<(int X, int Y)> visitedPositions = new();
        List<(int x, int y)> knots = Enumerable.Repeat((0, 0), ropeLenght).ToList();

        foreach (var (direction, steps) in _input)
        {
            for (int i = 0; i < steps; i++)
            {
                (int x, int y) head = knots[0];
                MoveHead(ref head, direction);
                knots[0] = head;

                for (int j = 1; j < knots.Count; j++)
                {
                    var knot = knots[j];
                    MoveKnot(ref knot, knots[j - 1]);
                    knots[j] = knot;
                }

                visitedPositions.Add((knots[^1].x, knots[^1].y));
            }
        }

        return visitedPositions.Count;
    }
    
    private static void MoveHead(ref (int x, int y) head, string direction)
    {
        if (direction == "L") head.x -= 1;
        if (direction == "R") head.x += 1;
        if (direction == "U") head.y += 1;
        if (direction == "D") head.y -= 1;
    }
    
    private static void MoveKnot(ref (int x, int y) knot, (int x, int y) previousKnot)
    {
        if (Math.Abs(previousKnot.x - knot.x) > 1)
        {
            if (previousKnot.x < knot.x)
            {
                if (previousKnot.y > knot.y)
                {
                    knot.x -= 1;
                    knot.y += 1;
                } else if (previousKnot.y < knot.y)
                {
                    knot.x -= 1;
                    knot.y -= 1;
                }
                else
                {
                    knot.x -= 1;
                }
            }
            else
            {
                if (previousKnot.y > knot.y)
                {
                    knot.x += 1;
                    knot.y += 1;
                } else if (previousKnot.y < knot.y)
                {
                    knot.x += 1;
                    knot.y -= 1;
                }
                else
                {
                    knot.x += 1;    
                }
            }
        }

        if (Math.Abs(previousKnot.y - knot.y) > 1)
        {
            if (previousKnot.y < knot.y)
            {
                if (previousKnot.x > knot.x)
                {
                    knot.x += 1;
                    knot.y -= 1;
                } else if (previousKnot.x < knot.x)
                {
                    knot.x -= 1;
                    knot.y -= 1;
                }
                else
                {
                    knot.y -= 1;
                }
            }
            else
            {
                if (previousKnot.x > knot.x)
                {
                    knot.x += 1;
                    knot.y += 1;
                } else if (previousKnot.x < knot.x)
                {
                    knot.x -= 1;
                    knot.y += 1;
                }
                else
                {
                    knot.y += 1;
                }
            }
        }
    }
}