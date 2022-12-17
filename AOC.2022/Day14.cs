using System.Numerics;
using System.Runtime.InteropServices;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day14
{
    private List<string> _input = File.ReadAllLines("Inputs/Day14.txt").ToList();

    [Test]
    public void Part1()
    {
        new Cave(_input, hasFloor: false)
            .FillWithSand(new Complex(500, 0))
            .Should().Be(737);
    }
    
    [Test]
    public void Part2()
    {
        new Cave(_input, hasFloor: true)
            .FillWithSand(new Complex(500, 0))
            .Should().Be(28145);
    }

}

internal class Cave
{
    private readonly bool _hasFloor;
    private readonly Dictionary<Complex, char> _map;
    private readonly int _maxImaginary;

    public Cave(IList<string> input, bool hasFloor)
    {
        _hasFloor = hasFloor;
        _map = new Dictionary<Complex, char>();

        foreach (string line in input)
        {
            Complex[] steps = (
                from step in line.Split(" -> ")
                let parts = step.Split(",")
                select new Complex(int.Parse(parts[0]), int.Parse(parts[1]))
            ).ToArray();

            for (int i = 1; i < steps.Length; i++)
            {
                FillWithRocks(steps[i - 1], steps[i]);
            }
        }

        _maxImaginary = (int)_map.Keys.Select(pos => pos.Imaginary).Max();
    }

    private int FillWithRocks(Complex from, Complex to)
    {
        var dir = new Complex(
            Math.Sign(to.Real - from.Real),
            Math.Sign(to.Imaginary - from.Imaginary)
        );

        var steps = 0;
        for (var pos = from; pos != to + dir; pos += dir)
        {
            _map[pos] = '#';
            steps++;
        }

        return steps;
    }

    public int FillWithSand(Complex sandSource)
    {
        while (true)
        {
            var location = SimulateFallingSand(sandSource);

            if (_map.ContainsKey(location))
                break;

            if (!_hasFloor && location.Imaginary == _maxImaginary + 1)
                break;

            _map[location] = 'o';
        }

        return _map.Values.Count(x => x == 'o');
    }

    private Complex SimulateFallingSand(Complex sand)
    {
        Complex down = new(0, 1);
        Complex left = new(-1, 1);
        Complex right = new(1, 1);

        while (sand.Imaginary < _maxImaginary + 1) {
            if (!_map.ContainsKey(sand + down)) {
                sand += down;
            } else if (!_map.ContainsKey(sand + left)) {
                sand += left;
            } else if (!_map.ContainsKey(sand + right)) {
                sand += right;
            } else {
                break;
            }
        }
        
        return sand;
    }
}