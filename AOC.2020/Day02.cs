using FluentAssertions;
using NUnit.Framework;

namespace AOC._2020;

[TestFixture]
public class Day02
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day02.txt");

    [Test]
    public void Part1()
    {
        ValidCount(_input, (pe) =>
        {
            var count = pe.password.Count(ch => ch == pe.ch);
            return pe.a <= count && count <= pe.b;
        }).Should().Be(614);
    }
    
    [Test]
    public void Part2()
    {
        ValidCount(_input, (pe) => (pe.password[pe.a - 1] == pe.ch) ^ (pe.password[pe.b - 1] == pe.ch))
            .Should().Be(354);
    }

    private static int ValidCount(string[] input, Func<PasswordEntry, bool> isValid) =>
        input.Select(line =>
        {
            var parts = line.Split(' ');
            var range = parts[0].Split('-').Select(int.Parse).ToArray();
            var ch = parts[1][0];
            return new PasswordEntry(range[0], range[1], ch, parts[2]);
        }).Count(isValid);

    private record PasswordEntry(int a, int b, char ch, string password);
}