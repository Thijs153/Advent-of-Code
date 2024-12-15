using System.Collections.Immutable;
using System.Numerics;

namespace AOC._2024;

using Map = ImmutableDictionary<Complex, char>;

public class Day15
{
    private readonly string _input = File.ReadAllText("Inputs/Day15.txt").ReplaceLineEndings("\n");

    private static readonly Complex Up = -Complex.ImaginaryOne;
    private static readonly Complex Down = Complex.ImaginaryOne;
    private static readonly Complex Right = 1;
    private static readonly Complex Left = -1;
    
    [Fact]
    public void Part1() => Solve(_input).Should().Be(1475249);

    [Fact]
    public void Part2() => Solve(ScaleUp(_input)).Should().Be(1509724);
    
    private static double Solve(string input)
    {
        var (map, steps) = Parse(input);
        var robot = map.Keys.Single(k => map[k] == '@');

        foreach (var step in steps)
        {
            if (TryToStep(ref map, robot, step))
            {
                robot += step;
            }
        }

        return map.Keys
            .Where(k => map[k] == '[' || map[k] == 'O')
            .Sum(box => box.Real + 100 * box.Imaginary);
    }
    
    /*
     * Attempts to move the robot in the given direction on the map, pushing boxes as necessary.
     * If the move is successful, the map is updated to reflect the new positions and the function returns true.
     * Otherwise, the map remains unchanged and the function returns false;
     */
    private static bool TryToStep(ref Map map, Complex position, Complex direction)
    {
        var mapOrig = map;

        if (map[position] == '.')
        {
            return true;
        }
        if (map[position] == 'O' || map[position] == '@')
        {
            if (TryToStep(ref map, position + direction, direction))
            {
                map = map
                    .SetItem(position + direction, map[position])
                    .SetItem(position, '.');
                return true;
            }
        }
        else if (map[position] == ']')
        {
            return TryToStep(ref map, position + Left, direction);
        }
        else if (map[position] == '[')
        {
            if (direction == Left)
            {
                if (TryToStep(ref map, position + Left, direction))
                {
                    map = map
                        .SetItem(position + Left, '[')
                        .SetItem(position, ']')
                        .SetItem(position + Right, '.');
                    return true;
                }
            } 
            else if (direction == Right)
            {
                if (TryToStep(ref map, position + 2 * Right, direction))
                {
                    map = map
                        .SetItem(position, '.')
                        .SetItem(position + Right, '[')
                        .SetItem(position + 2 * Right, ']');
                    return true;
                }
            }
            else
            {
                if (TryToStep(ref map, position + direction, direction) &&
                    TryToStep(ref map, position + Right + direction, direction))
                {
                    map = map
                        .SetItem(position, '.')
                        .SetItem(position + Right, '.')
                        .SetItem(position + direction, '[')
                        .SetItem(position + direction + Right, ']');
                    return true;
                }
            }
        }

        map = mapOrig;
        return false;
    }

    private static string ScaleUp(string input) =>
        input.Replace("#", "##").Replace(".", "..").Replace("O", "[]").Replace("@", "@.");
    
    private static (Map, Complex[]) Parse(string input) {
        var blocks = input.Split("\n\n");
        var lines = blocks[0].Split("\n");
        var map = (
            from y in Enumerable.Range(0, lines.Length)
            from x in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Complex, char>(x + y * Down, lines[y][x])
        ).ToImmutableDictionary();

        var steps = blocks[1].ReplaceLineEndings("").Select(ch =>
            ch switch {
                '^' => Up,
                '<' => Left,
                '>' => Right,
                'v' => Down,
                _ => throw new Exception()
            });

        return (map, steps.ToArray());
    }
}