namespace AOC._2024;

public class Day05
{
    private readonly string _input = File.ReadAllText("Inputs/Day05.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        var (updates, comparer) = Parse(_input);
        
        updates
            .Where(update => Sorted(update, comparer))
            .Sum(GetMiddlePage)
            .Should().Be(5639);
    }

    [Fact]
    public void Part2()
    {
        var (updates, comparer) = Parse(_input);

        updates
            .Where(update => !Sorted(update, comparer))
            .Select(update => update.OrderBy(p => p, comparer).ToArray())
            .Sum(GetMiddlePage)
            .Should().Be(5273);
    }
    
    private static bool Sorted(string[] update, Comparer<string> comparer) =>
        update.SequenceEqual(update.OrderBy(x => x, comparer));
    
    private static int GetMiddlePage(string[] update) => 
        int.Parse(update[update.Length / 2]);
    
    private static (string[][] updates, Comparer<string>) Parse(string input)
    {
        var parts = input.Split("\n\n");

        var ordering = new HashSet<string>(parts[0].Split("\n"));
        var comparer = Comparer<string>.Create((p1, p2) => ordering.Contains(p1 + '|' + p2) ? -1 : 1);

        var updates = parts[1].Split("\n").Select(line => line.Split(',')).ToArray();
        return (updates, comparer);
    }
}