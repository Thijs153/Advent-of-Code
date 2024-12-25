namespace AOC._2024;

public class Day25
{
    private readonly string _input = File.ReadAllText("Inputs/Day25.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        var patterns = _input.Split("\n\n").Select(b => b.Split("\n")).ToList();
        var keys = patterns.Where(p => p[0][0] == '.').Select(ParsePattern).ToList();
        var locks = patterns.Where(p => p[0][0] == '#').Select(ParsePattern).ToList();

        keys.Sum(k => locks.Count(l => Match(l, k)))
            .Should().Be(3107);
    }

    private static bool Match(int[] k, int[] l) =>
        Enumerable.Range(0, k.Length).All(i => k[i] + l[i] <= 7);
    
    private static int[] ParsePattern(string[] lines) => 
        Enumerable.Range(0, lines[0].Length).Select(x =>
            Enumerable.Range(0, lines.Length).Count(y => lines[y][x] == '#')
        ).ToArray();
}