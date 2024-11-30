namespace AOC._2021;

public class Day08
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day08.txt");

    /*
     *     0:      1:      2:      3:      4:      5:      6:      7:      8:      9:
     *    aaaa    ....    aaaa    aaaa    ....    aaaa    aaaa    aaaa    aaaa    aaaa
     *   b    c  .    c  .    c  .    c  b    c  b    .  b    .  .    c  b    c  b    c
     *   b    c  .    c  .    c  .    c  b    c  b    .  b    .  .    c  b    c  b    c
     *    ....    ....    dddd    dddd    dddd    dddd    dddd    ....    dddd    dddd
     *   e    f  .    f  e    .  .    f  .    f  .    f  e    f  .    f  e    f  .    f
     *   e    f  .    f  e    .  .    f  .    f  .    f  e    f  .    f  e    f  .    f
     *    gggg    ....    gggg    gggg    ....    gggg    gggg    ....    gggg    gggg
     */
    
    [Fact]
    public void Part1()
    {
        // we can identify digits 1, 7, 4 and 8 be their active active segments count:
        var segmentCounts = new[] { "cd", "acf", "bcdf", "abcdefg" }
            .Select(x => x.Length)
            .ToHashSet();

        (
            from line in _input
            let parts = line.Split(" | ")
            from segment in parts[1].Split(" ")
            where segmentCounts.Contains(segment.Length)
            select 1
        ).Count()
            .Should().Be(470);
    }

    [Fact]
    public void Part2()
    {
        var res = 0;
        foreach (var line in _input)
        {
            var parts = line.Split(" | ");
            var patterns = parts[0].Split(" ")
                .Select(x => x.ToHashSet()).ToArray();
            
            // let's figure out what segments belong to each digit
            var digits = new HashSet<char>[10];
            
            // we can do these by length:
            digits[1] = patterns.Single(p => p.Count == "cf".Length);
            digits[4] = patterns.Single(p => p.Count == "bcdf".Length);

            HashSet<char> Lookup(int segmentCount, int commonWithOne, int commonWithFour) => 
                patterns.Single(p => 
                    p.Count == segmentCount && 
                    p.Intersect(digits[1]).Count() == commonWithOne && 
                    p.Intersect(digits[4]).Count() == commonWithFour
                );

            digits[0] = Lookup(6, 2, 3);
            digits[2] = Lookup(5, 1, 2);
            digits[3] = Lookup(5, 2, 3);
            digits[5] = Lookup(5, 1, 3);
            digits[6] = Lookup(6, 1, 3);
            digits[7] = Lookup(3, 2, 2);
            digits[8] = Lookup(7, 2, 4);
            digits[9] = Lookup(6, 2, 4);

            int Decode(string v) => 
                Enumerable.Range(0, 10).Single(i => digits[i].SetEquals(v));

            res += parts[1].Split(" ").Aggregate(0, (n, digit) => n * 10 + Decode(digit));
        }

        res.Should().Be(989396);
    }
}