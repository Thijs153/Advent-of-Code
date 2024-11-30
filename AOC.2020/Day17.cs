namespace AOC._2020;

public class Day17
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day17.txt");

    [Fact]
    public void Part1()
    {
        var ds = (
            from dx in new[] { -1, 0, 1 }
            from dy in new[] { -1, 0, 1 }
            from dz in new[] { -1, 0, 1 }
            where dx != 0 || dy != 0 || dz != 0
            select (dx, dy, dz)
        ).ToArray();

        Solve(
            _input,
            (x, y) => (x: x, y: y, z: 0),
            (p) => ds.Select(d => (p.x + d.dx, p.y + d.dy, p.z + d.dz))
        ).Should().Be(348);
    }

    [Fact]
    public void Part2()
    {
        var ds = (
            from dx in new[] { -1, 0, 1 }
            from dy in new[] { -1, 0, 1 }
            from dz in new[] { -1, 0, 1 }
            from dw in new[] { -1, 0, 1 }
            where dx != 0 || dy != 0 || dz != 0 || dw != 0
            select (dx, dy, dz, dw)
        ).ToArray();
        
        Solve(
            _input,
            (x, y) => (x: x, y: y, z: 0, w: 0),
            (p) => ds.Select(d => (p.x + d.dx, p.y + d.dy, p.z + d.dz, p.w + d.dw))
        ).Should().Be(2236);
    }
    
    private static int Solve<T>(string[] lines, Func<int, int, T> create, Func<T, IEnumerable<T>> neighbours)
    {
        var (width, height) = (lines[0].Length, lines.Length);
        var activePoints = new HashSet<T>(
            from x in Enumerable.Range(0, width)
            from y in Enumerable.Range(0, height)
            where lines[y][x] == '#'
            select create(x, y)
        );

        for (var i = 0; i < 6; i++)
        {
            var newActivePoints = new HashSet<T>();
            var inactivePoints = new Dictionary<T, int>();

            foreach (var point in activePoints)
            {
                var activeNeighbours = 0;
                foreach (var neighbour in neighbours(point))
                {
                    if (activePoints.Contains(neighbour))
                    {
                        activeNeighbours++;
                    }
                    else
                    {
                        inactivePoints[neighbour] = inactivePoints.GetValueOrDefault(neighbour) + 1;
                    }
                }

                if (activeNeighbours is 2 or 3)
                {
                    newActivePoints.Add(point);
                }
            }

            foreach (var (point, activeNeighbours) in inactivePoints)
            {
                if (activeNeighbours == 3)
                {
                    newActivePoints.Add(point);
                }
            }

            activePoints = newActivePoints;
        }

        return activePoints.Count;
    }
}