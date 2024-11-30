using System.Text.RegularExpressions;

namespace AOC._2023;

public class Day01
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day01.txt");

    [Fact]
    public void Part1() =>
        Solve(_input, @"\d").Should().Be(55123);

    [Fact]
    public void Part2() =>
        Solve(_input, @"\d|one|two|three|four|five|six|seven|eight|nine").Should().Be(55260);

    private static int Solve(IEnumerable<string> input, string rx) => (
        from line in input
        let first = Regex.Match(line, rx)
        let last = Regex.Match(line, rx, RegexOptions.RightToLeft)
        select ParseMatch(first.Value) * 10 + ParseMatch(last.Value)
    ).Sum();

    private static int ParseMatch(string st) => st switch
    {
        "one" => 1,
        "two" => 2,
        "three" => 3,
        "four" => 4,
        "five" => 5,
        "six" => 6,
        "seven" => 7,
        "eight" => 8,
        "nine" => 9,
        _ => int.Parse(st)
    };
}