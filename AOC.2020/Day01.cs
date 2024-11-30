namespace AOC._2020;

public class Day01
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day01.txt");

    [Fact]
    public void Part1()
    {
        var numbers = Numbers(_input);
        (
            from x in numbers
            let y = 2020 - x
            where numbers.Contains(y)
            select x * y
        ).First().Should().Be(485739);
    }

    [Fact]
    public void Part2()
    {
        var numbers = Numbers(_input);
        (
            from x in numbers
            from y in numbers
            let z = 2020 - x - y
            where numbers.Contains(z)
            select x * y * z
        ).First().Should().Be(161109702);
    }

    private static HashSet<int> Numbers(string[] input) => input.Select(int.Parse).ToHashSet();
}