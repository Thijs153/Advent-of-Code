namespace AOC._2020;

public class Day03
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day03.txt");

    [Fact]
    public void Part1()
    {
        TreeCount(_input, (1, 3))
            .Should().Be(230);   
    }
    
    [Fact]
    public void Part2()
    {
        TreeCount(_input, (1, 1), (1, 3), (1, 5), (1, 7), (2, 1))
            .Should().Be(9533698720L);
    }
    
    private static long TreeCount(string[] input, params (int drow, int dcol)[] slopes)
    {
        var (crow, ccol) = (input.Length, input[0].Length);
        var mul = 1L;

        foreach (var (drow, dcol) in slopes)
        {
            var (irow, icol) = (drow, dcol);
            var trees = 0;
            while (irow < crow)
            {
                if (input[irow][icol % ccol] == '#')
                {
                    trees++;
                }

                (irow, icol) = (irow + drow, icol + dcol);
            }

            mul *= trees;
        }

        return mul;
    }
}