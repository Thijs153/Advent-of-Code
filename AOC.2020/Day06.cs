using System.Collections.Immutable;

namespace AOC._2020;

public class Day06
{
    private readonly string _input = File.ReadAllText("Inputs/Day06.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        Solve(_input, (a, b) => a.Union(b)).Should().Be(6532);
    }

    [Fact]
    public void Part2()
    {
        Solve(_input, (a, b) => a.Intersect(b)).Should().Be(3427);
    }

    private static int Solve(string input,
        Func<ImmutableHashSet<char>, ImmutableHashSet<char>, ImmutableHashSet<char>> combine)
    {
        return (
            from grp in input.Split("\n\n")
            let answers = from line in grp.Split("\n") select line.ToImmutableHashSet()
            select answers.Aggregate(combine).Count
        ).Sum();
    }
}