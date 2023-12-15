
using FluentAssertions;
using NUnit.Framework;
using Map = char[][];

namespace AOC._2023;

public class Day14
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day14.txt");

    [Test]
    public void Part1() => 
        Measure(Tilt(Parse(_input))).Should().Be(106990);

    [Test]
    public void Part2() => 
        Measure(Iterate(Parse(_input), Cycle, 1_000_000_000)).Should().Be(100531);
    
    private static Map Parse(string[] input) =>
        (from line in input select line.ToCharArray()).ToArray();

    private static Map Iterate(Map map, Func<Map, Map> cycle, long count)
    {
        // The usual trick: keep iterating until we find a loop, make a shortcut
        // and finish with the remaining elements.
        var seen = new List<string>();
        while (count > 0)
        {
            map = cycle(map);
            count--;

            var hash = string.Join("", from line in map from ch in line select ch);
            if (!seen.Contains(hash))
            {
                seen.Add(hash);
            }
            else
            {
                count %= seen.Count - seen.IndexOf(hash);
                break;
            }
        }

        for (; count > 0; count--)
        {
            map = cycle(map);
        }

        return map;
    }

    private static Map Cycle(Map map)
    {
        for (var i = 0; i < 4; i++)
        {
            map = Rotate(Tilt(map));
        }

        return map;
    }

    // Tilt the map to the North, so that the '0' tiles roll to the top
    private static Map Tilt(Map map)
    {
        for (var iCol = 0; iCol < map[0].Length; iCol++)
        {
            var iRowT = 0; // tells where to roll up to the next '0' tile
            for (var iRow = 0; iRow < map.Length; iRow++)
            {
                if (map[iRow][iCol] == '#')
                {
                    iRowT = iRow + 1;
                } 
                else if (map[iRow][iCol] == 'O')
                {
                    (map[iRow][iCol], map[iRowT][iCol]) = ('.', 'O');
                    iRowT++;
                }
            }
        }

        return map;
    }
    
    // Ugly coordinate magic, turns the map 90º clockwise
    private static Map Rotate(Map src)
    {
        var dst = new char[src.Length][];
        for (var iRow = 0; iRow < src[0].Length; iRow++)
        {
            dst[iRow] = new char[src[0].Length];
            for (var iCol = 0; iCol < src.Length; iCol++)
            {
                dst[iRow][iCol] = src[src.Length - iCol - 1][iRow];
            }
        }

        return dst;
    }
    
    // returns the cumulated distances of the 'O' tiles from the bottom of the map
    private static int Measure(Map map) =>
        map.Select((row, iRow) => (map.Length - iRow) * row.Count(ch => ch == 'O')).Sum();
}