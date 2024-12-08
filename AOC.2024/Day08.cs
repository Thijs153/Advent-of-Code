using System.Numerics;
using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;

namespace AOC._2024;

public class Day08
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day08.txt");

    [Fact]
    public void Part1()
    {
        GetUniquePositions(_input, GetAntinodes1).Count
            .Should().Be(367);
    }

    [Fact]
    public void Part2()
    {
        GetUniquePositions(_input, GetAntinodes2).Count
            .Should().Be(1285);
    }
    
    private static HashSet<Complex> GetUniquePositions(string[] input, GetAntiNodes getAntiNodes)
    {
        var map = GetMap(input);

        var antennaLocations = (
            from position in map.Keys
            where char.IsAsciiLetterOrDigit(map[position])
            select position
        ).ToArray();

        return (
            from srcAntenna in antennaLocations
            from dstAntenna in antennaLocations
            where srcAntenna != dstAntenna && map[srcAntenna] == map[dstAntenna]
            from antinode in getAntiNodes(srcAntenna, dstAntenna, map)
            select antinode
        ).ToHashSet();
    }
    
    private static IEnumerable<Complex> GetAntinodes1(Complex srcAntenna, Complex dstAntenna, Map map)
    {
        var displacement = dstAntenna - srcAntenna;
        var antinode = dstAntenna + displacement;
        if (map.Keys.Contains(antinode))
        {
            yield return antinode;
        }
    }

    private static IEnumerable<Complex> GetAntinodes2(Complex srcAntenna, Complex dstAntenna, Map map)
    {
        var displacement = dstAntenna - srcAntenna;
        var antinode = dstAntenna;
        while (map.Keys.Contains(antinode))
        {
            yield return antinode;
            antinode += displacement;
        }
    }
    
    private static Map GetMap(string[] input) => (
        from y in Enumerable.Range(0, input.Length)
        from x in Enumerable.Range(0, input[0].Length)
        select new KeyValuePair<Complex, char>(-Complex.ImaginaryOne * y + x, input[y][x])
    ).ToDictionary();
    
    private delegate IEnumerable<Complex> GetAntiNodes(Complex srcAntenna, Complex dstAntenna, Map map);
}