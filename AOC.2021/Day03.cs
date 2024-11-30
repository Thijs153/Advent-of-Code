namespace AOC._2021;

public class Day03
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day03.txt");

    [Fact]
    public void Part1()
    {
        (GammaRate(_input) * EpsilonRate(_input))
            .Should().Be(2498354);
    }

    [Fact]
    public void Part2()
    {
        (OxygenGeneratorRating(_input) * Co2ScrubberRating(_input))
            .Should().Be(3277956);
    }
    
    private static int GammaRate(string[] input) =>
        Extract1(input, MostCommonBitAt);
    
    private static int EpsilonRate(string[] input) =>
        Extract1(input, LeastCommonBitAt);
    
    private static int OxygenGeneratorRating(string[] input) =>
        Extract2(input, MostCommonBitAt);
    
    private static int Co2ScrubberRating(string[] input) =>
        Extract2(input, LeastCommonBitAt);
    
    private static char MostCommonBitAt(string[] lines, int iBit) =>
        2 * lines.Count(line => line[iBit] == '1') >= lines.Length ? '1' : '0';
    
    private static char LeastCommonBitAt(string[] lines, int iBit) =>
        MostCommonBitAt(lines, iBit) == '1' ? '0' : '1';
    
    private static int Extract1(string[] lines, Func<string[], int, char> selectBitAt)
    {
        var cBit = lines[0].Length;
        var bits = "";

        for (var i = 0; i < cBit; i++)
        {
            bits += selectBitAt(lines, i);
        }

        return Convert.ToInt32(bits, 2);
    }

    private static int Extract2(string[] lines, Func<string[], int, char> selectBitAt)
    {
        var cBit = lines[0].Length;

        for (var i = 0; lines.Length > 1 && i < cBit; i++)
        {
            var bit = selectBitAt(lines, i);
            lines = lines.Where(line => line[i] == bit).ToArray();
        }

        return Convert.ToInt32(lines[0], 2);
    }
}