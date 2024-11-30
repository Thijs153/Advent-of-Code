namespace AOC._2022;

public class Day01
{
    private readonly IList<int> _totalCaloriesPerElf = new List<int>();

    public Day01()
    {
        var totalCalories = 0;
        foreach (var line in File.ReadAllLines("Inputs/Day01.txt"))
        {
            if (line == "")
            {
                _totalCaloriesPerElf.Add(totalCalories);
                totalCalories = 0;
                continue;
            }

            totalCalories += int.Parse(line);
        }

        _totalCaloriesPerElf.Add(totalCalories);
    }

    [Fact]
    public void Part1() => _totalCaloriesPerElf.Max().Should().Be(69289);
    
    [Fact]
    public void Part2() => _totalCaloriesPerElf.OrderByDescending(x => x).Take(3).Sum().Should().Be(205615);
}