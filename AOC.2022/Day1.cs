using NUnit.Framework;
using FluentAssertions;

namespace AOC._2022;

[TestFixture]
public class Day1
{
    private readonly IList<int> _totalCaloriesPerElf = new List<int>();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        var totalCalories = 0;
        foreach (var line in File.ReadAllLines("Inputs/Day1.txt"))
        {
            if (line == "")
            {
                _totalCaloriesPerElf.Add(totalCalories);
                totalCalories = 0;
                continue;
            }

            totalCalories += int.Parse(line);
        }

        _totalCaloriesPerElf.Add(totalCalories);
    }

    [Test]
    public void Part1() => _totalCaloriesPerElf.Max().Should().Be(69289);
    
    [Test]
    public void Part2() => _totalCaloriesPerElf.OrderByDescending(x => x).Take(3).Sum().Should().Be(205615);
}