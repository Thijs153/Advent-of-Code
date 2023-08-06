using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2020;

[TestFixture]
public class Day13
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day13.txt");

    [Test]
    public void Part1()
    {
        var problem = Parse(_input);
        problem.buses.Aggregate(
            (wait: long.MaxValue, bus: long.MaxValue),
            (min, bus) =>
            {
                var wait = bus.period - (problem.earliestDepart % bus.period);
                return wait < min.wait ? (wait, bus.period) : min;
            },
            min => min.wait * min.bus
        ).Should().Be(261);
    }

    [Test]
    public void Part2()
    {
        ChineseRemainderTheorem(
            Parse(_input).buses
                .Select(bus => (mod: bus.period, a: bus.period - bus.delay))
                .ToArray()
        ).Should().Be(807435693182510);
    }

    private static (int earliestDepart, (long period, int delay)[] buses) Parse(string[] input)
    {
        var earliestDepartment = int.Parse(input[0]);
        var buses = input[1].Split(",")
            .Select((part, idx) => (part, idx))
            .Where(item => item.part != "x")
            .Select(item => (period: long.Parse(item.part), delay: item.idx))
            .ToArray();
        return (earliestDepartment, buses);
    }

    private static long ChineseRemainderTheorem((long mod, long a)[] items)
    {
        var prod = items.Aggregate(1L, (acc, item) => acc * item.mod);
        var sum = items.Select((item, i) =>
        {
            var p = prod / item.mod;
            return item.a * ModInv(p, item.mod) * p;
        }).Sum();

        return sum % prod;
    }

    private static long ModInv(long a, long m) => (long)BigInteger.ModPow(a, m - 2, m);
}