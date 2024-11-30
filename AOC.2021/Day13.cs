using System.Drawing;

namespace AOC._2021;

public class Day13
{
    private readonly string _input = File.ReadAllText("Inputs/Day13.txt");

    [Fact]
    public void Part1()
    {
        GetFolds(_input).First().Count
            .Should().Be(671);
    }

    [Fact]
    public void Part2()
    {
        ToString(GetFolds(_input).Last())
            .Should().Be(
                "###   ##  ###  #  #  ##  ###  #  # #   \n" +
                "#  # #  # #  # #  # #  # #  # # #  #   \n" +
                "#  # #    #  # #### #  # #  # ##   #   \n" +
                "###  #    ###  #  # #### ###  # #  #   \n" +
                "#    #  # #    #  # #  # # #  # #  #   \n" +
                "#     ##  #    #  # #  # #  # #  # ####\n"
            );
        
        // PCPHARKL
    }
    
    private static IEnumerable<HashSet<Point>> GetFolds(string input)
    {
        input = input.Replace("\r", "");
        var blocks = input.Split("\n\n");
        
        // parse points into a hashset
        var points = (
            from line in blocks[0].Split("\n")
            let coords = line.Split(",")
            select new Point(int.Parse(coords[0]), int.Parse(coords[1]))
        ).ToHashSet();
        
        // fold line by line, yielding a new hashset
        foreach (var line in blocks[1].Split("\n"))
        {
            var rule = line.Split("=");
            
            points = rule[0].EndsWith("x") 
                ? FoldX(int.Parse(rule[1]), points) 
                : FoldY(int.Parse(rule[1]), points);

            yield return points;
        }
    }

    private static string ToString(HashSet<Point> d)
    {
        var res = "";
        var height = d.MaxBy(p => p.Y).Y;
        var width = d.MaxBy(p => p.X).X;
        for (var y = 0; y <= height; y++) {
            for (var x = 0; x <= width; x++) {
                res += d.Contains(new Point(x, y)) ? '#' : ' ';
            }
            res += "\n";
        }
        return res;
    }
    
    private static HashSet<Point> FoldX(int x, HashSet<Point> d) =>
        d.Select(p => p.X > x ? p with { X = 2 * x - p.X } : p).ToHashSet();
    
    private static HashSet<Point> FoldY(int y, HashSet<Point> d) =>
        d.Select(p => p.Y > y ? p with { Y = 2 * y - p.Y } : p).ToHashSet();
}