using System.Text.RegularExpressions;

namespace AOC._2023;

public class Day02
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day02.txt");

    [Fact]
    public void Part1() => (
        from line in _input
        let game = ParseGame(line)
        where game.Red <= 12 && game.Green <= 13 && game.Blue <= 14
        select game.Id
    ).Sum().Should().Be(2476);

    [Fact]
    public void Part2() => (
        from line in _input
        let game = ParseGame(line)
        select game.Red * game.Green * game.Blue
    ).Sum().Should().Be(54911);
    
    private static Game ParseGame(string line) =>
        new(
            ParseInts(line, @"Game (\d+)").First(),
            ParseInts(line, @"(\d+) red").Max(),
            ParseInts(line, @"(\d+) green").Max(),
            ParseInts(line, @"(\d+) blue").Max()
        );

    private static IEnumerable<int> ParseInts(string st, string rx) =>
        from Match m in Regex.Matches(st, rx)
        select int.Parse(m.Groups[1].Value); // regex '()' defines a group, the number itself is group 1.
    
    private record Game(int Id, int Red, int Green, int Blue);
}