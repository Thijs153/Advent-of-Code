using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

using Map = Dictionary<Complex, char>;

public class Day10
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day10.txt");

    [Test]
    public void Part1()
    {
        var map = ParseMap(_input);
        var loop = LoopPositions(map);
        (loop.Count / 2).Should().Be(6613);
    }
    
    [Test]
    public void Part2()
    {
        var map = ParseMap(_input);
        var loop = LoopPositions(map);
        
        // remove pipes not in the loop:
        map = (
            from kvp in map
            let position = kvp.Key
            let cell = loop.Contains(position) ? kvp.Value : '.'
            select (position, cell)
        ).ToDictionary();

        map.Keys.Count(position => Inside(map, position))
            .Should().Be(511);
    }

    private static readonly Complex Up = -Complex.ImaginaryOne;
    private static readonly Complex Down = Complex.ImaginaryOne;
    private static readonly Complex Left = -Complex.One;
    private static readonly Complex Right = Complex.One;
    private static readonly Complex[] Dirs = [Up, Right, Down, Left];

    private static readonly Dictionary<char, Complex[]> Exits = new()
    {
        { '7', [Left, Down] },
        { 'F', [Right, Down] },
        { 'L', [Up, Right] },
        { 'J', [Up, Left] },
        { '|', [Up, Down] },
        { '-', [Left, Right] },
        { 'S', [Up, Down, Left, Right] },
    };
    
    // Returns the positions that make up the loop starting at 'S'
    private static HashSet<Complex> LoopPositions(Map map)
    {
        var position = map.Keys.Single(k => map[k] == 'S');
        var positions = new HashSet<Complex>();
        
        // pick one direction that leads out from S and connected to the neighbour
        var dir = Dirs.First(dir => Exits[map[position + dir]].Contains(-dir));

        for (;;)
        {
            positions.Add(position);
            position += dir;
            if (map[position] == 'S')
            {
                break;
            }
            dir = Exits[map[position]].Single(dirOut => dirOut != -dir);
        }

        return positions;
    }
    
    // Check if position is inside the loop using ray casting algorithm
    private static bool Inside(Map map, Complex position)
    {
        var cell = map[position];
        if (cell != '.')
        {
            return false;
        }
        
        /*
         * Imagine a small elf starting from the top half of a cell and moving
         * to the left jumping over the pipes it encounters. It needs to jump
         * over only 'vertically' oriented pipes, since it runs in the top of the
         * row. Each jump flips the "Inside" variable.
         */

        var inside = false;
        position--;
        while (map.ContainsKey(position))
        {
            if ("SJL|".Contains(map[position]))
            {
                inside = !inside;
            }

            position--;
        }

        return inside;
    }
    
    private static Map ParseMap(string[] input)
    {
        var cRow = input.Length;
        var cCol = input[0].Length;
        var res = new Map();
        for (var iRow = 0; iRow < cRow; iRow++)
        {
            for (var iCol = 0; iCol < cCol; iCol++)
            {
                res[new Complex(iCol, iRow)] = input[iRow][iCol];
            }
        }

        return res;
    }
}