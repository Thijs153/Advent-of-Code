using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day10
{
    private readonly List<string> _input = File.ReadAllLines("Inputs/Day10.txt").ToList();
    private readonly HashSet<int> _interestingCycles = new() { 20, 60, 100, 140, 180, 220 };
    private readonly HashSet<int> _rowCycles = new() { 40, 80, 120, 160, 200, 240 };

    [Test]
    public void Part1()
    {
        int currentCycle = 0;
        int currentValue = 1;
        List<int> interestingValues = new List<int>();
        

        foreach (string line in _input)
        {
            if (line == "noop")
            {
                currentCycle += 1;
                CheckInterestingCycle(currentCycle, interestingValues, currentValue);
                
                continue;
            }

            currentCycle += 1; // first cycle
            CheckInterestingCycle(currentCycle, interestingValues, currentValue);
            currentCycle += 1; // second cycle
            CheckInterestingCycle(currentCycle, interestingValues, currentValue);
            
            currentValue += int.Parse(line.Split(" ")[1]);
        }

        interestingValues.Sum().Should().Be(12740);
    }

    [Test]
    public void Part2()
    {
        List<string> rows = new();
        int currentCycle = 0;
        int currentValue = 1;
        int index = 0;

        foreach (string line in _input)
        {
            (int left, int middle, int right) sprite = (currentValue - 1, currentValue, currentValue + 1);
            if (line == "noop")
            {
                currentCycle += 1;

                continue;
            }

            currentCycle += 1; // first cycle

            currentCycle += 1; // second cycle

            
            currentValue += int.Parse(line.Split(" ")[1]);
        }
    }

    private void CheckInterestingCycle(int currentCycle, List<int> interestingValues, int currentValue)
    {
        if (_interestingCycles.Contains(currentCycle))
        {
            interestingValues.Add(currentValue * currentCycle);
        }
    }
    
}
