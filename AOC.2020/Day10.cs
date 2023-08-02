using System.Collections.Immutable;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2020;

[TestFixture]
public class Day10
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day10.txt");

    [Test]
    public void Part1()
    {
        var jolts = Parse(_input);
        var window = jolts.Skip(1).Zip(jolts).Select(p => (current: p.First, prev: p.Second)).ToArray();

        var oneJolts = window.Count(pair => pair.current - pair.prev == 1);
        var threeJolts = window.Count(pair => pair.current - pair.prev == 3);

        (oneJolts * threeJolts).Should().Be(2244);
    }

    [Test]
    public void Part2()
    {
        var jolts = Parse(_input);
        
        // dynamic programming with rolling variables a, b, c for the function values at i + 1, i + 2, and i + 3.
        var (a, b, c) = (1L, 0L, 0L);
        for (var i = jolts.Count - 2; i >= 0; i--)
        {
            var s =
                (i + 1 < jolts.Count && jolts[i + 1] - jolts[i] <= 3 ? a : 0) +
                (i + 2 < jolts.Count && jolts[i + 2] - jolts[i] <= 3 ? b : 0) +
                (i + 3 < jolts.Count && jolts[i + 3] - jolts[i] <= 3 ? c : 0);

            (a, b, c) = (s, a, b);
        }

        a.Should().Be(3947645370368);
    }

    private static ImmutableList<int> Parse(string[] input)
    {
        var num = input.Select(int.Parse).OrderBy(x => x).ToArray();
        return ImmutableList
            .Create(0)
            .AddRange(num)
            .Add(num.Last() + 3);
    }
}