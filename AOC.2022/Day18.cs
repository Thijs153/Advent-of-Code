namespace AOC._2022;

public class Day18
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day18.txt");

    [Fact]
    public void Part1()
    {
        var lavaLocations = GetLavaLocations(_input).ToHashSet();
        lavaLocations
            .SelectMany(Neighbours)
            .Count(p => !lavaLocations.Contains(p))
            .Should().Be(4608);
    }

    [Fact]
    public void Part2()
    {
        var lavaLocations = GetLavaLocations(_input).ToHashSet();
        var bounds = GetBounds(lavaLocations);
        var waterLocations = FillWithWater(bounds.Min, bounds, lavaLocations);

        lavaLocations
            .SelectMany(Neighbours)
            .Count(p => waterLocations.Contains(p))
            .Should().Be(2652);
    }

    private static HashSet<Point> FillWithWater(Point from, Bounds bounds, HashSet<Point> lavaLocations)
    {
        var result = new HashSet<Point>();
        var queue = new Queue<Point>();

        result.Add(from);
        queue.Enqueue(from);
        while (queue.Any())
        {
            var water = queue.Dequeue();
            foreach (var neighbour in Neighbours(water))
            {
                if (result.Contains(neighbour) ||
                    !Within(bounds, neighbour) ||
                    lavaLocations.Contains(neighbour)) continue;
                
                result.Add(neighbour);
                queue.Enqueue(neighbour);
            }
        }

        return result;
    }

    private IEnumerable<Point> GetLavaLocations(string[] input) =>
        from line in input
        let coords = line.Split(",").Select(int.Parse).ToArray()
        select new Point(coords[0], coords[1], coords[2]);

    private static Bounds GetBounds(IEnumerable<Point> points)
    {
        points = points.ToList();
        var minX = points.Select(p => p.X).Min() - 1;
        var maxX = points.Select(p => p.X).Max() + 1;

        var minY = points.Select(p => p.Y).Min() - 1;
        var maxY = points.Select(p => p.Y).Max() + 1;
        
        var minZ = points.Select(p => p.Z).Min() - 1;
        var maxZ = points.Select(p => p.Z).Max() + 1;

        return new Bounds(new Point(minX, minY, minZ), new Point(maxX, maxY, maxZ));
    }
    
    private static bool Within(Bounds bounds, Point point) =>
        bounds.Min.X <= point.X && point.X <= bounds.Max.X &&
        bounds.Min.Y <= point.Y && point.Y <= bounds.Max.Y &&
        bounds.Min.Z <= point.Z && point.Z <= bounds.Max.Z;

    private static IEnumerable<Point> Neighbours(Point point) =>
        new[]
        {
            point with { X = point.X - 1 },
            point with { X = point.X + 1 },
            point with { Y = point.Y - 1 },
            point with { Y = point.Y + 1 },
            point with { Z = point.Z - 1 },
            point with { Z = point.Z + 1 }
        };

    private record Point(int X, int Y, int Z);

    private record Bounds(Point Min, Point Max);
    
}