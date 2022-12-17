using System.Collections;
using System.ComponentModel;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day15
{
    private string[] _input = File.ReadAllLines("Inputs/Day15.txt");

    [Test]
    public void Part1()
    {
        var pairing = Parse(_input).ToArray();
        
        var rects = pairing.Select(pair => pair.ToRect()).ToArray();
        var left = rects.Select(r => r.Left).Min();
        var right = rects.Select(r => r.Right).Max();

        var y = 2000000;
        var res = 0;
        for (var x = left; x <= right; x++) {
            var pos = new Pos(x, y);
            if (pairing.Any(pair => pair.Beacon != pos && pair.InRange(pos))) {
                res++;
            }
        }

        res.Should().Be(5511201);
    }

    [Test]
    public void Part2()
    {
        var pairing = Parse(_input).ToArray();
        var area = GetUncoveredAreas(pairing, new Rect(0, 0, 4000001, 4000001)).First();

        (area.X * 4000000L + area.Y).Should().Be(11318723411840);
    }
    
    
    
    private IEnumerable<Pair> Parse(string[] input)
    {
        foreach (string line in input)
        {
            var numbers = Regex.Matches(line, "-?[0-9]+").Select(m => int.Parse(m.Value)).ToArray();
            yield return new Pair(
                Sensor: new Pos(numbers[0], numbers[1]),
                Beacon: new Pos(numbers[2], numbers[3])
            );
        }
    }

    private IEnumerable<Rect> GetUncoveredAreas(Pair[] pairing, Rect rect)
    {
        if (rect.Width == 0 || rect.Height == 0)
            yield break;

        foreach (var pair in pairing)
        {
            if (rect.Corners.All(corner => pair.InRange(corner)))
                yield break;
        }

        if (rect.Width == 1 && rect.Height == 1)
        {
            yield return rect;
            yield break;
        }

        foreach (var rectT in rect.Split())
        {
            foreach (var area in GetUncoveredAreas(pairing, rectT))
            {
                yield return area;
            }
        }
    }

    private record struct Pos(int X, int Y);

    private record struct Pair(Pos Sensor, Pos Beacon)
    {
        public int Radius = Manhattan(Sensor, Beacon);

        public bool InRange(Pos pos) => Manhattan(pos, Sensor) <= Radius;

        public Rect ToRect() =>
            new Rect(Sensor.X - Radius, Sensor.Y - Radius, 2 * Radius + 1, 2 * Radius + 1);

        private static int Manhattan(Pos p1, Pos p2) =>
            Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
    }

    private record struct Rect(int X, int Y, int Width, int Height)
    {
        public int Left => X;
        public int Right => X + Width - 1;
        public int Top => Y;
        public int Bottom => Y + Height - 1;

        public IEnumerable<Pos> Corners
        {
            get
            {
                yield return new Pos(Left, Top);
                yield return new Pos(Right, Top);
                yield return new Pos(Right, Bottom);
                yield return new Pos(Left, Bottom);
            }
        }

        public IEnumerable<Rect> Split()
        {
            var w0 = Width / 2;
            var w1 = Width - w0;
            var h0 = Height / 2;
            var h1 = Height - h0;
            yield return new Rect(Left, Top, w0, h0);
            yield return new Rect(Left + w0, Top, w1, h0);
            yield return new Rect(Left, Top + h0, w0, h1);
            yield return new Rect(Left + w0, Top + h0, w1, h1);
        }
    }
    
    
}