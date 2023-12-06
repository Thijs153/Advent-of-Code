using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

public class Day05
{
    private readonly string _input = File.ReadAllText("Inputs/Day05.txt").ReplaceLineEndings("\n");

    [Test]
    public void Part1() =>
        Solve(_input, PartOneRanges).Should().Be(993500720);

    [Test]
    public void Part2() =>
        Solve(_input, PartTwoRanges).Should().Be(4917124);

    private static long Solve(string input, Func<long[], IEnumerable<Range>> parseSeeds)
    {
        var blocks = input.Split("\n\n");
        var seedRanges = parseSeeds(ParseNumbers(blocks[0])).ToArray();
        var maps = blocks.Skip(1).Select(ParseMap).ToArray();

        // Project each range through the series of maps, this will result some
        // new ranges. Return the leftmost value (minimum) of these.
        return maps.Aggregate(seedRanges, Project).Select(r => r.From).Min();
    }

    private static Range[] Project(Range[] inputRanges, Dictionary<Range, Range> map)
    {
        var todo = new Queue<Range>();
        foreach (var range in inputRanges)
        {
            todo.Enqueue(range);
        }

        var outputRanges = new List<Range>();
        while (todo.Count > 0)
        {
            var range = todo.Dequeue();
            var src = map.Keys.FirstOrDefault(src => Intersects(src, range));

            if (src == null)
            {
                outputRanges.Add(range);
            }
            else if (src.From <= range.From && range.To <= src.To)
            {
                var dst = map[src];
                var shift = dst.From - src.From;
                outputRanges.Add(new Range(range.From + shift, range.To + shift));
            }
            else if (range.From < src.From)
            {
                todo.Enqueue(new Range(range.From, src.From - 1));
                todo.Enqueue(new Range(src.From, range.To));
            }
            else
            {
                todo.Enqueue(new Range(range.From, src.To));
                todo.Enqueue(new Range(src.To + 1, range.To));
            }
        }

        return [..outputRanges];
    }

    private static bool Intersects(Range r1, Range r2) => r1.From <= r2.To && r2.From <= r1.To;
    
    private static IEnumerable<Range> PartOneRanges(long[] numbers) =>
        from n in numbers select new Range(n, n);
    
    private static IEnumerable<Range> PartTwoRanges(long[] numbers) =>
        from c in numbers.Chunk(2) select new Range(c[0], c[0] + c[1] - 1);

    private static long[] ParseNumbers(string input) =>
        [..from m in Regex.Matches(input, @"\d+") select long.Parse(m.Value)];

    private static Dictionary<Range, Range> ParseMap(string input) => (
        from line in input.Split("\n").Skip(1)
        let parts = line.Split(" ").Select(long.Parse).ToArray()
        let src = new Range(parts[1], parts[2] + parts[1] - 1)
        let dst = new Range(parts[0], parts[2] + parts[0] - 1)
        select new KeyValuePair<Range, Range>(src, dst)
    ).ToDictionary();

    private record Range(long From, long To);
}