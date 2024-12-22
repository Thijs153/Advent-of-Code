namespace AOC._2024;

public class Day22
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day22.txt");

    [Fact]
    public void Part1()
    {
        GetNumbers(_input)
            .Select(x => (long)SecretNumbers(x).Last())
            .Sum()
            .Should().Be(15335183969L);
    }

    [Fact]
    public void Part2()
    {
        // Create a dictionary of all buying options then select the one with the most banana:
        var buyingOptions = new Dictionary<string, int>();
        foreach (var number in GetNumbers(_input))
        {
            var optionsBySeller = BuyingOptions(number);
            foreach (var seq in optionsBySeller.Keys)
            {
                buyingOptions[seq] = buyingOptions.GetValueOrDefault(seq) + optionsBySeller[seq];
            }
        }

        buyingOptions.Values.Max().Should().Be(1696);
    }

    private static Dictionary<string, int> BuyingOptions(int seed)
    {
        var bananasSold = Bananas(seed).ToArray();
        var buyOptions = new Dictionary<string, int>();
        
        // a sliding window of 5 elements over the sold bananas defines the sequence the monkey
        // will recognize. add the first occurrence of each sequence to the buyOptions dictionary
        // with the corresponding banana count.
        for (var i = 5; i < bananasSold.Length; i++)
        {
            var slice = bananasSold[(i - 5) .. i];
            var seq = string.Join(",", Diff(slice));
            if (!buyOptions.ContainsKey(seq))
            {
                buyOptions[seq] = slice.Last();
            }
        }

        return buyOptions;
    }
    
    private static int[] Bananas(int seed) => SecretNumbers(seed).Select(n => n % 10).ToArray();

    private static int[] Diff(int[] source) => source
        .Zip(source.Skip(1))
        .Select(p => p.Second - p.First)
        .ToArray();
    
    private static IEnumerable<int> SecretNumbers(int seed)
    {
        yield return seed;
        for (var i = 0; i < 2000; i++)
        {
            seed = MixAndPrune(seed, seed * 64L);
            seed = MixAndPrune(seed, seed / 32L);
            seed = MixAndPrune(seed, seed * 2048L);
            yield return seed;
        }
        
        int MixAndPrune(int a, long b) => (int)((a ^ b) % 16777216);
    }
    
    private static IEnumerable<int> GetNumbers(string[] input) => input.Select(int.Parse);
}