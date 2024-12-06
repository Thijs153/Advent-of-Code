using System.Numerics;
using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;

namespace AOC._2024;

public class Day04
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day04.txt");

    private static readonly Complex Up = -Complex.ImaginaryOne;
    private static readonly Complex Down = Complex.ImaginaryOne;
    private static readonly Complex Left = -1;
    private static readonly Complex Right = 1;
    
    [Fact]
    public void Part1()
    {
        var map = Parse(_input);
        (
            from point in map.Keys
            from direction in new[] { Right, Right + Down, Down + Left, Down }
            where Matches(map, point, direction, "XMAS")
            select 1
        ).Count().Should().Be(2593);
    }

    [Fact]
    public void Part2()
    {
        var map = Parse(_input);
        (
            from point in map.Keys
            where
                Matches(map, point + Up + Left, Down + Right, "MAS") &&
                Matches(map, point + Down + Left, Up + Right, "MAS")
            select 1
        ).Count().Should().Be(1950);
    }

    // Check if the pattern (or its reverse) can be read from the given direction.
    private static bool Matches(Map map, Complex point, Complex dir, string pattern)
    {
        var chars = Enumerable.Range(0, pattern.Length)
            .Select(i => map.GetValueOrDefault(point + i * dir))
            .ToArray();
        return
            chars.SequenceEqual(pattern) ||
            chars.SequenceEqual(pattern.Reverse());
    }

    private static Map Parse(string[] input) => (
        from y in Enumerable.Range(0, input.Length)
        from x in Enumerable.Range(0, input[0].Length)
        select new KeyValuePair<Complex, char>(Complex.ImaginaryOne * y + x, input[y][x])
    ).ToDictionary();
}