namespace AOC._2021;

public class Day02
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day02.txt");
    
    [Fact]
    public void Part1()
    {
        Parse(_input)
            .Aggregate(
                new State1(0, 0),
                (state, step) => step.dir switch
                {
                    'f' => state with { x = state.x + step.amount },
                    'u' => state with { y = state.y - step.amount },
                    'd' => state with { y = state.y + step.amount },
                    _ => throw new Exception(),
                },
                res => res.x * res.y
            )
            .Should().Be(1654760);
    }

    [Fact]
    public void Part2()
    {
        Parse(_input)
            .Aggregate(
                new State2(0, 0, 0),
                (state, step) => step.dir switch {
                    'f' => state with { 
                        x = state.x + step.amount, 
                        y = state.y + step.amount * state.aim 
                    },
                    'u' => state with { aim = state.aim - step.amount },
                    'd' => state with { aim = state.aim + step.amount },
                    _ => throw new Exception(),
                },
                res => res.x * res.y
            )
            .Should().Be(1956047400);
    }
    
    private static IEnumerable<Input> Parse(string[] input) =>
        from line in input
        let parts = line.Split()
        select new Input(parts[0][0], int.Parse(parts[1]));
    
    private record Input(char dir, int amount);

    private record State1(int x, int y);

    private record State2(int x, int y, int aim);
}