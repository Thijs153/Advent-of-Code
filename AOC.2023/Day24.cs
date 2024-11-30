using System.Numerics;
using System.Text.RegularExpressions;

namespace AOC._2023;

public class Day24
{
    private readonly string _input = File.ReadAllText("Inputs/Day24.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        const long areaBegin = 200000000000000;
        const long areaEnd = 400000000000000;
        var particles = ParseParticles(_input);
        var res = 0;
        
        for (var i = 0; i < particles.Length; i++) {
            for (var j = i + 1; j < particles.Length; j++) {
                var mp = MeetPoint(particles[i], particles[j]);
                
                if (mp == null!) 
                    continue;
                
                if (mp.x is < areaBegin or > areaEnd) 
                    continue;
                
                if (mp.y is < areaBegin or > areaEnd) 
                    continue;
                
                if (Past(particles[i], mp)) 
                    continue;
                
                if (Past(particles[j], mp)) 
                    continue;
                
                res++;
            }
        }

        res.Should().Be(20336);
    }

    [Fact]
    public void Part2()
    {
        var particles = ParseParticles3(_input);
        var res = Solve(v => v.x, particles) + Solve(v => v.y, particles) + Solve(v => v.z, particles);
        res.Should().Be(677656046662770);
    }
    
    private static bool Past(Particle p, Vec2 v)
    {
        // p.pos.x + t * p.vel.x = v.x
        if (p.vel.x == 0)
        {
            return true;
        }

        return (v.x - p.pos.x) / p.vel.x < 0;
    }
    
    private static Vec2 MeetPoint(Particle p1, Particle p2)
    {
        var m1 = new Mat2(
            p1.vel.y, -p1.vel.x,
            p2.vel.y, -p2.vel.x
        );

        var det = m1.Det;
        if (det == 0)
        {
            return null!;
        }

        var v = new Vec2(
            p1.vel.y * p1.pos.x - p1.vel.x * p1.pos.y,
            p2.vel.y * p2.pos.x - p2.vel.x * p2.pos.y
        );

        return m1.Inv() * v;
    }
    
    private static BigInteger Solve(Func<Vec3, BigInteger> dim, Particle3[] particles)
    {
        for (var v0 = -10000; v0 < 10000; v0++)
        {
            var items = new List<(BigInteger dv, BigInteger x)>();
            foreach (var p in particles)
            {
                var dv = v0 - dim(p.vel);
                if (IsPrime(dv) && items.All(i => i.dv != dv))
                {
                    items.Add((dv, x: dim(p.pos)));
                }
            }

            if (items.Count <= 1) 
                continue;
            
            var p0 = ChineseRemainderTheorem(items.ToArray());
            var ok = true;
            foreach (var p in particles)
            {
                var dv = v0 - dim(p.vel);
                var dx = dim(p.pos) > p0 ? dim(p.pos) - p0 : p0 - dim(p.pos);
                if (dv == 0)
                {
                    if (dx != 0)
                    {
                        ok = false;
                    }
                }
                else
                {
                    if (dx % dv != 0)
                    {
                        ok = false;
                    }
                }
            }

            if (ok)
            {
                return p0;
            }
            
        }

        throw new Exception();
    }
    
    private static bool IsPrime(BigInteger number)
    {
        if (number <= 2) return false;
        if (number % 2 == 0) return false;

        for (var i = 3; i * i <= number; i += 2)
        {
            if (number % i == 0) return false;
        }

        return true;
    }
    
    private static Particle[] ParseParticles(string input) => (
        from line in input.Split('\n')
        let v = Regex.Matches(line, @"-?\d+").Select(m => decimal.Parse(m.Value)).ToArray()
        select new Particle(new Vec2(v[0], v[1]), new Vec2(v[3], v[4]))
    ).ToArray();

    private static Particle3[] ParseParticles3(string input) => (
        from line in input.Split('\n')
        let v = Regex.Matches(line, @"-?\d+").Select(m => BigInteger.Parse(m.Value)).ToArray()
        select new Particle3(new Vec3(v[0], v[1], v[2]), new Vec3(v[3], v[4], v[5]))
    ).ToArray();
    
    private static BigInteger ChineseRemainderTheorem((BigInteger mod, BigInteger a)[] items)
    {
        var prod = items.Aggregate(BigInteger.One, (acc, item) => acc * item.mod);
        var sum = items.Select((item, _) =>
        {
            var p = prod / item.mod;
            return item.a * ModInv(p, item.mod) * p;
        });

        var s = sum.Aggregate(BigInteger.Zero, (current, item) => current + item);

        return s % prod;
    }

    private static BigInteger ModInv(BigInteger a, BigInteger m) =>
        BigInteger.ModPow(a, m - 2, m);
    
    private record Particle(Vec2 pos, Vec2 vel);
    private record Particle3(Vec3 pos, Vec3 vel);

    private record Vec2(decimal x, decimal y);

    private record Vec3(BigInteger x, BigInteger y, BigInteger z)
    {
        public static Vec3 operator +(Vec3 v1, Vec3 v2) =>
            new(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        
        public static Vec3 operator -(Vec3 v1, Vec3 v2) =>
            new(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        
        public static Vec3 operator *(BigInteger d, Vec3 v1) =>
             new(d * v1.x, d * v1.y, d * v1.z);
    }

    private record Mat2(decimal a, decimal b, decimal c, decimal d)
    {
        public decimal Det => a * d - b * c;

        public Mat2 Inv()
        {
            var det = Det;
            return new Mat2(d / det, -b / det, -c / det, a / det);
        }
        
        public static Mat2 operator *(Mat2 m1, Mat2 m2) =>
            new(
                m1.a * m2.a + m1.b * m2.c,
                m1.a * m2.b + m1.b * m2.d,
                m1.b * m2.a + m1.d * m2.c,
                m1.b * m2.b + m1.d * m2.d
            );
        

        public static Vec2 operator *(Mat2 m1, Vec2 v) =>
            new(
                m1.a * v.x + m1.b * v.y,
                m1.c * v.x + m1.d * v.y
            );
        
    }
}