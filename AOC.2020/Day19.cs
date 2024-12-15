namespace AOC._2020;

public class Day19
{
    private readonly string _input = File.ReadAllText("Inputs/Day19.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1() => Solve(_input, part1: true).Should().Be(120);

    [Fact]
    public void Part2() => Solve(_input, part1: false).Should().Be(350);
    
    private static int Solve(string input, bool part1)
    {
        var blocks = (
            from block in input.Split("\n\n")
            select block.Split("\n")
        ).ToArray();

        var rules = (
            from line in blocks[0]
            let parts = line.Split(": ")
            let index = int.Parse(parts[0])
            let rule = parts[1]
            select new KeyValuePair<int, string>(index, rule)
        ).ToDictionary();

        if (!part1)
        {
            rules[8] = "42 | 42 8";
            rules[11] = "42 31 | 42 11 31";
        }
        
        // a parser will process some prefix of the input and return the possible remainders (nothing in case of error)
        var parsers = new Dictionary<int, Parser>();

        var parser = GetParser(0);
        return blocks[1].Count(data => parser(data).Any(st => st == ""));

        Parser GetParser(int index)
        {
            if (parsers.TryGetValue(index, out var parser1))
            {
                return parser1;
            }
            
            parsers[index] = i => GetParser(index)(i);
            parsers[index] = Alt(
                (
                    from sequence in rules[index].Split(" | ")
                    select Seq(
                        (
                            from item in sequence.Split(" ")
                            select int.TryParse(item, out var i) ? GetParser(i) : Literal(item.Trim('"'))
                        ).ToList()
                    )
                ).ToList()
            );

            return parsers[index];
        }
    }
    
    // Parser combinators
    private static Parser Literal(string st) =>
        input => input.StartsWith(st) ? [input[st.Length..]] : [];

    private static Parser Seq(List<Parser> parsers)
    {
        if (parsers.Count == 1)
        {
            return parsers.Single();
        }

        var parseHead = parsers.First();
        var parseTail = Seq(parsers.Skip(1).ToList());

        return input => (
            from tail in parseHead(input)
            from rest in parseTail(tail)
            select rest
        ).ToList();
    }

    private static Parser Alt(List<Parser> parsers)
    {
        if (parsers.Count == 1)
        {
            return parsers.Single();
        }

        var arr = parsers.ToArray();
        return input => (
            from parser in arr
            from rest in parser(input)
            select rest
        ).ToList();
    }
    
    private delegate List<string> Parser(string st);
}