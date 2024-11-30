namespace AOC._2022;

public class Day06
{
    private readonly string _input = File.ReadAllText("Inputs/Day06.txt");

    [Fact]
    public void Part1() => Test(4).Should().Be(1142);

    [Fact]
    public void Part2() => Test(14).Should().Be(2803);
    
    private int Test(int n)
    {
        var index = 0;
        for (var i = n; i < _input.Length; i++)
        {
            var sequence = _input[(i - n)..i];

            if (!sequence.Distinct().SequenceEqual(sequence)) continue;

            index = i;
            break;
        }

        return index;
    }
}