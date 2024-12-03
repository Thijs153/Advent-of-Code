namespace AOC._2024;

public class Day02
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day02.txt");

    [Fact]
    public void Part1()
    {
        Parse(_input).Count(IsValid)
            .Should().Be(356);
    }

    [Fact]
    public void Part2()
    {
        Parse(_input).Count(reports => RemoveOne(reports).Any(IsValid))
            .Should().Be(413);
    }
    
    private static bool IsValid(int[] reports)
    {
        var pairs = reports.Zip(reports.Skip(1)).ToList();
        return
            pairs.All(p => 1 <= p.Second - p.First && p.Second - p.First <= 3) ||
            pairs.All(p => 1 <= p.First - p.Second && p.First - p.Second <= 3);
    }

    private static IEnumerable<int[]> RemoveOne(int[] report) =>
        from i in Enumerable.Range(0, report.Length + 1)
        let before = report.Take(i - 1)
        let after = report.Skip(i)
        select before.Concat(after).ToArray();
    
    private static IEnumerable<int[]> Parse(string[] input) =>
        from line in input
        let levels = line.Split(" ").Select(int.Parse)
        select levels.ToArray();
}