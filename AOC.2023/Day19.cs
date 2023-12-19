using System.Collections.Immutable;
using System.Numerics;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

using Rules = Dictionary<string, string>;
using Cube = ImmutableArray<Range>;

internal record Range(int begin, int end);
internal record Cond(int dim, int num, string jmp);

public class Day19
{
    private readonly string _input = File.ReadAllText("Inputs/Day19.txt").ReplaceLineEndings("\n");

    [Test]
    public void Part1()
    {
        var parts = _input.Split("\n\n");
        var rules = ParseRules(parts[0]);

        (
            from cube in ParseUnitCube(parts[1]) 
            where AcceptedVolume(rules, cube, "in") == 1
            select cube.Select(dim => dim.begin).Sum()
        ).Sum().Should().Be(362930);
    }

    [Test]
    public void Part2()
    {
        var parts = _input.Split("\n\n");
        var rules = ParseRules(parts[0]);
        var cube = Enumerable.Repeat(new Range(1, 4000), 4).ToImmutableArray();

        AcceptedVolume(rules, cube, "in").Should().Be(116365820987729);
    }
    
    private static BigInteger AcceptedVolume(Rules rules, Cube cube, string state)
    {
        if (cube.Any(coord => coord.end < coord.begin))
            return 0;
        
        if (state == "A")
            return Volume(cube);
        
        if (state == "R")
            return 0;
        
        var res = BigInteger.Zero;
        foreach (var stm in rules[state].Split(","))
        {
            // slicing happens here in the presence of < and > symbols.
            if (TryParseCond(stm, '<', out var cond))
            {
                // here we have a condition, something like 'x < 1000 : foo'
                // so the cube is split along the right dimension (x) into two halves:
                var (lo, hi) = SplitRange(cube[cond.dim], cond.num);
                // recurse with the accepted half to 'foo'
                res += AcceptedVolume(rules, cube.SetItem(cond.dim, lo), cond.jmp);
                // and continue with the rejected half
                cube = cube.SetItem(cond.dim, hi);
            } 
            else if (TryParseCond(stm, '>', out cond))
            {
                // same as the other case with 'x > 1000'
                var (lo, hi) = SplitRange(cube[cond.dim], cond.num + 1);
                res += AcceptedVolume(rules, cube.SetItem(cond.dim, hi), cond.jmp);
                cube = cube.SetItem(cond.dim, lo);
            }
            else
            {
                res += AcceptedVolume(rules, cube, stm);
            }
        }

        return res;
    }

    private static BigInteger Volume(Cube cube) =>
        cube.Aggregate(BigInteger.One, (m, dim) => m * (dim.end - dim.begin + 1));
    
    private static bool TryParseCond(string st, char ch, out Cond cond)
    {
        if (!st.Contains(ch))
        {
            cond = null;
            return false;
        }

        // st has the form of "x<1000:foo"
        var parts = st.Split(ch, ':');
        cond = new Cond(
            dim: parts[0] switch { "x" => 0, "m" => 1, "a" => 2, _ => 3 },
            num: int.Parse(parts[1]),
            jmp: parts[2]
        );

        return true;
    }
    
    // returns two ranges containing [r.begin .. num - 1], [num .. r.end],
    // returns empty ranges where end < begin if num is outside of r
    private static (Range lo, Range hi) SplitRange(Range r, int num) => (
        r with { end = Math.Min(num - 1, r.end) },
        r with { begin = Math.Max(r.begin, num) }
    );

    private static Rules ParseRules(string input) => (
        from line in input.Split('\n')
        let parts = line.Split('{', '}')
        select new KeyValuePair<string, string>(parts[0], parts[1])
    ).ToDictionary();

    private static IEnumerable<Cube> ParseUnitCube(string input) =>
        from line in input.Split('\n')
        let nums = Regex.Matches(line, @"\d+").Select(m => int.Parse(m.Value)).ToArray()
        select nums.Select(n => new Range(n, n)).ToImmutableArray();
}