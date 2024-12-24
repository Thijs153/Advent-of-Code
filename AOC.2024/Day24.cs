using System.Text.RegularExpressions;

namespace AOC._2024;

using Circuit = Dictionary<string, Day24.Gate>;

public class Day24
{
    private readonly string _input = File.ReadAllText("Inputs/Day24.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        var (inputs, circuit) = Parse(_input);
        var outputs = from label in circuit.Keys where label.StartsWith("z") select label;

        var result = 0L;
        foreach (var label in outputs.OrderByDescending(label => label))
        {
            result = result * 2 + Evaluate(label, circuit, inputs);
        }

        result.Should().Be(56729630917616L);
    }

    [Fact]
    public void Part2()
    {
        var circuit = Parse(_input).circuit;
        var result = string.Join(",", Fix(circuit).OrderBy(label => label));
        
        result.Should().Be("bjm,hsw,nvr,skf,wkr,z07,z13,z18");
    }

    private static int Evaluate(string label, Circuit circuit, Dictionary<string, int> inputs)
    {
        if (inputs.TryGetValue(label, out var result))
        {
            return result;
        }

        return circuit[label] switch
        {
            (var in1, "AND", var in2) => Evaluate(in1, circuit, inputs) & Evaluate(in2, circuit, inputs),
            (var in1, "OR", var in2) => Evaluate(in1, circuit, inputs) | Evaluate(in2, circuit, inputs),
            (var in1, "XOR", var in2) => Evaluate(in1, circuit, inputs) ^ Evaluate(in2, circuit, inputs),
            _ => throw new Exception(circuit[label].ToString())
        };
    }
    
    // The circuit should define a full adder for two 44 bit numbers
    private static IEnumerable<string> Fix(Circuit circuit)
    {
        var cin = Output(circuit, "x00", "AND", "y00");
        for (var i = 1; i < 45; i++)
        {
            var x = $"x{i:D2}";
            var y = $"y{i:D2}";
            var z = $"z{i:D2}";
            
            var xor1 = Output(circuit, x, "XOR", y);
            var and1 = Output(circuit, x, "AND", y);
            var xor2 = Output(circuit, cin, "XOR", xor1);
            var and2 = Output(circuit, cin, "AND", xor1);

            if (xor2 == null && and2 == null)
            {
                return SwapAndFix(circuit, xor1, and1);
            }

            var carry = Output(circuit, and1, "OR", and2);
            if (xor2 != z)
            {
                return SwapAndFix(circuit, z, xor2);
            }

            cin = carry;

        }
        
        return [];
    }
    
    private static IEnumerable<string> SwapAndFix(Circuit circuit, string out1, string out2)
    {
        (circuit[out1], circuit[out2]) = (circuit[out2], circuit[out1]);
        return Fix(circuit).Concat([out1, out2]);
    }
    
    private static string Output(Circuit circuit, string x, string kind, string y) =>
        circuit.SingleOrDefault(pair =>
            (pair.Value.in1 == x && pair.Value.kind == kind && pair.Value.in2 == y) ||
            (pair.Value.in1 == y && pair.Value.kind == kind && pair.Value.in2 == x)
        ).Key;

    private static (Dictionary<string, int> inputs, Circuit circuit) Parse(string input)
    {
        var blocks = input.Split("\n\n");

        var inputs = blocks[0]
            .Split("\n")
            .Select(line => line.Split(": "))
            .ToDictionary(parts => parts[0], parts => int.Parse(parts[1]));

        var circuit = blocks[1]
            .Split("\n")
            .Select(line => Regex.Matches(line, "[a-zA-z0-9]+").Select(m => m.Value).ToArray())
            .ToDictionary(parts => parts[3], parts => new Gate(in1: parts[0], kind: parts[1], in2: parts[2]));

        return (inputs, circuit);
    }
    
    public record struct Gate(string in1, string kind, string in2);
}