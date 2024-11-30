using System.Text.RegularExpressions;

namespace AOC._2023;

public class Day04
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day04.txt");

    [Fact]
    public void Part1() => (
        from line in _input
        let card = ParseCard(line)
        where card.matches > 0
        select Math.Pow(2, card.matches - 1)
    ).Sum().Should().Be(25010);

    [Fact]
    public void Part2()
    {
        var cards = _input.Select(ParseCard).ToArray();
        var counts = cards.Select(_ => 1).ToArray();

        for (var i = 0; i < cards.Length; i++)
        {
            var (card, count) = (cards[i], counts[i]);
            for (var j = 0; j < card.matches; j++)
            {
                counts[i + j + 1] += count;
            }
        }

        counts.Sum().Should().Be(9924412);
    }
    
    private static Card ParseCard(string line)
    {
        var parts = line.Split(':', '|');
        var left = from m in Regex.Matches(parts[1], @"\d+") select m.Value;
        var right = from m in Regex.Matches(parts[2], @"\d+") select m.Value;
        return new Card(left.Intersect(right).Count());
    }

    private record Card(int matches);
}