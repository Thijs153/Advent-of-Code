namespace AOC._2020;

public class Day09
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day09.txt");

    [Fact]
    public void Part1()
    {
        FirstMisMatch(_input).Should().Be(105950735);
    }

    [Fact]
    public void Part2()
    {
        EncryptionWeakness(_input).Should().Be(13826915);
    }

    private static long FirstMisMatch(string[] input)
    {
        var numbers = input.Select(long.Parse).ToArray();

        bool MisMatch(int i) => (
            from j in Range(i - 25, i)
            from k in Range(j + 1, i)
            select numbers[j] + numbers[k]
        ).All(sum => sum != numbers[i]);

        return numbers[Range(25, input.Length).First(MisMatch)];
    }

    private static long EncryptionWeakness(string[] input)
    {
        var d = FirstMisMatch(input);
        var lines = input.Select(long.Parse).ToList();

        foreach (var j in Range(0, lines.Count))
        {
            var s = lines[j];
            foreach (var k in Range(j + 1, lines.Count))
            {
                s += lines[k];
                if (s > d)
                {
                    break;
                }

                if (s != d) continue;
                
                var range = lines.GetRange(j, k - j + 1);
                return range.Min() + range.Max();
            }
        }

        throw new Exception();
    }
    
    private static IEnumerable<int> Range(int min, int lim) => Enumerable.Range(min, lim - min);
}