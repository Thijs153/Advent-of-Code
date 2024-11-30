using System.Collections.Immutable;
using System.Drawing;

namespace AOC._2021;

public class Day09
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day09.txt");

    [Fact]
    public void Part1()
    {
        var map = GetMap(_input);

        GetLowPoints(map)
            .Select(p => 1 + map[p]).Sum()
            .Should().Be(554);
    }

    [Fact]
    public void Part2()
    {
        var map = GetMap(_input);

        GetLowPoints(map)
            .Select(p => BasinSize(map, p))
            .OrderByDescending(basinSize => basinSize)
            .Take(3)
            .Aggregate(1, (m, basinSize) => m * basinSize)
            .Should().Be(1017792);
    }

    private static ImmutableDictionary<Point, int> GetMap(string[] input)
    {
        var map = input;
        return (
            from y in Enumerable.Range(0, map.Length)
            from x in Enumerable.Range(0, map[0].Length)
            select new KeyValuePair<Point, int>(new Point(x, y), map[y][x] - '0')
        ).ToImmutableDictionary();
    }

    private static IEnumerable<Point> Neighbours(Point point) =>
    [
        point with { Y = point.Y + 1 },
            point with { Y = point.Y - 1 },
            point with { X = point.X + 1 },
            point with { X = point.X - 1 }
    ];

    private static IEnumerable<Point> GetLowPoints(ImmutableDictionary<Point, int> map) =>
        from point in map.Keys
        // point is low if each of its neighbours is higher:
        where Neighbours(point).All(neighbour => map[point] < map.GetValueOrDefault(neighbour, 9))
        select point;

    private static int BasinSize(ImmutableDictionary<Point, int> map, Point point)
    {
        // flood fill algorithm
        var filled = new HashSet<Point> { point };
        var queue = new Queue<Point>(filled);

        while (queue.Any())
        {
            foreach (var neighbour in Neighbours(queue.Dequeue()).Except(filled))
            {
                if (map.GetValueOrDefault(neighbour, 9) != 9)
                {
                    queue.Enqueue(neighbour);
                    filled.Add(neighbour);
                }
            }
        }

        return filled.Count;
    }
}