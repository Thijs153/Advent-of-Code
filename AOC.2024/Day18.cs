using System.Numerics;
using System.Text.RegularExpressions;

namespace AOC._2024;

public class Day18
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day18.txt");

    [Fact]
    public void Part1()
    {
        Distance(GetBlocks(_input).Take(1024))
            .Should().Be(282);
    }

    [Fact]
    public void Part2()
    {
        // find the first block position that will cut off the goal position
        // we can use a binary search for this.

        var blocks = GetBlocks(_input);
        var (low, high) = (0, blocks.Length);
        while (high - low > 1)
        {
            var m = (low + high) / 2;
            if (Distance(blocks.Take(m)) == null)
            {
                high = m;
            }
            else
            {
                low = m;
            }
        }

        $"{blocks[low].Real},{blocks[low].Imaginary}".Should().Be("64,29");
    }

    private static int? Distance(IEnumerable<Complex> blocks)
    {
        // our standard priority queue based path finding.

        var size = 70;
        var (start, goal) = (0, size + size * Complex.ImaginaryOne);
        var blocked = blocks.Concat([start]).ToHashSet();

        var q = new PriorityQueue<Complex, int>();
        q.Enqueue(start, 0);
        while (q.TryDequeue(out var position, out var distance))
        {
            if (position == goal)
            {
                return distance;
            }

            foreach (var direction in new[] { 1, -1, Complex.ImaginaryOne, -Complex.ImaginaryOne})
            {
                var posT = position + direction;
                if (!blocked.Contains(posT) &&
                    0 <= posT.Imaginary && posT.Imaginary <= size &&
                    0 <= posT.Real && posT.Real <= size
                )
                {
                    q.Enqueue(posT, distance + 1);
                    blocked.Add(posT);
                }
            }
        }

        return null;
    }
    
    private static Complex[] GetBlocks(string[] input) => (
        from line in input
        let nums = Regex.Matches(line, @"\d+").Select(m => int.Parse(m.Value)).ToArray()
        select nums[0] + nums[1] * Complex.ImaginaryOne
    ).ToArray();
}