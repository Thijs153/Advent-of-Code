using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2020;

[TestFixture]
public class Day07
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day07.txt");

    [Test]
    public void Part1()
    {
        var parentsOf = new Dictionary<string, HashSet<string>>();

        foreach (var line in _input)
        {
            var description = ParseLine(line);

            foreach (var (_, bag) in description.children)
            {
                if (!parentsOf.ContainsKey(bag))
                {
                    parentsOf[bag] = new HashSet<string>();
                }

                parentsOf[bag].Add(description.bag);
            }
        }

        IEnumerable<string> PathsToRoot(string bag)
        {
            yield return bag;

            if (parentsOf.ContainsKey(bag))
            {
                foreach (var container in parentsOf[bag])
                {
                    foreach (var bagT in PathsToRoot(container))
                    {
                        yield return bagT;
                    }
                }
            }
        }

        (PathsToRoot("shiny gold bag").ToHashSet().Count - 1)
            .Should().Be(335);
    }

    [Test]
    public void Part2()
    {
        var childrenOf = new Dictionary<string, List<(int count, string bag)>>();
        foreach (var line in _input)
        {
            var description = ParseLine(line);
            childrenOf[description.bag] = description.children;
        }

        long CountWithChildren(string bag) =>
            1 + (from child in childrenOf[bag] select child.count * CountWithChildren(child.bag)).Sum();

        (CountWithChildren("shiny gold bag") - 1)
            .Should().Be(2431);
    }

    private static (string bag, List<(int count, string bag)> children) ParseLine(string line)
    {
        var bag = Regex.Match(line, "^[a-z]+ [a-z]+ bag").Value;

        var children = Regex
            .Matches(line, "(\\d+) ([a-z]+ [a-z]+ bag)")
            .Select(x => (count: int.Parse(x.Groups[1].Value), bag: x.Groups[2].Value))
            .ToList();

        return (bag, children);
    }
}