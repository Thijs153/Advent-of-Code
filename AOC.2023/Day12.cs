using System.Collections.Immutable;

using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;

namespace AOC._2023;

public class Day12
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day12.txt");

    [Fact]
    public void Part1() => Solve(_input, 1).Should().Be(7674);

    [Fact]
    public void Part2() => Solve(_input, 5).Should().Be(4443895258186);
    
    private static long Solve(string[] input, int repeat) => (
        from line in input
        let parts = line.Split(" ")
        let pattern = Unfold(parts[0], '?', repeat)
        let numString = Unfold(parts[1], ',', repeat)
        let nums = numString.Split(',').Select(int.Parse)
        select Compute(pattern, ImmutableStack.CreateRange(nums.Reverse()), new Cache())
    ).Sum();

    private static string Unfold(string st, char join, int unfold) =>
        string.Join(join, Enumerable.Repeat(st, unfold));

    private static long Compute(string pattern, ImmutableStack<int> nums, Cache cache)
    {
        if (!cache.ContainsKey((pattern, nums)))
        {
            cache[(pattern, nums)] = Dispatch(pattern, nums, cache);
        }

        return cache[(pattern, nums)];
    }
    
    private static long Dispatch(string pattern, ImmutableStack<int> nums, Cache cache) =>
        pattern.FirstOrDefault() switch
        {
            '.' => ProcessDot(pattern, nums, cache),
            '?' => ProcessQuestion(pattern, nums, cache),
            '#' => ProcessHash(pattern, nums, cache),
            _ => ProcessEnd(pattern, nums, cache)
        };

    private static long ProcessEnd(string _, ImmutableStack<int> nums, Cache __)
    {
        // the good case is when there are no numbers left at the end of the pattern
        return !nums.IsEmpty ? 0 : 1;
    }

    private static long ProcessDot(string pattern, ImmutableStack<int> nums, Cache cache)
    {
        // consume one spring and recurse
        return Compute(pattern[1..], nums, cache);
    }

    private static long ProcessQuestion(string pattern, ImmutableStack<int> nums, Cache cache)
    {
        // recurse both ways
        return Compute("." + pattern[1..], nums, cache) + Compute("#" + pattern[1..], nums, cache);
    }

    private static long ProcessHash(string pattern, ImmutableStack<int> nums, Cache cache)
    {
        // take the first number and consume that many dead springs, recurse
        if (nums.IsEmpty)
        {
            return 0; // no more numbers left, this is no good :(
        }

        var n = nums.Peek();
        nums = nums.Pop();

        var potentiallyDead = pattern.TakeWhile(s => s is '#' or '?').Count();

        if (potentiallyDead < n)
        {
            return 0; // not enough dead springs
        }
        
        if (pattern.Length == n)
        {
            return Compute("", nums, cache);
        }
        
        if (pattern[n] == '#')
        {
            return 0; // dead spring follows the range -> not good
        }

        return Compute(pattern[(n + 1)..], nums, cache);
    }
}