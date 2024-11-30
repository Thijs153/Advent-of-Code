using System.Text.RegularExpressions;
using Gates = System.Collections.Generic.Dictionary<string, AOC._2023.Gate>;
using Signal = (string sender, string receiver, bool value);

namespace AOC._2023;


public record Gate(string[] inputs, Func<Signal, IEnumerable<Signal>> handle);

public class Day20
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day20.txt");

    [Fact]
    public void Part1()
    {
        var gates = ParseGates(_input);
        var values = (
            from _ in Enumerable.Range(0, 1000)
            from signal in Trigger(gates)
            select signal.value
        ).ToArray();

        (values.Count(v => v) * values.Count(v => !v)).Should().Be(666795063);
    }

    [Fact]
    public void Part2()
    {
        var gates = ParseGates(_input);
        var nand = gates["rx"].inputs.Single();
        var branches = gates[nand].inputs;

        branches.Aggregate(1L, (m, branch) => m * LoopLength(_input, branch))
            .Should().Be(253302889093151);
    }

    private static int LoopLength(string[] input, string output)
    {
        var gates = ParseGates(input);
        for (var i = 1;; i++)
        {
            var signals = Trigger(gates);
            if (signals.Any(s => s.sender == output && s.value))
            {
                return i;
            }
        }
    }

    // emits a button press, executes until things settle down and returns
    // all signals for investigation
    private static IEnumerable<Signal> Trigger(Gates gates)
    {
        var q = new Queue<Signal>();
        q.Enqueue(new Signal("button", "broadcaster", false));
        while (q.TryDequeue(out var signal))
        {
            yield return signal;
            var receiver = gates[signal.receiver];
            foreach (var signalT in receiver.handle(signal))
            {
                q.Enqueue(signalT);
            }
        }
    }

    private static Gates ParseGates(string[] input)
    {
        var descriptions = (
            from line in input
            let kind = char.IsLetter(line[0]) ? "" : line[..1]
            let parts = from m in Regex.Matches(line, "[a-z]+") select m.Value
            select (kind, name: parts.First(), outputs: parts.Skip(1).ToArray())
        ).ToList();

        descriptions.Add(("", "button", ["broadcaster"]));
        descriptions.Add(("", "rx", []));

        var gates = new Gates();
        foreach (var descr in descriptions)
        {
            var inputs = (
                from decrT in descriptions
                where decrT.outputs.Contains(descr.name)
                select decrT.name
            ).ToArray();

            gates[descr.name] = descr.kind switch
            {
                "&" => NandGate(descr.name, inputs, descr.outputs),
                "%" => FlipFlop(descr.name, inputs, descr.outputs),
                _ => Repeater(descr.name, inputs, descr.outputs)
            };
        }

        return gates;
    }
    
    private static Gate NandGate(string name, string[] inputs, string[] outputs)
    {
        // initially assign low value for each input:
        var state = inputs.ToDictionary(input => input, _ => false);

        return new Gate(inputs, (s) =>
        {
            state[s.sender] = s.value;
            var value = !state.Values.All(b => b);
            return outputs.Select(o => new Signal(name, o, value));
        });
    }
    
    private static Gate FlipFlop(string name, string[] inputs, string[] outputs)
    {
        var state = false;

        return new Gate(inputs, (s) =>
        {
            if (!s.value)
            {
                state = !state;
                return outputs.Select(o => new Signal(name, o, state));
            }

            return [];
        });
    }
    
    private static Gate Repeater(string name, string[] inputs, string[] outputs) =>
        new(inputs, (s) => 
            from o in outputs select new Signal(name, o, s.value)
        );
}