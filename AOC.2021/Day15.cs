using System.Drawing;

namespace AOC._2021;

public class Day15
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day15.txt");

    [Fact]
    public void Part1()
    {
        Solve(Parse(_input))
            .Should().Be(696);
    }

    [Fact]
    public void Part2()
    {
        Solve(ScaleUp(Parse(_input)))
            .Should().Be(2952);
    }

    private static int Solve(Dictionary<Point, int> riskMap)
    {
        // Dijkstra's algorithm

        var topLeft = new Point(0, 0);
        var bottomRight = new Point(riskMap.Keys.MaxBy(p => p.X).X, riskMap.Keys.MaxBy(p => p.Y).Y);
        
        // Visit points in order of cumulated risk
        var queue = new PriorityQueue<Point, int>();
        var totalRiskMap = new Dictionary<Point, int>
        {
            [topLeft] = 0
        };

        queue.Enqueue(topLeft, 0);
        
        // Go until we find the bottom right corner
        while (queue.Count > 0)
        {
            var p = queue.Dequeue();

            if (p == bottomRight)
                break;

            foreach (var n in Neighbours(p))
            {
                if (riskMap.TryGetValue(n, out var value))
                {
                    var totalRiskThroughP = totalRiskMap[p] + value;
                    if (totalRiskThroughP < totalRiskMap.GetValueOrDefault(n, int.MaxValue))
                    {
                        totalRiskMap[n] = totalRiskThroughP;
                        queue.Enqueue(n, totalRiskThroughP);
                    }
                }
            }
        }
        
        // return bottom right corner's total risk:
        return totalRiskMap[bottomRight];
    }

    // Create a 5x scaled up map, as described in part 2
    private static Dictionary<Point, int> ScaleUp(Dictionary<Point, int> map)
    {
        var (cCol, cRow) = (map.Keys.MaxBy(p => p.X).X + 1, map.Keys.MaxBy(p => p.Y).Y + 1);

        var res = new Dictionary<Point, int>(
            from y in Enumerable.Range(0, cRow * 5)
            from x in Enumerable.Range(0, cCol * 5)

            // x, y and risk level in the original map:
            let tileY = y % cRow
            let tileX = x % cCol
            let tileRiskLevel = map[new Point(tileX, tileY)]

            // risk level is increased by tile distance from origin:
            let tileDistance = (y / cRow) + (x / cCol)

            // risk level wraps around from 9 to 1:
            let riskLevel = (tileRiskLevel + tileDistance - 1) % 9 + 1
            select new KeyValuePair<Point, int>(new Point(x, y), riskLevel)
        );

        return res;
    }

    private static Dictionary<Point, int> Parse(string[] input)
    {
        return new Dictionary<Point, int>(
            from y in Enumerable.Range(0, input.Length)
            from x in Enumerable.Range(0, input[0].Length)
            select new KeyValuePair<Point, int>(new Point(x, y), input[y][x] - '0')
        );
    }

    private static IEnumerable<Point> Neighbours(Point point) =>
    [
        point with { Y = point.Y + 1 },
            point with { Y = point.Y - 1 },
            point with { X = point.X + 1 },
            point with { X = point.X - 1 }
    ];
}