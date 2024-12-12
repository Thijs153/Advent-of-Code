using System.Numerics;

namespace AOC._2024;

using Region = HashSet<Complex>;

public class Day12
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day12.txt");
    
    private static readonly Complex Up = Complex.ImaginaryOne;
    private static readonly Complex Down = -Complex.ImaginaryOne;
    private static readonly Complex Left = 1;
    private static readonly Complex Right = -1;

    [Fact]
    public void Part1() => CalculateFencePrice(_input, FindEdges).Should().Be(1477762);
    
    [Fact]
    public void Part2() => CalculateFencePrice(_input, FindCorners).Should().Be(923480);
    
    private static int CalculateFencePrice(string[] input, MeasurePerimeter measure)
    {
        var regions = GetRegions(input);

        return (
            from region in regions.Values.Distinct() 
            let perimeter = region.Sum(point => measure(regions, point)) 
            select region.Count * perimeter
        ).Sum();
    }
    
    private delegate int MeasurePerimeter(Dictionary<Complex, Region> map, Complex pt);
    
    private static int FindEdges(Dictionary<Complex, Region> map, Complex pt)
    {
        var region = map[pt];
        Complex[] directions = [Up, Right, Down, Left];
        
        return directions.Count(dir => map.GetValueOrDefault(pt + dir) != region);
    }

    private static int FindCorners(Dictionary<Complex, Region> map, Complex pt)
    {
        var region = map[pt];
        
        var result = 0;
        // rotate du and dv and check for the 4 corner types.
        foreach (var (du, dv) in new [] { (Up, Right), (Right, Down), (Down, Left), (Left, Up)})
        {
            //  .
            //  x. (outside corner)
            if (map.GetValueOrDefault(pt + du) != region &&
                map.GetValueOrDefault(pt + dv) != region)
            {
                result++;
            }

            //  x.
            //  xx (inside corner)
            if (map.GetValueOrDefault(pt + du) == region &&
                map.GetValueOrDefault(pt + dv) == region &&
                map.GetValueOrDefault(pt + du + dv) != region)
            {
                result++;
            }
        }

        return result;
    }
    
    // Maps the positions of plants in a garden to their corresponding regions,
    // grouping plants of the same type into contiguous regions.
    private static Dictionary<Complex, Region> GetRegions(string[] input)
    {
        var garden = (
            from y in Enumerable.Range(0, input.Length)
            from x in Enumerable.Range(0, input[0].Length)
            select new KeyValuePair<Complex, char>(Down * y + x, input[x][y])
        ).ToDictionary();
        
        // go over the positions of the garden and use a flood fill to determine the region
        var result = new Dictionary<Complex, Region>();
        var positions = garden.Keys.ToHashSet();
        while (positions.Any())
        {
            var pivot = positions.First();
            var region = new Region { pivot };

            var queue = new Queue<Complex>();
            queue.Enqueue(pivot);

            var plant = garden[pivot];
            
            while (queue.Any())
            {
                var point = queue.Dequeue();
                result[point] = region;
                positions.Remove(point);

                foreach (var direction in new[] { Up, Down, Left, Right})
                {
                    if (!region.Contains(point + direction) && garden.GetValueOrDefault(point + direction) == plant)
                    {
                        region.Add(point + direction);
                        queue.Enqueue(point + direction);
                    }
                }
            }
        }

        return result;
    }
}