using System.Text.RegularExpressions;

namespace AOC._2022;

public class Day11
{
    private readonly List<Monkey> _monkeys;
    private readonly int _mod;

    public Day11()
    {
        List<List<string>> monkeyGroups = new();

        List<string> monkeyText = new();
        List<string> lines = File.ReadLines("Inputs/Day11.txt").ToList();
        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i];
            
            if (line != "") monkeyText.Add(line);
            
            if (line == "" || i == lines.Count - 1)
            {
                monkeyGroups.Add(monkeyText);
                monkeyText = new List<string>();
            }
        }

        _monkeys = new List<Monkey>();
        foreach (List<string> monkeyText2 in monkeyGroups)
        {
            var arr = monkeyText2[1].Split(':')[1].Split(',').Select(x => x.Trim()).Select(long.Parse);
            Queue<long> monkeyItems = new Queue<long>(arr);
            
            var operation = Regex.Match(monkeyText2[2], @"  Operation: new = old ([\*|\+]) (.+)");
            string operationChar = operation.Groups[1].Value;
            string operationValue = operation.Groups[2].Value;

            int test = int.Parse(monkeyText2[3].Split(" ").Last());
            int trueMonkey = int.Parse(monkeyText2[4].Split(" ").Last());
            int falseMonkey = int.Parse(monkeyText2[5].Split(" ").Last());

            _monkeys.Add(new Monkey(monkeyItems, (operationChar, operationValue), (test, trueMonkey, falseMonkey)));
        }
        
        _mod = _monkeys.Aggregate(1, (mod, monkey) => mod * monkey.Test.test);
    }
    

    [Fact]
    public void Part1()
    {
        Run(20, true).Should().Be(110888);
    }
    
   [Fact]
    public void Part2()
    {
        Run(10000, false).Should().Be(25590400731);
    }

    private long Run(int rounds, bool shouldDivideBy3)
    {
        for (int i = 0; i < rounds; i++)
        {
            foreach (Monkey monkey in _monkeys)
            {
                while (monkey.Items.Count > 0)
                {
                    long item = monkey.Items.Dequeue();
                    long newWorry = ExecuteOperation(item, monkey.Operation.mod, monkey.Operation.value, shouldDivideBy3);

                    if (newWorry % monkey.Test.test == 0)
                    {
                        _monkeys[monkey.Test.trueMonkey].Items.Enqueue(newWorry);
                    }
                    else
                    {
                        _monkeys[monkey.Test.falseMonkey].Items.Enqueue(newWorry);
                    }

                    monkey.Counter += 1;
                }
            }    
        }

        return _monkeys.Select(m => m.Counter)
            .OrderByDescending(c => c)
            .Take(2)
            .Aggregate(1L, (a, b) => a * b);
    }
    
    private long ExecuteOperation(long old, string mod, string value, bool divide)
    {
        long secondValue = value == "old" ? old : long.Parse(value);

        long newWorry = mod == "*" ? (old * secondValue) : (old + secondValue);

        if (divide) return newWorry / 3;
        return newWorry % _mod;
    }
}


internal class Monkey
{
    public readonly Queue<long> Items;
    public (string mod, string value) Operation;
    public (int test, int trueMonkey, int falseMonkey) Test;
    public int Counter;

    public Monkey(Queue<long> items, (string, string) operation, (int, int, int) test)
    {
        Items = items;
        Operation = operation;
        Test = test;
        Counter = 0;
    }
}


