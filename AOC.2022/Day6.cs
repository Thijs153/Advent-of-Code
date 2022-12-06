using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

public class Day6
{
    private readonly string _input = File.ReadAllText("Inputs/Day6.txt");
    
    [TestCase(4, 1142)]
    [TestCase(14, 2803)]
    public void Test(int n , int result)
    {
        int index = 0;
        for (int i = n; i < _input.Length; i++)
        {
            string sequence = _input[(i - n)..i];

            if (!sequence.Distinct().SequenceEqual(sequence)) continue;

            index = i;
            break;
        }

        index.Should().Be(result);
    } 
    
    
}