using System.Numerics;

namespace AOC._2022;

public class Day23
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day23.txt");

    [Fact]
    public void Part1()
    {
        Run(Parse(_input))
            .Select(Area)
            .ElementAt(9) // 10th round
            .Should().Be(3917);
    }

    [Fact]
    public void Part2()
    {
        Run(Parse(_input))
            .Count()
            .Should().Be(988);
    }
    
    
    private static IEnumerable<HashSet<Complex>> Run(HashSet<Complex> elves)
    {
        var lookAround = new Queue<Complex>(new[] { N, S, W, E });

        for (var fixpoint = false; !fixpoint; lookAround.Enqueue(lookAround.Dequeue()))
        {
            // 1) collect proposals; for each position (key) compute the list of elves
            // who want to step there
            var proposals = new Dictionary<Complex, List<Complex>>();

            foreach (var elf in elves)
            {
                var lonely = Directions.All(dir => !elves.Contains(elf + dir));
                if (lonely) continue;

                foreach (var dir in lookAround)
                {
                    // elf proposes a position if nobody stands in that directions
                    var proposes = ExtendDir(dir).All(d => !elves.Contains(elf + d));
                    if (proposes)
                    {
                        var pos = elf + dir;
                        if (!proposals.ContainsKey(pos))
                        {
                            proposals[pos] = new List<Complex>();
                        }

                        proposals[pos].Add(elf);
                        break;
                    }
                }
            }
            
            // 2) move elves, compute fixpoint (where no elf moves)
            fixpoint = true;
            foreach (var (to, from) in proposals)
            {
                // Only 1 elf proposed to move here, so we can move :)
                if (from.Count == 1)
                {
                    elves.Remove(from.Single());
                    elves.Add(to);
                    fixpoint = false;
                }
            }

            yield return elves;
        }
    }

    private static double Area(HashSet<Complex> elves)
    {
        // smallest enclosing rectangle
        var width = elves.Select(p => p.Real).Max() -
            elves.Select(p => p.Real).Min() + 1;

        var height = elves.Select(p => p.Imaginary).Max() -
            elves.Select(p => p.Imaginary).Min() + 1;

        return width * height - elves.Count;
    }
    
    private static HashSet<Complex> Parse(string[] input)
    {
        return (
            from iRow in Enumerable.Range(0, input.Length)
            from iCol in Enumerable.Range(0, input[0].Length)
            where input[iRow][iCol] == '#'
            select new Complex(iCol, iRow)
        ).ToHashSet();
    }

    private static readonly Complex N = new(0, -1);
    private static readonly Complex E = new(1, 0);
    private static readonly Complex S = new(0, 1);
    private static readonly Complex W = new(-1, 0);
    private static readonly Complex NW = N + W;
    private static readonly Complex NE = N + E;
    private static readonly Complex SE = S + E;
    private static readonly Complex SW = S + W;

    private static Complex[] Directions = [NW, N, NE, E, SE, S, SW, W];
    
    // Extends an ordinal position with its intercardinal neighbours
    private static Complex[] ExtendDir(Complex dir) =>
        dir == N ? new[] { NW, N, NE } :
        dir == E ? new[] { NE, E, SE } :
        dir == S ? new[] { SW, S, SE } :
        dir == W ? new[] { NW, W, SW } :
        throw new Exception();
}