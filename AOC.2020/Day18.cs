namespace AOC._2020;

public class Day18
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day18.txt");

    [Fact]
    public void Part1() => Solve(_input, part1: true).Should().Be(4297397455886L);

    [Fact]
    public void Part2() => Solve(_input, part1: false).Should().Be(93000656194428L);
    
    private static long Solve(string[] input, bool part1)
    {
        var sum = 0L;
        foreach (var line in input)
        {
            // https://en.wikipedia.org/wiki/Shunting-yard_algorithm

            var opStack = new Stack<char>();
            var valStack = new Stack<long>();

            void evalUntil(string ops)
            {
                while (!ops.Contains(opStack.Peek()))
                {
                    if (opStack.Pop() == '+')
                    {
                        valStack.Push(valStack.Pop() + valStack.Pop());
                    }
                    else
                    {
                        valStack.Push(valStack.Pop() * valStack.Pop());
                    }
                }
            }
            
            opStack.Push('(');

            // 1 + (2 * 3) + (4 * (5 + 6))
            foreach (var ch in line)
            {
                switch (ch)
                {
                    case ' ':
                        break;
                    case '*':
                        evalUntil("(");
                        opStack.Push('*');
                        break;
                    case '+':
                        evalUntil(part1 ? "(" : "(*");
                        opStack.Push('+');
                        break;
                    case '(': 
                        opStack.Push('(');
                        break;
                    case ')':
                        evalUntil("(");
                        opStack.Pop();
                        break;
                    default:
                        valStack.Push(long.Parse(ch.ToString()));
                        break;
                }
            }
            
            evalUntil("(");
            sum += valStack.Single();
        }

        return sum;
    }
}