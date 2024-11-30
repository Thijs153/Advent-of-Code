namespace AOC._2022;

public class Day02
{
    private readonly List<(char, char)> _inputs = [];
    private readonly Dictionary<(char, char), int> _scoresP1 = new()
    {
        {('A','X'), 4}, {('A','Y'), 8}, {('A', 'Z'), 3},
        {('B','X'), 1}, {('B','Y'), 5}, {('B', 'Z'), 9},
        {('C','X'), 7}, {('C','Y'), 2}, {('C', 'Z'), 6}
    };
    
    private readonly Dictionary<(char, char), int> _scoresP2 = new()
    {
        {('A','X'), 3}, {('A','Y'), 4}, {('A', 'Z'), 8},
        {('B','X'), 1}, {('B','Y'), 5}, {('B', 'Z'), 9},
        {('C','X'), 2}, {('C','Y'), 6}, {('C', 'Z'), 7}
    };

    public Day02()
    {
        foreach (var line in File.ReadAllLines("Inputs/Day02.txt"))
        {
            _inputs.Add((line[0], line[2]));
        } 
    }
    

    [Fact]
    public void Part1() => _inputs.Sum(kvp => _scoresP1[kvp]).Should().Be(10941);
    
    [Fact]
    public void Part2() => _inputs.Sum(kvp => _scoresP2[kvp]).Should().Be(13071);
}