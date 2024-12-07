using System.Text.RegularExpressions;

namespace AOC._2024;

public class Day07
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day07.txt");

    [Fact]
    public void Part1()
    {
        Filter(_input, Check1)
            .Sum()
            .Should().Be(1153997401072L);
    }

    [Fact]
    public void Part2()
    {
        Filter(_input, Check2)
            .Sum()
            .Should().Be(97902809384118L);
    }
    
    private static IEnumerable<long> Filter(string[] input, Func<long, long, List<long>, bool> check) =>
        from line in input
            let parts = Regex.Matches(line, @"\d+").Select(m => long.Parse(m.Value))
            let target = parts.First()
            let numbers = parts.Skip(1).ToList()
        where check(target, numbers[0], numbers[1..])
        select target;
    
    private static bool Check1(long target, long accumulator, List<long> numbers) =>
        numbers switch
        {
            [] => target == accumulator,
            _ => Check1(target, accumulator * numbers[0], numbers[1..]) ||
                 Check1(target, accumulator + numbers[0], numbers[1..])
        };

    private static bool Check2(long target, long accumulator, List<long> numbers) =>
        numbers switch
        {
            _ when accumulator > target => false,
            [] => target == accumulator,
            _ => Check2(target, long.Parse($"{accumulator}{numbers[0]}"), numbers[1..]) ||
                 Check2(target, accumulator * numbers[0], numbers[1..]) ||
                 Check2(target, accumulator + numbers[0], numbers[1..])
        };
}