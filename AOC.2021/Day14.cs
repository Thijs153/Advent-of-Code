namespace AOC._2021;

public class Day14
{
    private readonly string _input = File.ReadAllText("Inputs/Day14.txt");

    [Fact]
    public void Part1()
    {
        Solve(_input, 10)
            .Should().Be(3408);
    }

    [Fact]
    public void Part2()
    {
        Solve(_input, 40)
            .Should().Be(3724343376942);
    }
    
    private static long Solve(string input, int steps)
    {
        input = input.Replace("\r", "");
        var blocks = input.Split("\n\n");

        var polymer = blocks[0];

        var generatedElement = (
            from line in blocks[1].Split("\n")
            let parts = line.Split(" -> ")
            select (molecule: parts[0], element: parts[1])
        ).ToDictionary(p => p.molecule, p => p.element);
        
        // Cut the polymer into molecules first:
        var moleculeCount = new Dictionary<string, long>();
        foreach (var i in Enumerable.Range(0, polymer.Length - 1))
        {
            var ab = polymer.Substring(i, 2);
            moleculeCount[ab] = moleculeCount.GetValueOrDefault(ab) + 1;
        }
        
        // Update the map in a loop:
        for (var i = 0; i < steps; i++)
        {
            var updated = new Dictionary<string, long>();
            foreach (var (molecule, count) in moleculeCount)
            {
                var (a, n, b) = (molecule[0], generatedElement[molecule], molecule[1]);
                updated[$"{a}{n}"] = updated.GetValueOrDefault($"{a}{n}") + count;
                updated[$"{n}{b}"] = updated.GetValueOrDefault($"{n}{b}") + count;
            }

            moleculeCount = updated;
        }
        
        // To count the elements consider just one end of each molecule:
        var elementCounts = new Dictionary<char, long>();
        foreach (var (molecule, count) in moleculeCount)
        {
            var a = molecule[0];
            elementCounts[a] = elementCounts.GetValueOrDefault(a) + count;
        }
        
        // The # of the closing element is off by one:
        elementCounts[polymer.Last()]++;

        return elementCounts.Values.Max() - elementCounts.Values.Min();
    }
}