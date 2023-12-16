using System.Numerics;
using FluentAssertions;
using FluentAssertions.Formatting;
using NUnit.Framework;

namespace AOC._2023;

using Map = Dictionary<Complex, char>;
using Beam = (Complex pos, Complex dir);

public class Day16
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day16.txt");

    [Test]
    public void Part1() =>
        EnergizedCells(ParseMap(_input), (Complex.Zero, Right)).Should().Be(7860);

    [Test]
    public void Part2()
    {
        var map = ParseMap(_input);

        (
            from beam in StartBeams(map)
            select EnergizedCells(map, beam)
        ).Max().Should().Be(8331);
    }
    
    private static Map ParseMap(string[] input) => (
        from iRow in Enumerable.Range(0, input.Length)
        from iCol in Enumerable.Range(0, input[0].Length)
        let pos = new Complex(iCol, iRow)
        let cell = input[iRow][iCol]
        select new KeyValuePair<Complex, char>(pos, cell)
    ).ToDictionary();

    // follow the beam in the map and return the energized cell count.
    private static int EnergizedCells(Map map, Beam beam)
    {
        // this is essentially just a flood fill algorithm
        var q = new Queue<Beam>([beam]);
        var seen = new HashSet<Beam>();

        while (q.TryDequeue(out beam))
        {
            seen.Add(beam);
            foreach (var dir in Exits(map[beam.pos], beam.dir))
            {
                var pos = beam.pos + dir;
                if (map.ContainsKey(pos) && !seen.Contains((pos, dir)))
                {
                    q.Enqueue((pos, dir));
                }
            }
        }

        return seen.Select(b => b.pos).Distinct().Count();
    }
    
    // go around the edges of the map and return the inward pointing directions
    private static IEnumerable<Beam> StartBeams(Map map)
    {
        var (tl, br) = (TopLeft(map), BottomRight(map));
        return
        [
            ..from pos in Positions(map, tl, Right) select (pos, Down),
            .. from pos in Positions(map, tl, Down) select (pos, Right),
            .. from pos in Positions(map, br, Left) select (pos, Up),
            .. from pos in Positions(map, br, Up) select (pos, Left)
        ];
    }
    
    // map boundary
    private static Complex TopLeft(Map map) => Complex.Zero;
    private static Complex BottomRight(Map map) => map.Keys.MaxBy(pos => pos.Imaginary + pos.Real);
    
    // allowed positions of the map from 'start' going in 'dir'
    private static IEnumerable<Complex> Positions(Map map, Complex start, Complex dir)
    {
        for (var pos = start; map.ContainsKey(pos); pos += dir)
        {
            yield return pos;
        }
    }
    
    // The 'exit' direction(s) of the given cell when entered by a beam moving in 'dir'
    private static Complex[] Exits(char cell, Complex dir) => (cell, dir.Real, dir.Imaginary) switch
    {
        ('-', 0, _) => [Left, Right],
        ('|', _, 0) => [Up, Down],
        ('/', _, _) => [-new Complex(dir.Imaginary, dir.Real)],
        ('\\', _, _) => [new Complex(dir.Imaginary, dir.Real)],
        _ => [dir]
    };

    private static readonly Complex Up = -Complex.ImaginaryOne;
    private static readonly Complex Down =  Complex.ImaginaryOne;
    private static readonly Complex Left = -Complex.One;
    private static readonly Complex Right = Complex.One;
}