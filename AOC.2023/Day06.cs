using System.Text.RegularExpressions;
using FluentAssertions;
using FluentAssertions.Extensions;
using NUnit.Framework;

namespace AOC._2023;

public class Day06
{
    private readonly string _input = File.ReadAllText("Inputs/Day06.txt").ReplaceLineEndings("\n");

    [Test]
    public void Part1() =>
        Solve(_input).Should().Be(1195150);

    [Test]
    public void Part2() =>
        Solve(_input.Replace(" ", "")).Should().Be(42550411);
    

    private static long Solve(string input)
    {
        var rows = input.Split("\n");
        var times = Parse(rows[0]);
        var records = Parse(rows[1]);

        var res = 1L;
        for (var i = 0; i < times.Length; i++)
        {
            res *= WinningMoves(times[i], records[i]);
        }

        return res;
    }
    
    private static long WinningMoves(long time, long record)
    {
        // If we wait x ms, our boat moves '(time - x) * x' millimeters.
        // This breaks the record when '(time - x) * x > record'
        // or '-x^2 + time * x - record > 0'.
        
        // get the roots first
        var (x1, x2) = SolveEq(-1, time, -record);
        
        // integers in between the roots
        var maxX = (long)Math.Ceiling(x2) - 1;
        var minX = (long)Math.Floor(x1) + 1;
        
        return maxX - minX + 1;
    }

    private static (double, double) SolveEq(long a, long b, long c)
    {
        var d = Math.Sqrt(b * b - 4 * a * c);
        var x1 = (-b - d) / (2 * a);
        var x2 = (-b + d) / (2 * a);
        return (Math.Min(x1, x2), Math.Max(x1, x2));
    }
    
    private static long[] Parse(string input) => (
        from m in Regex.Matches(input, @"\d+")
        select long.Parse(m.Value)
    ).ToArray();

}