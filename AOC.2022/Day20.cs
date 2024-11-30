namespace AOC._2022;

public class Day20
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day20.txt");

    [Fact]
    public void Part1()
    {
        GetGrooveCoordinates(Mix(Parse(_input, 1)))
            .Should().Be(15297L);
    }

    [Fact]
    public void Part2()
    {
        var data = Parse(_input, 811589153L);
        for (var i = 0; i < 10; i++)
        {
            data = Mix(data);
        }

        GetGrooveCoordinates(data)
            .Should().Be(2897373276210L);
    }

    private static List<Data> Parse(string[] input, long m) =>
        input.Select((line, index) => new Data(index, long.Parse(line) * m))
            .ToList();

    private static List<Data> Mix(List<Data> numsWithIndex)
    {
        var mod = numsWithIndex.Count - 1;
        for (var index = 0; index < numsWithIndex.Count; index++)
        {
            var srcIndex = numsWithIndex.FindIndex(x => x.Index == index);
            var num = numsWithIndex[srcIndex];

            var dstIndex = (srcIndex + num.Num) % mod;
            if (dstIndex < 0)
                dstIndex += mod;

            numsWithIndex.RemoveAt(srcIndex);
            numsWithIndex.Insert((int)dstIndex, num);
        }

        return numsWithIndex;
    }
    
    private static long GetGrooveCoordinates(List<Data> numsWithIndex)
    {
        var index = numsWithIndex.FindIndex(x => x.Num == 0);
        return (
            numsWithIndex[(index + 1000) % numsWithIndex.Count].Num +
            numsWithIndex[(index + 2000) % numsWithIndex.Count].Num +
            numsWithIndex[(index + 3000) % numsWithIndex.Count].Num
        );
    }

    private record Data(int Index, long Num);
}