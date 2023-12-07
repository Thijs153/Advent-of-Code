using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

public class Day07
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day07.txt");

    [Test]
    public void Part1() =>
        Solve(_input, Part1Points).Should().Be(251058093);

    [Test]
    public void Part2() =>
        Solve(_input, Part2Points).Should().Be(249781879);
    
    
    private static BigInteger Part1Points(string hand) =>
        (PatternValue(hand) << 64) + CardValue(hand, "123456789TJQKA");

    private static BigInteger Part2Points(string hand)
    {
        // The most frequent card ignoring J with a special case for "JJJJJ":
        var replacement = (
            from ch in hand
            where ch != 'J'
            group ch by ch into g
            orderby g.Count() descending
            select g.Key
        ).FirstOrDefault('J');

        var cv = CardValue(hand, "J123456789TQKA");
        var pv = PatternValue(hand.Replace('J', replacement));
        return (pv << 64) + cv;
    }
    
    // map cards to their indices in cardOrder. E.g. for 123456789TJQKA
    // A8A8A becomes (13)(7)(13)(7)(13), 9A34Q becomes (8)(13)(2)(3)(11)
    private static BigInteger CardValue(string hand, string cardOrder) =>
        new(hand.Select(ch => (byte)cardOrder.IndexOf(ch)).Reverse().ToArray());
    
    // map cards to the number of their occurrences in the hand then order them such that
    // A8A8A becomes 33322, 9A34Q becomes 11111 and K99AA becomes 22221
    private static BigInteger PatternValue(string hand) =>
        new(hand.Select(ch => (byte)hand.Count(x => x == ch)).Order().ToArray());
    
    private static int Solve(string[] input, Func<string, BigInteger> getPoints)
    {
        var bidsByRanking = (
            from line in input
            let hand = line.Split(" ")[0]
            let bid = int.Parse(line.Split(" ")[1])
            orderby getPoints(hand)
            select bid
        );

        return bidsByRanking.Select((bid, rank) => (rank + 1) * bid).Sum();
    }
}