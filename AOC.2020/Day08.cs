namespace AOC._2020;

public class Day08
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day08.txt");

    [Fact]
    public void Part1()
    {
        Run(Parse(_input)).acc.Should().Be(1584);
    }

    [Fact]
    public void Part2()
    {
        Patches(Parse(_input))
            .Select(Run)
            .First(res => res.terminated).acc
            .Should().Be(920);
    }


    private static (int acc, bool terminated) Run(Stm[] program)
    {
        var (ip, acc, seen) = (0, 0, new HashSet<int>());

        while (true)
        {
            if (ip >= program.Length)
            {
                return (acc, true);
            }  
            if (!seen.Add(ip))
            {
                return (acc, false);
            }

            var stm = program[ip];
            switch (stm.op)
            {
                case "nop": ip++;
                    break;
                case "acc": ip++;
                    acc += stm.arg;
                    break;
                case "jmp": ip += stm.arg;
                    break;
            }
        }
    }

    private static IEnumerable<Stm[]> Patches(Stm[] program)
    {
        return Enumerable.Range(0, program.Length)
            .Where(line => program[line].op != "acc")
            .Select(lineToPatch =>
                program.Select((stm, line) =>
                    line != lineToPatch ? stm :
                    stm.op == "jmp" ? stm with { op = "nop" } :
                    stm.op == "nop" ? stm with { op = "jmp" } :
                    throw new Exception()
                ).ToArray()
            );
    }

    private static Stm[] Parse(string[] input) =>
        input.Select(line => line.Split(" "))
            .Select(parts => new Stm(parts[0], int.Parse(parts[1])))
            .ToArray();
    
    private record Stm(string op, int arg);
}