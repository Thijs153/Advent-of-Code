namespace AOC._2020;

public class Day05
{
    private readonly string _input = File.ReadAllText("Inputs/Day05.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        Seats(_input).Max().Should().Be(922);
    }

    [Fact]
    public void Part2()
    {
        var seats = Seats(_input);
        var (min, max) = (seats.Min(), seats.Max());
        
        Enumerable.Range(min, max - min + 1).Single(id => !seats.Contains(id))
            .Should().Be(747);
    }
    
    private static HashSet<int> Seats(string input)
    {
        return input
            .Replace("B", "1")
            .Replace("F", "0")
            .Replace("R", "1")
            .Replace("L", "0")
            .Split("\n")
            .Select(row => Convert.ToInt32(row, 2))
            .ToHashSet();
    }
}