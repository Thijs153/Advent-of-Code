using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day11
{
    private List<Monkey> _monkeys = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
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
    }
    
    [Test]
    public void Part1()
    {
        for (int i = 0; i < 20; i++)
        {
            foreach (Monkey monkey in _monkeys)
            {
                while (monkey.Items.Count > 0)
                {
                    long item = monkey.Items.Dequeue();
                    long newWorry = Monkey.ExecuteOperation(item, monkey.Operation.mod, monkey.Operation.value, true);

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

        _monkeys.Select(m => m.Counter)
            .OrderByDescending(c => c)
            .Take(2)
            .Aggregate(1, (a, b) => a * b)
            .Should().Be(10605);
    }
    
   [Test]
    public void Part2()
    {
        for (int i = 0; i < 10000; i++)
        {
            foreach (Monkey monkey in _monkeys)
            {
                while (monkey.Items.Count > 0)
                {
                    long item = monkey.Items.Dequeue();
                    long newWorry = Monkey.ExecuteOperation(item, monkey.Operation.mod, monkey.Operation.value, false);

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

        _monkeys.Select(m => m.Counter)
            .OrderByDescending(c => c)
            .Take(2)
            .Aggregate(1L, (a, b) => a * b)
            .Should().Be(2713310158);
    }
}


internal class Monkey
{
    public Queue<long> Items;
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

    public static long ExecuteOperation(long old, string mod, string value, bool divide)
    {
        long secondValue = value == "old" ? old : long.Parse(value);

        long newWorry = mod == "*" ? (old * secondValue) : (old + secondValue);

        if (divide) return newWorry / 3;
        return newWorry;
    }
}


