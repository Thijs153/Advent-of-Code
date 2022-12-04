using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

public class Day3
{
    private List<string> _inputs = new();
    
    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _inputs = File.ReadAllLines("Inputs/Day3.txt").ToList();
    }

    [Test]
    public void Part1()
    {
        _inputs.Sum(line => 
            GetPriority( line[..(line.Length / 2)].Intersect(line[(line.Length / 2)..]).First()))
            .Should().Be(7795);
    }
    
    [Test]
    public void Part2()
    {
        _inputs.Chunk(3)
            .Sum(chunk => 
                GetPriority(chunk[0].Intersect(chunk[1]).Intersect(chunk[2]).First()))
            .Should().Be(2703);
    }

    private static int GetPriority(char character) => char.IsLower(character) ? character - 'a' + 1 : character - 'A' + 27;
}