using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day10
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day10.txt");

    [Test]
    public void Part1()
    {
        GetScores(_input, true).Sum()
            .Should().Be(323691);
    }

    [Test]
    public void Part2()
    {
        Median(GetScores(_input, false))
            .Should().Be(2858785164);
    }
    
    private static long Median(IReadOnlyCollection<long> items) =>
        items.OrderBy(x => x).ElementAt(items.Count / 2);
    
    private static long[] GetScores(IEnumerable<string> input, bool getSyntaxErrorScore) =>
        input.Select(line => GetScore(line, getSyntaxErrorScore))
            .Where(score => score > 0).ToArray();

    private static long GetScore(string line, bool getSyntaxErrorScore)
    {
        var stack = new Stack<char>();

        foreach (var ch in line)
        {
            switch ((stack.FirstOrDefault(), ch))
            {
                // matching closing parenthesis:
                case ('(', ')'): stack.Pop(); break;
                case ('[', ']'): stack.Pop(); break;
                case ('{', '}'): stack.Pop(); break;
                case ('<', '>'): stack.Pop(); break;
                // return early if syntax error found:
                case (_, ')'): return getSyntaxErrorScore ? 3     : 0;
                case (_, ']'): return getSyntaxErrorScore ? 57    : 0;
                case (_, '}'): return getSyntaxErrorScore ? 1197  : 0;
                case (_, '>'): return getSyntaxErrorScore ? 25137 : 0;
                // otherwise, it's an opening parenthesis:
                case (_, _): stack.Push(ch); break;
            }
        }

        if (getSyntaxErrorScore)
            return 0;
        
        return stack
            .Select(item => 1 + "([{<".IndexOf(item)) // convert chars to digits
            .Aggregate(0L, (acc, item) => acc * 5 + item); // get base 5 number
    }
}