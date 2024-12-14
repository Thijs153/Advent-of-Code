using System.Text;
using System.Text.RegularExpressions;

namespace AOC._2024;

public class Day14
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day14.txt");

    private const int Width = 101;
    private const int Height = 103;
    
    [Fact]
    public void Part1()
    {
        var quadrants = Simulate(_input)
            .ElementAt(100)
            .CountBy(GetQuadrant)
            .Where(group => group.Key.X != 0 && group.Key.Y != 0) // ignore ones at the middle.
            .Select(group => group.Value)
            .ToArray();

        var safetyFactor = quadrants[0] * quadrants[1] * quadrants[2] * quadrants[3];

        safetyFactor.Should().Be(215987200);
    }

    [Fact]
    public void Part2()
    {
        Simulate(_input)
            .TakeWhile(robots => !Plot(robots).Contains("#################"))
            .Count()
            .Should().Be(8050);
    }

    private static IEnumerable<Robot[]> Simulate(string[] input)
    {
        var robots = Parse(input).ToArray();
        while (true)
        {
            yield return robots;
            robots = robots.Select(Step).ToArray();
        }
        
        // ReSharper disable once IteratorNeverReturns
    }
    
    // Advance a robot by its velocity taking care of the 'teleportation'
    private static Robot Step(Robot robot) =>
        robot with { Position = AddWithWrapAround(robot.Position, robot.Velocity) };
    
    // Returns the direction (-1/0/1) of the robot to the center of the room
    private static Vec2 GetQuadrant(Robot robot) =>
        new(Math.Sign(robot.Position.X - Width / 2), Math.Sign(robot.Position.Y - Height / 2));
    
    private static Vec2 AddWithWrapAround(Vec2 a, Vec2 b) =>
        new((a.X + b.X + Width) % Width, (a.Y + b.Y + Height) % Height);
    
    // Shows the robot locations in the room.
    private static string Plot(IEnumerable<Robot> robots)
    {
        var result = new char[Height, Width];
        foreach (var robot in robots)
        {
            result[robot.Position.Y, robot.Position.X] = '#';
        }

        var sb = new StringBuilder();
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                sb.Append(result[y, x] == '#' ? "#" : " ");
            }

            sb.AppendLine();
        }

        return sb.ToString();
    }
    
    private static IEnumerable<Robot> Parse(string[] input) =>
        from line in input
        let nums = Regex.Matches(line, @"-?\d+").Select(m => int.Parse(m.Value)).ToArray()
        select new Robot(new Vec2(nums[0], nums[1]), new Vec2(nums[2], nums[3]));

    private record struct Vec2(int X, int Y);
    private record struct Robot(Vec2 Position, Vec2 Velocity);
}