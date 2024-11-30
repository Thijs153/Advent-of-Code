using System.Text.RegularExpressions;

namespace AOC._2021;

public class Day17
{
    private readonly string _input = File.ReadAllText("Inputs/Day17.txt");

    [Fact]
    public void Part1()
    {
        Solve(_input).Max()
            .Should().Be(5995);
    }

    [Fact]
    public void Part2()
    {
        Solve(_input).Count()
            .Should().Be(3202);
    }
    
    private static IEnumerable<int> Solve(string input)
    {
        var m = Regex.Matches(input, "-?[0-9]+").Select(m => int.Parse(m.Value)).ToArray();

        // Get the target rectangle
        var (xMin, xMax) = (m[0], m[1]);
        var (yMin, yMax) = (m[2], m[3]);

        // Bounds for the initial horizontal and vertical speeds:
        var vx0Min = 0; 
        var vx0Max = xMax;  
        var vy0Min = yMin;  
        var vy0Max = -yMin;

        // Run the simulation in the given bounds, maintaining maxY
        for (var vx0 = vx0Min; vx0 <= vx0Max; vx0++) {
            for (var vy0 = vy0Min; vy0 <= vy0Max; vy0++) {

                var (x, y, vx, vy) = (0, 0, vx0, vy0);
                var maxY = 0;
                
                while (x <= xMax && y >= yMin) {
                   
                    x += vx;
                    y += vy;
                    vy -= 1;
                    vx = Math.Max(0, vx - 1);
                    maxY = Math.Max(y, maxY);

                    // if we are within target, yield maxY:
                    if (x >= xMin && x <= xMax && y >= yMin && y <= yMax) {
                        yield return maxY;
                        break;
                    }
                }
            }
        }
    }
}