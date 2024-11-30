namespace AOC._2022;

public class Day07
{
    private readonly List<int> _directorySizes;

    public Day07()
    {
        var path = new Stack<string>();
        var sizes = new Dictionary<string, int>();
        foreach (var line in File.ReadLines("Inputs/Day07.txt"))
        {
            if (line == "$ ls" || line.StartsWith("dir")) continue;
            if (line == "$ cd ..") {
                path.Pop();
            } else if (line.StartsWith("$ cd")) {
                path.Push(string.Join("", path)+line.Split(" ")[2]);
            } else {
                var size = int.Parse(line.Split(" ")[0]);
                foreach (var dir in path) {
                    sizes[dir] = sizes.GetValueOrDefault(dir) + size;
                }
            }
        }
        _directorySizes = sizes.Values.ToList();
    }
    
    [Fact]
    public void Part1()
    {
        _directorySizes.Where(size => size < 100_000).Sum().Should().Be(1555642);
    }
    
    [Fact]
    public void Part2()
    {
        _directorySizes.Where(size => size + (70_000_000 - _directorySizes.Max()) >= 30_000_000).Min()
            .Should().Be(5974547);
    }
}