using System.Numerics;

namespace AOC._2024;

public class Day20
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day20.txt");

    [Fact]
    public void Part1() => Solve(_input, cheat: 2).Should().Be(1389);

    [Fact]
    public void Part2() => Solve(_input, cheat: 20).Should().Be(1005068);
    
    private static int Solve(string[] input, int cheat)
    {
        var path = GetPath(input);
        var indices = Enumerable.Range(0, path.Length).ToArray();

        return indices.AsParallel().Select(CheatsFromI).Sum();

        // sum up the worthy cheats for each index along the path.
        int CheatsFromI(int i) => (
            from j in indices[..i] // only look at points further from the finish. 
            let distance = Manhattan(path[i], path[j]) 
            let saving = i - (j + distance) 
            where distance <= cheat && saving >= 100 
            select 1
        ).Sum();
    }

    private static int Manhattan(Complex a, Complex b) =>
        (int)(Math.Abs(a.Imaginary - b.Imaginary) + Math.Abs(a.Real - b.Real));
    
    // Follow the path from finish to start, supposed that there is a single track in the input.
    // The index of a position in the returned array equals to its distance from the finish.
    private static Complex[] GetPath(string[] input)
    {
        var map = (
            from y in Enumerable.Range(0, input.Length)
            from x in Enumerable.Range(0, input[0].Length)
            select new KeyValuePair<Complex, char>(Complex.ImaginaryOne * y + x, input[y][x])
        ).ToDictionary();

        Complex[] directions = [-1, 1, Complex.ImaginaryOne, -Complex.ImaginaryOne];

        var start = map.Keys.Single(k => map[k] == 'S');
        var goal = map.Keys.Single(k => map[k] == 'E');

        var (previous, current) = ((Complex?)null, goal);
        var result = new List<Complex> { current };

        while (current != start)
        {
            var direction = directions.Single(d => map[current + d] != '#' && current + d != previous);
            (previous, current) = (current, current + direction);
            result.Add(current);
        }

        return result.ToArray();
    }
}