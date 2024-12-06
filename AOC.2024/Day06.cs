using System.Numerics;

using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;

namespace AOC._2024;

public class Day06
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day06.txt");

    private static readonly Complex Up = Complex.ImaginaryOne; // (0, 1)
    private static readonly Complex TurnRight = -Complex.ImaginaryOne; // (-0, -1)

    [Fact]
    public void Part1()
    {
        var (map, start) = Parse(_input);
        
        Walk(map, start).positions
            .Count()
            .Should().Be(4883);
    }

    [Fact]
    public void Part2()
    {
        var (map, start) = Parse(_input);
        var positions = Walk(map, start).positions;
        var loops = 0;
        
        // try a blocker in each location visited by the guard and count the loops.
        foreach (var block in positions.Skip(1)) // skip starting position.
        {
            map[block] = '#';
            if (Walk(map, start).isLoop)
            {
                loops++;
            }

            map[block] = '.';
        }

        loops.Should().Be(1655);
    }
    
    // returns the positions visited when starting from 'position', isLoop is set if the
    // guard enters a cycle.
    private static (IEnumerable<Complex> positions, bool isLoop) Walk(Map map, Complex position)
    {
        var seen = new HashSet<(Complex position, Complex direction)>();
        var direction = Up;

        while (map.ContainsKey(position) && !seen.Contains((position, direction)))
        {
            seen.Add((position, direction));
            if (map.GetValueOrDefault(position + direction) == '#')
            {
                direction *= TurnRight;
            }
            else
            {
                position += direction;
            }
        }

        return (
            positions: seen.Select(s => s.position).Distinct(),
            isLoop: seen.Contains((position, direction))
        );
    }
    
    // start = starting position of the guard.
    private static (Map map, Complex start) Parse(string[] input)
    {
        // [0][0] -> position (0, 0)
        // [0][1] -> position (0, -1)
        var map = (
            from y in Enumerable.Range(0, input.Length)
            from x in Enumerable.Range(0, input[0].Length)
            select new KeyValuePair<Complex, char>(-Up * y + x, input[y][x])
        ).ToDictionary();

        var start = map.First(x => x.Value == '^').Key;

        return (map, start);
    }
}