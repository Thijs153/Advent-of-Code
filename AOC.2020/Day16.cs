using System.Text.RegularExpressions;

namespace AOC._2020;

public class Day16
{
    private readonly string _input = File.ReadAllText("Inputs/Day16.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        var problem = Parse(_input);
        (
            from ticket in problem.Tickets
            from value in ticket
            where !FieldCandidates(problem.Fields, value).Any()
            select value
        ).Sum().Should().Be(21071);
    }

    [Fact]
    public void Part2()
    {
        var problem = Parse(_input);
        
        // Keep valid tickets only
        var tickets = (
            from ticket in problem.Tickets
            where ticket.All(value => FieldCandidates(problem.Fields, value).Any())
            select ticket
        ).ToArray();

        var fields = problem.Fields.ToHashSet();
        var columns = Enumerable.Range(0, fields.Count).ToHashSet();

        var res = 1L;
        while (columns.Any())
        {
            foreach (var column in columns)
            {
                var valuesInColumn = (from ticket in tickets select ticket[column]).ToArray();
                var candidates = FieldCandidates(fields, valuesInColumn);
                if (candidates.Length == 1)
                {
                    var field = candidates.Single();
                    fields.Remove(field);
                    columns.Remove(column);
                    if (field.Name.StartsWith("departure"))
                    {
                        res *= valuesInColumn.First();
                    }

                    break;
                }
            }
        }

        res.Should().Be(3429967441937L);
    }

    private static Field[] FieldCandidates(IEnumerable<Field> fields, params int[] values) =>
        fields.Where(field => values.All(field.IsValid)).ToArray();

    private static Problem Parse(string input)
    {
        int[] parseNumbers(string line) => (
            from m in Regex.Matches(line, "\\d+") // take the consecutive ranges of digits
            select int.Parse(m.Value)             // convert them to numbers
        ).ToArray();

        var blocks = (
            from block in input.Split("\n\n")
            select block.Split("\n")
        ).ToArray();

        var fields = (
            from line in blocks.First()
            let bounds = parseNumbers(line)
            select new Field(
                line.Split(":")[0],
                n => n >= bounds[0] && n <= bounds[1] ||
                     n >= bounds[2] && n <= bounds[3])
        ).ToArray();

        var tickets = (
            from block in blocks.Skip(1)
            let numbers = block.Skip(1)
            from line in numbers
            select parseNumbers(line)
        ).ToArray();

        return new Problem(fields, tickets);
    }
    
    private record Field(string Name, Func<int, bool> IsValid);

    private record Problem(Field[] Fields, int[][] Tickets);
}