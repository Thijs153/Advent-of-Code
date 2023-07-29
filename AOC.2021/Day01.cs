using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day01
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day01.txt");

    [Test]
    public void Part1()
    {
        DepthIncrease(GetNumbers(_input).ToList())
            .Should().Be(1752);
    }

    [Test]
    public void Part2()
    {
        DepthIncrease(ThreeMeasurements(GetNumbers(_input).ToList()).ToList())
            .Should().Be(1781);
    }
    
    
    private static int DepthIncrease(IList<int> numbers) => (
        from p in numbers.Zip(numbers.Skip(1))
        where p.First < p.Second
        select 1
    ).Count();

    private static IEnumerable<int> ThreeMeasurements(IList<int> numbers) =>
        from t in numbers.Zip(numbers.Skip(1), numbers.Skip(2))
        select t.First + t.Second + t.Third;

    private static IEnumerable<int> GetNumbers(IEnumerable<string> input) =>
        from n in input
        select int.Parse(n);
}