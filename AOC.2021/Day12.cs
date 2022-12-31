using System.Collections.Immutable;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day12
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day12.txt");

    [Test]
    public void Part1()
    {
        Explore(_input, false)
            .Should().Be(3450);
    }

    [Test]
    public void Part2()
    {
        Explore(_input, true)
            .Should().Be(96528);
    }
    
    private static int Explore(string[] input, bool part2)
    {
        var map = Parse(input);

        int pathCount(string currentCave, ImmutableHashSet<string> visitedCaves, bool anySmallCaveVisitedTwice)
        {
            if (currentCave == "end")
                return 1;

            var res = 0;
            foreach (var cave in map[currentCave])
            {
                var isBigCave = cave.ToUpper() == cave;
                var seen = visitedCaves.Contains(cave);

                if (!seen || isBigCave)
                {
                    // we can visit big caves any number of times, small caves only once
                    res += pathCount(cave, visitedCaves.Add(cave), anySmallCaveVisitedTwice);
                } else if (part2 && !isBigCave && cave != "start" && !anySmallCaveVisitedTwice)
                {
                    // part 2 also allows us to visit a single small cave twice
                    res += pathCount(cave, visitedCaves, true);
                }
            }

            return res;
        }

        return pathCount("start", ImmutableHashSet.Create<string>("start"), false);
    }
    
    private static Dictionary<string, string[]> Parse(string[] input)
    {
        // taking all connections 'there and back':
        var connections =
            from line in input
            let parts = line.Split("-")
            let caveA = parts[0]
            let caveB = parts[1]
            from connection in new[] { (From: caveA, To: caveB), (From: caveB, To: caveA) }
            select connection;
        
        // grouped by "from"
        return (
            from p in connections
            group p by p.From
            into g
            select g
        ).ToDictionary(g => g.Key, g => g.Select(c => c.To).ToArray());
    }
}