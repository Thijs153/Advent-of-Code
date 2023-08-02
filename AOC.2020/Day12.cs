using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2020;

[TestFixture]
public class Day12
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day12.txt");

    [TestCase(true, 1106, TestName = "Part1")]
    [TestCase(false, 107281, TestName = "Part2")]
    public void Run(bool partOne, double result)
    {
        MoveShip(_input, partOne).Should().Be(result);
    }
    
    private static double MoveShip(string[] input, bool part1)
    {
        return input.Select(line => (line[0], int.Parse(line[1..])))
            .Aggregate(
                new State(pos: Complex.Zero, dir: part1 ? Complex.One : new Complex(10, 1)),
                (state, line) =>
                    line switch
                    {
                        ('N', var arg) when part1 => state with { pos = state.pos + arg * Complex.ImaginaryOne },
                        ('N', var arg)            => state with { dir = state.dir + arg * Complex.ImaginaryOne },
                        ('S', var arg) when part1 => state with { pos = state.pos - arg * Complex.ImaginaryOne },
                        ('S', var arg)            => state with { dir = state.dir - arg * Complex.ImaginaryOne },
                        ('E', var arg) when part1 => state with { pos = state.pos + arg },
                        ('E', var arg)            => state with { dir = state.dir + arg },
                        ('W', var arg) when part1 => state with { pos = state.pos - arg },
                        ('W', var arg)            => state with { dir = state.dir - arg },
                        ('F', var arg)            => state with { pos = state.pos + arg * state.dir },
                        ('L', 90) or ('R', 270)   => state with { dir = state.dir * Complex.ImaginaryOne },
                        ('L', 270) or ('R', 90)   => state with { dir = -state.dir * Complex.ImaginaryOne },
                        ('L', 180) or ('R', 180)  => state with { dir = -state.dir },
                        _ => throw new Exception()
                    },
                state => Math.Abs(state.pos.Imaginary) + Math.Abs(state.pos.Real));
    }

    private record State(Complex pos, Complex dir);
}

