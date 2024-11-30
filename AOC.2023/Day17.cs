using System.Numerics;
using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, int>;

namespace AOC._2023;

public class Day17
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day17.txt");

    [Fact]
    public void Part1() =>
        HeatLoss(
            _input,
            new Rules(
                canChangeDir: _ => true,
                canGoStraight: crucible => crucible.straightMoves < 3
            )
        ).Should().Be(1099);
    
    [Fact]
    public void Part2() =>
        HeatLoss(
            _input,
            new Rules(
                canChangeDir: crucible => crucible.straightMoves >= 4,
                canGoStraight: crucible => crucible.straightMoves < 10
            )
        ).Should().Be(1266);
    
    // Graph search using a priority queue.
    private static int HeatLoss(string[] input, Rules rules)
    {
        var map = ParseMap(input);
        var goal = map.Keys.MaxBy(pos => pos.Imaginary + pos.Real);

        var q = new PriorityQueue<Crucible, int>();
        
        // initial direction: right or down
        q.Enqueue(new Crucible(pos: 0, dir: 1, straightMoves: 0), 0);
        q.Enqueue(new Crucible(pos: 0, dir: Complex.ImaginaryOne, straightMoves: 0), 0);

        var seen = new HashSet<Crucible>();
        while (q.TryDequeue(out var crucible, out var heatLoss))
        {
            if (crucible.pos == goal && rules.canChangeDir(crucible))
            {
                return heatLoss;
            }

            foreach (var next in Moves(crucible, rules))
            {
                if (map.TryGetValue(next.pos, out var value) && seen.Add(next))
                {
                    q.Enqueue(next, heatLoss + value);
                }
            }
        }

        return -1; // unreachable
    }
    
    // returns possible next states based on the rules
    private static IEnumerable<Crucible> Moves(Crucible crucible, Rules rules)
    {
        if (rules.canGoStraight(crucible))
        {
            yield return crucible with
            {
                pos = crucible.pos + crucible.dir,
                straightMoves = crucible.straightMoves + 1
            };
        }

        if (rules.canChangeDir(crucible))
        {
            var dir = crucible.dir * Complex.ImaginaryOne;
            yield return new Crucible(crucible.pos + dir, dir, 1);
            yield return new Crucible(crucible.pos - dir, -dir, 1);
        }
    }

    private static Map ParseMap(string[] input) => (
        from iRow in Enumerable.Range(0, input.Length)
        from iCol in Enumerable.Range(0, input[0].Length)
        let cell = int.Parse(input[iRow].Substring(iCol, 1))
        let pos = new Complex(iCol, iRow)
        select new KeyValuePair<Complex, int>(pos, cell)
    ).ToDictionary();
    
    private record Crucible(Complex pos, Complex dir, int straightMoves);

    private record Rules(
        Func<Crucible, bool> canChangeDir,
        Func<Crucible, bool> canGoStraight
    );
}