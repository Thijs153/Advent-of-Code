using System.Text.RegularExpressions;

namespace AOC._2021;

public class Day22
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day22.txt");

    [Fact]
    public void Part1()
    {
        ActiveCubesInRange(_input, 50).Should().Be(642125);
    }

    [Fact]
    public void Part2()
    {
        ActiveCubesInRange(_input, int.MaxValue).Should().Be(1235164413198198);
    }


    private static long ActiveCubesInRange(string[] input, int range)
    {
        var cmds = Parse(input);
        
        // Recursive approach
        
        // If we can determine the number of active cubes in subregions
        // we can compute the effect of the i-th cmd as well:
        long ActiveCubesAfterICmd(int icmd, Region region)
        {
            if (region.IsEmpty || icmd < 0)
            {
                return 0; // empty is empty
            }

            var intersection = region.Intersect(cmds[icmd].region);
            var activeInRegion = ActiveCubesAfterICmd(icmd - 1, region);
            var activeInIntersection = ActiveCubesAfterICmd(icmd - 1, intersection);
            var activeOutsideIntersection = activeInRegion - activeInIntersection;
                
            // outside the intersection is unaffected, the rest is either on or off:
            return cmds[icmd].turnOff ? activeOutsideIntersection : activeOutsideIntersection + intersection.Volume;
        }

        return ActiveCubesAfterICmd(
            cmds.Length - 1,
            new Region(new Segment(-range, range), new Segment(-range, range), new Segment(-range, range))
        );
    }

    private static Cmd[] Parse(string[] input)
    {
        var res = new List<Cmd>();
        foreach (var line in input)
        {
            var turnOff = line.StartsWith("off");
            // get all the numbers with a regexp:
            var m = Regex.Matches(line, "-?[0-9]+").Select(m => int.Parse(m.Value)).ToArray();
            res.Add(new Cmd(turnOff, new Region(new Segment(m[0], m[1]), new Segment(m[2], m[3]), new Segment(m[4], m[5]))));
        }

        return res.ToArray();
    }
    
    private record Cmd(bool turnOff, Region region);

    private record Segment(int from, int to)
    {
        public bool IsEmpty => from > to;
        public long Length => IsEmpty ? 0 : to - from + 1;

        public Segment Intersect(Segment that) => new(Math.Max(from, that.from), Math.Min(to, that.to));
    }

    private record Region(Segment x, Segment y, Segment z)
    {
        public bool IsEmpty => x.IsEmpty || y.IsEmpty || z.IsEmpty;
        public long Volume => x.Length * y.Length * z.Length;

        public Region Intersect(Region that) => new(x.Intersect(that.x), y.Intersect(that.y), z.Intersect(that.z));
    }
}