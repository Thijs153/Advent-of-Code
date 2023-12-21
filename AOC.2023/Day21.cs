using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

public class Day21
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day21.txt");

    [Test]
    public void Part1()
    {
        var map = ParseMap(_input);
        var s = map.Keys.Single(k => map[k] == 'S');
        var pos = new HashSet<Complex> { s };

        for (var i = 0; i < 64; i++)
        {
            pos = Step(map, pos);
        }

        pos.Count.Should().Be(3764);
    }

    [Test]
    public void Part2()
    {
        const int steps = 26501365;
        const int loop = 260;

        var map = ParseMap(_input);
        var center = new Complex(65, 65);

        Complex[] corners = [
            new Complex(0, 0), new Complex(0, 130),
            new Complex(130, 130), new Complex(130, 0),
        ];

        Complex[] middles = [
            new Complex(65, 0), new Complex(65, 130),
            new Complex(0, 65), new Complex(130, 65),
        ];

        var cohorts = new Dictionary<Complex, long[]>();
        cohorts[center] = new long[loop + 1];
        
        foreach (var corner in corners)
        {
            cohorts[corner] = new long[loop + 1];
        }

        foreach (var middle in middles)
        {
            cohorts[middle] = new long[loop + 1];
        }

        var m = 0;
        cohorts[center][m] = 1;
        var phaseLength = loop + 1;
        
        for (var i = 1; i <= steps; i++)
        {
            var nextM = (m + phaseLength - 1) % phaseLength;
            foreach (var item in cohorts.Keys)
            {
                var phase = cohorts[item];
                var a = phase[(m + phase.Length - 1) % phase.Length];
                var b = phase[(m + phase.Length - 2) % phase.Length];
                var c = phase[(m + phase.Length - 3) % phase.Length];

                phase[nextM] = 0;
                phase[(nextM + phase.Length - 1) % phase.Length] = b;
                phase[(nextM + phase.Length - 2) % phase.Length] = a + c;
            }

            m = nextM;

            if (i >= 132 && (i - 132) % 131 == 0)
            {
                var newItems = i / 131;
                foreach (var corner in corners)
                {
                    cohorts[corner][m] += newItems;
                }
            } else if (i >= 66 && (i - 66) % 131 == 0)
            {
                foreach (var middle in middles)
                {
                    cohorts[middle][m]++;
                }
            }
        }

        var res = 0L;

        foreach (var item in cohorts.Keys)
        {
            var phase = cohorts[item];
            var pos = new HashSet<Complex> { item };

            for (var i = 0; i < phase.Length; i++)
            {
                var count = phase[(m + i) % phase.Length];
                res += pos.Count * count;
                pos = Step(map, pos);
            }
        }

        res.Should().Be(622926941971282);
    }
    

    private static HashSet<Complex> Step(Dictionary<Complex, char> map, HashSet<Complex> pos)
    {
        var res = new HashSet<Complex>();
        foreach (var p in pos)
        {
            foreach (var dir in new[] { 1, -1, Complex.ImaginaryOne, -Complex.ImaginaryOne})
            {
                var pT = p + dir;
                if (map.TryGetValue(pT, out var value) && value != '#')
                {
                    res.Add(pT);
                }
            }
        }

        return res;
    }
    
    private static Dictionary<Complex, char> ParseMap(string[] input) => (
        from iRow in Enumerable.Range(0, input.Length)
        from iCol in Enumerable.Range(0, input[0].Length)
        select new KeyValuePair<Complex, char>(
            new Complex(iCol, iRow), input[iRow][iCol]
        )
    ).ToDictionary();
}