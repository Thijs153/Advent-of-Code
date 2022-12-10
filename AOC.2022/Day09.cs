using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day09
{
    private List<(string, int)> _input = default!;
    private HashSet<(int X, int Y)> _visitedPositions = default!;
    
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _input = File.ReadAllLines("Inputs/Day09.txt").Select(x => (x.Split()[0], int.Parse(x.Split()[1]))).ToList();
    }

    [SetUp]
    public void SetUp()
    {
        _visitedPositions = new HashSet<(int X, int Y)> {(0,0)};
    }

    [Test]
    public void Part1()
    {
        (int X, int Y)  head = (0, 0);
        (int X, int Y)  tail = (0, 0);
        

        foreach (var (direction, steps) in _input)
        {
            for (int i = 0; i < steps; i++)
            {
                (int x, int y) previousHeadPosition = head;
                MoveHead(ref head, direction);

                if (!IsKnotInRangeOfPreviousOne(tail, head))
                {
                    tail = previousHeadPosition;
                    _visitedPositions.Add((tail.X, tail.Y));
                }
            }
        }

        _visitedPositions.Count.Should().Be(6464);
    }

    [Test]
    public void Part2()
    {
        List<(int x, int y)> knots = new()
        {
            (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0), (0, 0)
        };

        foreach (var (direction, steps) in _input)
        {
            for (int i = 0; i < steps; i++)
            {
                List<(int x, int y)> oldPositions = new(knots);
                
                (int x, int y) head = knots[0];
                MoveHead(ref head, direction);
                knots[0] = head;

                for (int j = 1; j < knots.Count; j++)
                {
                    if (!IsKnotInRangeOfPreviousOne(knots[j], knots[j - 1]))
                    {
                        knots[j] = oldPositions[j - 1];
                    }
                }

                _visitedPositions.Add((knots[^1].x, knots[^1].y));
            }
        }

        _visitedPositions.Count.Should().Be(2604);
    }

    private void MoveHead(ref (int x, int y) head, string direction)
    {
        if (direction == "L") head.x -= 1;
        if (direction == "R") head.x += 1;
        if (direction == "U") head.y += 1;
        if (direction == "D") head.y -= 1;
    }

    private bool IsKnotInRangeOfPreviousOne((int x, int y) knot, (int x, int y) previousKnot)
    {
        return Math.Abs(previousKnot.x - knot.x) <= 1 && Math.Abs(previousKnot.y - knot.y) <= 1;
    }
    private void MoveKnot(ref (int x, int y) knot, (int x, int y) previousKnot)
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