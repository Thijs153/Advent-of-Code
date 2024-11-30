using System.Numerics;

namespace AOC._2023;

public class Day18
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day18.txt");

    [Fact]
    public void Part1() => Area(Steps1(_input)).Should().Be(46394);

    [Fact]
    public void Part2() => Area(Steps2(_input)).Should().Be(201398068194715);

    private static IEnumerable<Complex> Steps1(string[] input) =>
        from line in input
        let parts = line.Split(' ')
        let dir = parts[0] switch
        {
            "R" => Complex.One,
            "U" => -Complex.ImaginaryOne,
            "L" => -Complex.One,
            "D" => Complex.ImaginaryOne,
            _ => throw new Exception()
        }
        let dist = int.Parse(parts[1])
        select dir * dist;

    private static IEnumerable<Complex> Steps2(string[] input) =>
        from line in input
        let hex = line.Split(' ')[2]
        let dir = hex[7] switch
        {
            '0' => Complex.One,
            '1' => -Complex.ImaginaryOne,
            '2' => -Complex.One,
            '3' => Complex.ImaginaryOne,
            _ => throw new Exception()
        }
        let dist = Convert.ToInt32(hex[2..7], 16)
        select dir * dist;

    // We are using a combination of the shoelace formula with Pick's theorem
    private static double Area(IEnumerable<Complex> steps)
    {
        var vertices = Vertices(steps).ToList();

        // Shoelace formula
        var shiftedVertices = vertices.Skip(1).Append(vertices[0]);
        var shoelaces =
            from points in vertices.Zip(shiftedVertices)
            let p1 = points.First
            let p2 = points.Second
            select p1.Real * p2.Imaginary - p1.Imaginary * p2.Real;
        var area = Math.Abs(shoelaces.Sum()) / 2;
        
        // Pick's theorem
        var boundary = steps.Select(x => x.Magnitude).Sum();
        var interior = area - boundary / 2 + 1;
        
        // Integer area
        return boundary + interior;
    }
    
    private static IEnumerable<Complex> Vertices(IEnumerable<Complex> steps)
    {
        var pos = Complex.Zero;
        foreach (var step in steps)
        {
            pos += step;
            yield return pos;
        }
    } 
}