using System.Drawing;

namespace AOC._2021;

public class Day11
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day11.txt");

    [Fact]
    public void Part1()
    {
        Simulate(_input).Take(100).Sum()
            .Should().Be(1647);
    }

    [Fact]
    public void Part2()
    {
        (Simulate(_input).TakeWhile(flash => flash != 100).Count() + 1)
            .Should().Be(348);
    }
    
    private static IEnumerable<int> Simulate(string[] input)
    {
        var map = Parse(input);

        while (true)
        {
            var queue = new Queue<Point>();
            var flashed = new HashSet<Point>();
            
            // increase the energy level of each octopus:
            foreach (var key in map.Keys)
            {
                map[key]++;
                if (map[key] == 10)
                {
                    queue.Enqueue(key);
                }
            }
            
            // those that reach level 10 should flash
            while (queue.Any())
            {
                var point = queue.Dequeue();
                flashed.Add(point);
                foreach (var n in Neighbours(point).Where(x => map.ContainsKey(x)))
                {
                    map[n]++;
                    if (map[n] == 10)
                    {
                        queue.Enqueue(n);
                    }
                }
            }
            
            // reset energy level of flashed octopuses
            foreach (var point in flashed)
            {
                map[point] = 0;
            }

            yield return flashed.Count;
        }
    }

    private static Dictionary<Point, int> Parse(string[] input) =>
        new(
            from y in Enumerable.Range(0, input.Length)
            from x in Enumerable.Range(0, input[0].Length)
            select new KeyValuePair<Point, int>(new Point(x, y), input[y][x] - '0')
        );

    private static IEnumerable<Point> Neighbours(Point point) =>
        from dx in new[] { -1, 0, 1 }
        from dy in new[] { -1, 0, 1 }
        where dx != 0 || dy != 0
        select new Point(point.X + dx, point.Y + dy);
}