using System.Collections.Concurrent;

namespace AOC._2024;

using Cache = ConcurrentDictionary<string, long>;

public class Day19
{
    private readonly string _input = File.ReadAllText("Inputs/Day19.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        MatchCounts(_input)
            .Count(c => c != 0)
            .Should().Be(304);
    }

    [Fact]
    public void Part2()
    {
        MatchCounts(_input)
            .Sum()
            .Should().Be(705756472327497L);
    }

    private static IEnumerable<long> MatchCounts(string input)
    {
        var blocks = input.Split("\n\n");
        var towels = blocks[0].Split(", ");
        return
            from pattern in blocks[1].Split("\n")
            select MatchCount(towels, pattern, new Cache());
    }
    
    /*
     * Computes the number of ways the pattern can be build up from the towels.
     * Works recursively by matching the prefix of the pattern with each towel.
     * A full match is found when the pattern becomes empty. the cache is applied to
     * speed up execution.
     */
    private static long MatchCount(string[] towels, string pattern, Cache cache) =>
        cache.GetOrAdd(pattern, p =>
            p switch
            {
                "" => 1,
                _ => towels
                    .Where(p.StartsWith)
                    .Sum(towel => MatchCount(towels, p[towel.Length ..], cache))
            }
        );
}