using System.Collections.Immutable;
using FluentAssertions;
using NUnit.Framework;
// ReSharper disable InconsistentNaming

namespace AOC._2022;

[TestFixture]
public class Day12
{
    private readonly Symbol _startSymbol = new('S');
    private readonly Symbol _endSymbol = new('E');
    private readonly Elevation _lowestElevation = new('a');
    private readonly Elevation _highestElevation = new('z');

    [Test]
    public void Part1()
    {
        GetPoints()
            .Single(point => point.Symbol == _startSymbol).DistanceFromGoal
            .Should().Be(370);
    }

    [Test]
    public void Part2()
    {
        GetPoints()
            .Where(point => point.Elevation == _lowestElevation)
            .Select(poi => poi.DistanceFromGoal)
            .Min()
            .Should().Be(363);
    }

    private IEnumerable<Point> GetPoints()
    {
        var map = ParseMap();
        Coord goal = map.Keys.Single(point => map[point] == _endSymbol);

        var pointByCoord = new Dictionary<Coord, Point>()
        {
            { goal, new Point(_endSymbol, GetElevation(_endSymbol), 0) }
        };

        var queue = new Queue<Coord>();
        queue.Enqueue(goal);

        while (queue.Any())
        {
            Coord thisCoord = queue.Dequeue();
            Point thisPoint = pointByCoord[thisCoord];

            foreach (var nextCoord in Neighbours(thisCoord).Where(map.ContainsKey))
            {
                if (pointByCoord.ContainsKey(nextCoord))
                    continue;

                Symbol nextSymbol = map[nextCoord];
                Elevation nextElevation = GetElevation(nextSymbol);

                if (thisPoint.Elevation.Value - nextElevation.Value <= 1)
                {
                    pointByCoord[nextCoord] = new Point()
                    {
                        Symbol = nextSymbol,
                        Elevation = nextElevation,
                        DistanceFromGoal = thisPoint.DistanceFromGoal + 1
                    };
                    queue.Enqueue(nextCoord);
                }
            }
        }

        return pointByCoord.Values;
    }

    private Elevation GetElevation(Symbol symbol) =>
        symbol.Value switch
        {
            'S' => _lowestElevation,
            'E' => _highestElevation,
            _ => new Elevation(symbol.Value)
        };

    private static ImmutableDictionary<Coord, Symbol> ParseMap()
    {
        var lines = File.ReadAllLines("Inputs/Day12.txt");
        return (
            from y in Enumerable.Range(0, lines.Length)
            from x in Enumerable.Range(0, lines[0].Length)
            select new KeyValuePair<Coord, Symbol>(
                new Coord(x, y), new Symbol(lines[y][x])
            )
        ).ToImmutableDictionary();
    }

    private static IEnumerable<Coord> Neighbours(Coord coord) =>
        new[]
        {
            coord with { Lat = coord.Lat + 1 },
            coord with { Lat = coord.Lat - 1 },
            coord with { Lon = coord.Lon + 1 },
            coord with { Lon = coord.Lon - 1 }
        };

    private record struct Coord(int Lat, int Lon);
    private record struct Symbol(char Value);
    private record struct Elevation(char Value);
    private record struct Point(Symbol Symbol, Elevation Elevation, int DistanceFromGoal);
    
}
