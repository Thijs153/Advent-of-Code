namespace AOC._2022;

public class Day04
{
    private readonly List<(Section First, Section Second)> _inputs = File.ReadAllLines("Inputs/Day04.txt")
        .Select(pair => pair.Split(","))
        .Select(x => (Section.Parse(x[0]), Section.Parse(x[1])))
        .ToList();

    [Fact]
    public void Part1()
    {
        _inputs
            .Count(a => a.First.FullyContains(a.Second) || a.Second.FullyContains(a.First))
            .Should().Be(644);
    }
    
    [Fact]
    public void Part2()
    {
        _inputs
            .Count(a => !a.First.DoesNotOverlap(a.Second))
            .Should().Be(926);
    }
    
    private record Section(int Start, int End)
    {
        public bool DoesNotOverlap(Section a) => 
            End < a.Start || Start > a.End;
        public bool FullyContains(Section a) => 
            Start <= a.Start && End >= a.End;
        public static Section Parse(string a) => 
            new(int.Parse(a.Split("-")[0]), int.Parse(a.Split("-")[1]));
    }
}