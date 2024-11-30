namespace AOC._2020;

public class Day15
{
    private readonly string _input = File.ReadAllText("Inputs/Day15.txt");

    [Fact]
    public void Part1() => NumberAt(_input, 2020).Should().Be(403);
    
    [Fact]
    public void Part2() => NumberAt(_input, 30000000).Should().Be(6823);
    
    private static int NumberAt(string input, int count)
    {
        var numbers = input.Split(",").Select(int.Parse).ToArray();
        var (lastSeen, number) = (new int[count], numbers[0]);
        for (var round = 0; round < count; round++)
        {
            (lastSeen[number], number) = (round,
                round < numbers.Length 
                    ? numbers[round] 
                    : lastSeen[number] == 0 
                        ? 0 
                        : round - lastSeen[number]);
        }

        return number;
    }
}