using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

public class Day5
{
    private static List<Stack<char>> _stacks = default!;
    
    private List<(int move, int from, int to)> _moves = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        foreach (string line in File.ReadAllLines("Inputs/Day5.txt").Skip(10))
        {
            string[] splitLine = line.Split(" ");
            _moves.Add((int.Parse(splitLine[1]), int.Parse(splitLine[3]) - 1, int.Parse(splitLine[5]) - 1));
        }

    }

    [SetUp]
    public void Setup()
    {
        _stacks = new List<Stack<char>>()
        {
            StringToStack("TPZCSLQN"),
            StringToStack("LPTVHCG"),
            StringToStack("DCZF"),
            StringToStack("GWTDLMVC"),
            StringToStack("PWC"),
            StringToStack("PFJDCTSZ"),
            StringToStack("VWGBD"),
            StringToStack("NJSQHW"),
            StringToStack("RCQFSLV")
        };
    }

    [Test]
    public void Part1()
    {
        foreach (var move in _moves)
        {
            for (int i = 0; i < move.move; i++)
            {
                _stacks[move.to].Push(_stacks[move.from].Pop());
            }
        }

        string topCrates = _stacks.Aggregate("", (current, stack) => current + stack.Peek());
        topCrates.Should().Be("SVFDLGLWV");
    }
    
    [Test]
    public void Part2()
    {
        foreach (var move in _moves)
        {
            List<char> characters = new();
            for (int i = 0; i < move.move; i++)
            {
                characters.Add(_stacks[move.from].Pop());
            }

            characters.Reverse();
            foreach (var c in characters)
            {
                _stacks[move.to].Push(c);
            }
        }

        string topCrates = _stacks.Aggregate("", (current, stack) => current + stack.Peek());
        topCrates.Should().Be("DCVTCVPCL");
    }

    private static Stack<char> StringToStack(string s)
    {
        Stack<char> stack = new Stack<char>();

        foreach (char c in s)
        {
            stack.Push(c);
        }

        return stack;
    }
}