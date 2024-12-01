namespace AOC._2024;

public class Day01
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day01.txt");

    [Fact]
    public void Part1()
    {
        ParseColumn(_input, 0).Zip(ParseColumn(_input, 1))
            .Select(p => Math.Abs(p.First - p.Second))
            .Sum()
            .Should().Be(1970720);
    }

    [Fact]
    public void Part2()
    {
        var weights = ParseColumn(_input, 1).CountBy(x => x).ToDictionary();
        ParseColumn(_input, 0)
            .Select(num => weights.GetValueOrDefault(num) * num)
            .Sum()
            .Should().Be(17191599);
    }


    private static IEnumerable<int> ParseColumn(string[] input, int column) =>
        from line in input
        let nums = line.Split("   ").Select(int.Parse).ToArray()
        orderby nums[column]
        select nums[column];
}