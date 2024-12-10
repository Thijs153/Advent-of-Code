using System.Numerics;

namespace AOC._2024;

using Map = Dictionary<Complex, char>;

public class Day10
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day10.txt");

    private static readonly Complex Up = Complex.ImaginaryOne;
    private static readonly Complex Down = -Complex.ImaginaryOne;
    private static readonly Complex Left = 1;
    private static readonly Complex Right = -1;
    
    [Fact]
    public void Part1()
    {
        GetAllTrails(GetMap(_input))
            .Sum(t => t.Value.Distinct().Count())
            .Should().Be(652);
    }
    
    [Fact]
    public void Part2()
    {
        GetAllTrails(GetMap(_input))
            .Sum(t => t.Value.Count)
            .Should().Be(1432);
    }
    
    private static Dictionary<Complex, List<Complex>> GetAllTrails(Map map) =>
        GetTrailHeads(map).ToDictionary(t => t, t => GetTrailsFrom(map, t));

    private static IEnumerable<Complex> GetTrailHeads(Map map) => 
        map.Keys.Where(pos => map[pos] == '0');
    
    private static List<Complex> GetTrailsFrom(Map map, Complex trailHead)
    {
        // standard flood fill algorithm using a queue
        var positions = new Queue<Complex>();
        positions.Enqueue(trailHead);

        var trails = new List<Complex>();
        while (positions.Any())
        {
            var point = positions.Dequeue();
            if (map[point] == '9')
            {
                trails.Add(point);
            }
            else
            {
                foreach (var direction in new[] { Up, Down, Left, Right})
                {
                    if (map.GetValueOrDefault(point + direction) == map[point] + 1)
                    {
                        positions.Enqueue(point + direction);
                    }
                }
            }
        }

        return trails;
    }
    
    private static Map GetMap(string[] input) => (
        from y in Enumerable.Range(0, input.Length)
        from x in Enumerable.Range(0, input[0].Length)
        select new KeyValuePair<Complex, char>(Down * y + x, input[y][x])
    ).ToDictionary();
}