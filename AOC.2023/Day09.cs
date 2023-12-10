using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

public class Day09
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day09.txt");

    [Test]
    public void Part1() =>
        Solve(_input, ExtrapolateRight).Should().Be(1806615041);

    [Test]
    public void Part2() =>
        Solve(_input, ExtrapolateLeft).Should().Be(1211);

    private static long Solve(string[] input, Func<long[], long> extrapolate) =>
        input.Select(ParseNumbers).Select(extrapolate).Sum();

    private static long[] ParseNumbers(string line) =>
        line.Split(" ").Select(long.Parse).ToArray();
    
    private static long ExtrapolateRight(long[] numbers) =>
        numbers.Length == 0 ? 0 : ExtrapolateRight(Diff(numbers)) + numbers.Last();

    private static long[] Diff(long[] numbers) =>
        numbers.Zip(numbers.Skip(1)).Select(p => p.Second - p.First).ToArray();
    
    private static long ExtrapolateLeft(long[] numbers) =>
        ExtrapolateRight(numbers.Reverse().ToArray());
}