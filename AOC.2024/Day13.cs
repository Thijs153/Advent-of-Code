using System.Text.RegularExpressions;

namespace AOC._2024;

using Machine = (Day13.Vec2 A, Day13.Vec2 B, Day13.Vec2 Prize);

public class Day13
{
    private readonly string _input = File.ReadAllText("Inputs/Day13.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1() => Parse(_input).Sum(GetPrice).Should().Be(36870L);

    [Fact]
    public void Part2() => Parse(_input, shift: 10000000000000).Sum(GetPrice).Should().Be(78101482023732L);
    
    private static long GetPrice(Machine m)
    {
        var (a, b, prize) = m;
        
        // solve prize = ai + bj for i and j using Cramer's rule.
        var i = Det(prize, b) / Det(a, b);
        var j = Det(a, prize) / Det(a, b);
        
        // return the prize when a non-negative solution is found.
        if (i >= 0 && j >= 0 && a.X * i + b.X * j == prize.X && a.Y * i + b.Y * j == prize.Y)
        {
            return 3 * i + j;
        }

        return 0;
    }
    
    private static long Det(Vec2 a, Vec2 b) => a.X * b.Y - a.Y * b.X;

    private static IEnumerable<Machine> Parse(string input, long shift = 0)
    {
        var blocks = input.Split("\n\n");
        foreach (var block in blocks)
        {
            var nums =
                Regex.Matches(block, @"\d+", RegexOptions.Multiline)
                    .Select(m => int.Parse(m.Value))
                    .Chunk(2).Select(p => new Vec2(p[0], p[1]))
                    .ToArray();

            nums[2] = new Vec2(nums[2].X + shift, nums[2].Y + shift);
            yield return (nums[0], nums[1], nums[2]);
        }
    }

    public record struct Vec2(long X, long Y);
}