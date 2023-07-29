using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day07
{
    private readonly string[] _input = File.ReadAllText("Inputs/Day07.txt").Split(",");

    [Test]
    public void Part1()
    {
        AlignSubmarines(Parse(_input).ToArray(), false)
            .Min()
            .Should().Be(348664);
    }

    [Test]
    public void Part2()
    {
        AlignSubmarines(Parse(_input).ToArray(), true)
            .Min()
            .Should().Be(100220525);
    }
    
    private static IEnumerable<int> AlignSubmarines(int[] numbers, bool expensiveFuelUsage)
    {
        for (var i = numbers.Min(); i <= numbers.Max(); i++)
        {
            var fuelUsage = 0;
            foreach (var number in numbers)
            {
                var steps = Math.Abs(number - i);
                
                // 'expensiveFuelUsage' : 1 + 2 + ... n : n
                fuelUsage += expensiveFuelUsage
                    ? steps * (steps + 1) / 2
                    : steps;
            }

            yield return fuelUsage;
        }
    }

    private static IEnumerable<int> Parse(string[] input)
    {
        return input.Select(int.Parse);
    }
}