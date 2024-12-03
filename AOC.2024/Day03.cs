using System.Text.RegularExpressions;

namespace AOC._2024;

public class Day03
{
    private readonly string _input = File.ReadAllText("Inputs/Day03.txt");

    [Fact]
    public void Part1() => Solve(_input, @"mul\((\d+),(\d+)\)").Should().Be(188116424L);

    [Fact]
    public void Part2() => Solve(_input, @"mul\((\d+),(\d+)\)|don't\(\)|do\(\)").Should().Be(104245808L);
    
    private static long Solve(string input, string regex)
    {
        var matches = Regex.Matches(input, regex, RegexOptions.Multiline);
        return matches.Aggregate(
            (enabled: true, result: 0L),
            (acc, match) => (match.Value, res: acc.result, acc.enabled) switch
            {
                ("don't()", _, _) => (false, acc.result),
                ("do()", _, _) => (true, acc.result),
                (_, var res, true) => (true, res + int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value)),
                _ => acc,
            },
            acc => acc.result
        );
    }
}