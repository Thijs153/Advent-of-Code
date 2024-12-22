using System.Collections.Concurrent;

namespace AOC._2024;

using Cache = ConcurrentDictionary<(char CurrentKey, char NextKey, int Depth), long>;
using Keypad = Dictionary<Day21.Vec2, char>;

public class Day21
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day21.txt");

    [Fact]
    public void Part1() => Solve(_input, 2).Should().Be(163920L);

    [Fact]
    public void Part2() => Solve(_input, 25).Should().Be(204040805018350L);
    
    
    private static long Solve(string[] input, int depth)
    {
        var keypad1 = ParseKeypad(["789", "456", "123", " 0A"]);
        var keypad2 = ParseKeypad([" ^A", "<v>"]);
        var keypads = Enumerable.Repeat(keypad2, depth).Prepend(keypad1).ToArray();

        var cache = new Cache();

        return (
            from line in input 
            let num = int.Parse(line[..^1])
            select num * EncodeKeys(line, keypads, cache)
        ).Sum();
    }
    
    /*
     * Determines the length of the shortest sequence that is needed to enter the given keys.
     * An empty keypad array means that the sequence is simply entered by a human
     * and no further encoding is needed. Otherwise, the sequence is entered by a robot
     * which needs to be programmed. In practice this means that the keys are encoded
     * using the robots keypad (the first keypad), generating another sequence of keys.
     * This other sequence is then recursively encoded using the rest of the keypads.
     */
    private static long EncodeKeys(string keys, Keypad[] keypads, Cache cache)
    {
        if (keypads.Length == 0)
        {
            return keys.Length;
        }
        
        // invariant: the robot starts and finishes by pointing at the 'A' key.
        var currentKey = 'A';
        var length = 0L;

        foreach (var nextKey in keys)
        {
            length += EncodeKey(currentKey, nextKey, keypads, cache);
            
            currentKey = nextKey;
        }
        
        currentKey.Should().Be('A', because: "The robot should point at the 'A' key");
        return length;
    }
    
    private static long EncodeKey(char currentKey, char nextKey, Keypad[] keypads, Cache cache) =>
        cache.GetOrAdd((currentKey, nextKey, keypads.Length), _ =>
        {
            var keypad = keypads[0];

            var currentPos = keypad.Single(kvp => kvp.Value == currentKey).Key;
            var nextPos = keypad.Single(kvp => kvp.Value == nextKey).Key;

            var dy = nextPos.Y - currentPos.Y;
            var vertical = new string(dy < 0 ? 'v' : '^', Math.Abs(dy));

            var dx = nextPos.X - currentPos.X;
            var horizontal = new string(dx < 0 ? '<' : '>', Math.Abs(dx));

            var cost = long.MaxValue;
            
            if (keypad[new Vec2(currentPos.X, nextPos.Y)] != ' ')
            {
                cost = Math.Min(cost, EncodeKeys($"{vertical}{horizontal}A", keypads[1..], cache));
            }

            if (keypad[new Vec2(nextPos.X, currentPos.Y)] != ' ')
            {
                cost = Math.Min(cost, EncodeKeys($"{horizontal}{vertical}A", keypads[1..], cache));
            }

            return cost;
        });

    private static Keypad ParseKeypad(string[] keypad) => (
        from y in Enumerable.Range(0, keypad.Length)
        from x in Enumerable.Range(0, keypad[0].Length)
        select new KeyValuePair<Vec2, char>(new Vec2(x, -y), keypad[y][x])
    ).ToDictionary();
        
    public record struct Vec2(int X, int Y);
}