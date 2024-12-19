using System.Text.RegularExpressions;

namespace AOC._2024;

public class Day17
{
    private readonly string _input = File.ReadAllText("Inputs/Day17.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        var (state, program) = Parse(_input);
        string.Join(",", Run(state, program)).Should().Be("1,5,0,3,7,3,0,3,1");
    }

    [Fact]
    public void Part2()
    {
        var (_, program) = Parse(_input);
        GenerateA(program, program).Min().Should().Be(105981155568026L);
    }

    private static List<long> Run(List<long> state, List<long> program)
    {
        var res = new List<long>();
        for (var ip = 0; ip < program.Count; ip += 2)
        {
            switch ((OpCode)program[ip], (int)program[ip + 1])
            {
                case (OpCode.Adv, var op): state[0] >>= (int)Combo(op); break;
                case (OpCode.Bdv, var op): state[1] = state[0] >> (int)Combo(op); break;
                case (OpCode.Cdv, var op): state[2] = state[0] >> (int)Combo(op); break;
                case (OpCode.Bxl, var op): state[1] ^= op; break;
                case (OpCode.Bst, var op): state[1] = Combo(op) % 8; break;
                case (OpCode.Jnz, var op): ip = state[0] == 0 ? ip : op - 2; break;
                case (OpCode.Bxc, _): state[1] ^= state[2]; break;
                case (OpCode.Out, var op): res.Add(Combo(op) % 8); break;
            }
        }

        return res;
        long Combo(int op) => op < 4 ? op : state[op - 4];
    }
    
    /*
     * Determines register A for the given output. The search works recursively and in
     * reverse order, starting from the last number to be printed and ending with the first.
     */
    private static IEnumerable<long> GenerateA(List<long> program, List<long> output)
    {
        if (!output.Any())
        {
            yield return 0;
            yield break;
        }

        foreach (var ah in GenerateA(program, output[1..]))
        {
            for (var al = 0; al < 8; al++)
            {
                var a = ah * 8 + al;
                if (Run([a, 0, 0], program).SequenceEqual(output))
                {
                    yield return a;
                }
            }
        }
    }
    
    private static (List<long> state, List<long> program) Parse(string input)
    {
        var blocks = input.Split("\n\n").Select(ParseNums).ToArray();
        return (blocks[0], blocks[1]);
    }
    
    private static List<long> ParseNums(string st) =>
        Regex.Matches(st, @"\d+", RegexOptions.Multiline)
            .Select(m => long.Parse(m.Value))
            .ToList();
    
    private enum OpCode { Adv, Bxl, Bst, Jnz, Bxc, Out, Bdv, Cdv }
}