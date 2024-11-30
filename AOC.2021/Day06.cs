namespace AOC._2021;

public class Day06
{
    private readonly string _input = File.ReadAllText("Inputs/Day06.txt");

    //       0123456           78 
    //   ┌──[.......]─<─(+)───[..]──┐
    //   └──────>────────┴─────>────┘
    //     reproduction     newborn
    
    [Fact]
    public void Part1()
    {
        FishCountAfterNDays(_input, 80)
            .Should().Be(391888);
    }
    
    [Fact]
    public void Part2()
    {
        FishCountAfterNDays(_input, 256)
            .Should().Be(1754597645339);
    }
    
    private static long FishCountAfterNDays(string input, int N)
    {
        // group the fish by their timer, no need to deal with them one by one:
        var fishCountByInternalTimer = new long[9];
        foreach (var ch in input.Split(','))
        {
            fishCountByInternalTimer[int.Parse(ch)]++;
        }

        for (var n = 0; n < N; n++)
        {
            fishCountByInternalTimer[(n + 7) % 9] += fishCountByInternalTimer[n % 9];
        }

        return fishCountByInternalTimer.Sum();
    }
}