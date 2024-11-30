using System.Text.RegularExpressions;

namespace AOC._2023;

public class Day03
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day03.txt");

    [Fact]
    public void Part1()
    {
        var symbols = Parse(_input, new Regex(@"[^.0-9]"));
        var nums = Parse(_input, new Regex(@"\d+"));

        (
            from n in nums 
            where symbols.Any(s => Adjacent(s, n)) 
            select n.Int
        ).Sum().Should().Be(525911);
    }

    [Fact]
    public void Part2()
    {
        var gears = Parse(_input, new Regex(@"\*"));
        var numbers = Parse(_input, new Regex(@"\d+"));

        (
            from g in gears
            let neighbours = from Part n in numbers where Adjacent(n, g) select n.Int
            where neighbours.Count() == 2
            select neighbours.First() * neighbours.Last()
        ).Sum().Should().Be(75805607);
    }
    
    
    private static bool Adjacent(Part p1, Part p2) =>
        Math.Abs(p2.Row - p1.Row) <= 1 &&
        p1.Col <= p2.Col + p2.Text.Length &&
        p2.Col <= p1.Col + p1.Text.Length;
    
    private static Part[] Parse(IReadOnlyList<string> rows, Regex rx) => (
        from iRow in Enumerable.Range(0, rows.Count)
        from match in rx.Matches(rows[iRow])
        select new Part(match.Value, iRow, match.Index)
    ).ToArray();
    
    private record Part(string Text, int Row, int Col)
    {
        public int Int => int.Parse(Text);
    }
}