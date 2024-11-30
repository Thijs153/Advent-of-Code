using System.Numerics;

using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;

namespace AOC._2023;

public class Day13
{
    private readonly string _input = File.ReadAllText("Inputs/Day13.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1() =>
        Solve(_input, 0).Should().Be(30802);

    [Fact]
    public void Part2() =>
        Solve(_input, 1).Should().Be(37876);

    private static readonly Complex Right = 1;
    private static readonly Complex Down = Complex.ImaginaryOne;
    private static Complex Ortho(Complex dir) => dir == Right ? Down : Right;

    private static double Solve(string input, int allowedSmudges) => (
        from block in input.Split("\n\n")
        let map = ParseMap(block)
        select GetScore(map, allowedSmudges)
    ).Sum();
    
    // place a mirror along the edges of the map, find the one that has the allowed smudges
    private static double GetScore(Map map, int allowedSmudges) => (
        from dir in new[] { Right, Down }
        from mirror in Positions(map, dir, dir)
        where FindSmudges(map, mirror, dir) == allowedSmudges
        select mirror.Real + 100 * mirror.Imaginary
    ).First();
    
    // cast a ray from each position along the mirror and count the smudges
    private static int FindSmudges(Map map, Complex mirror, Complex rayDir) => (
        from ray0 in Positions(map, mirror, Ortho(rayDir))
        let rayA = Positions(map, ray0, rayDir)
        let rayB = Positions(map, ray0 - rayDir, -rayDir)
        select rayA.Zip(rayB).Count(p => map[p.First] != map[p.Second])
    ).Sum();

    // allowed positions of the map from 'start' going in 'dir'
    private static IEnumerable<Complex> Positions(Map map, Complex start, Complex dir)
    {
        for (var pos = start; map.ContainsKey(pos); pos += dir)
        {
            yield return pos;
        }
    }
    
    private static Map ParseMap(string input)
    {
        var rows = input.Split("\n");
        return (
            from iRow in Enumerable.Range(0, rows.Length)
            from iCol in Enumerable.Range(0, rows[0].Length)
            let pos = new Complex(iCol, iRow)
            let cell = rows[iRow][iCol]
            select new KeyValuePair<Complex, char>(pos, cell)
        ).ToDictionary();
    }
}