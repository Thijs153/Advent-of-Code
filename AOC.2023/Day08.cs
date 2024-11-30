using System.Text.RegularExpressions;

namespace AOC._2023;

public class Day08
{
    private readonly string _input = File.ReadAllText("Inputs/Day08.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1() => Solve(_input, "AAA", "ZZZ").Should().Be(14257);

    [Fact]
    public void Part2() => Solve(_input, "..A", "..Z").Should().Be(16187743689077);

    private static long Solve(string input, string start, string end)
    {
        var parts = input.Split("\n\n");
        var dirs = parts[0];
        var map = new Dictionary<string, (string, string)>();
        
        foreach (var line in parts[1].Split("\n")) {
            var m = Regex.Matches(line, "[A-Z]+").ToArray();
            map[m[0].Value] = (m[1].Value, m[2].Value);
        }

        var res = 1L;
        foreach(var st in map.Keys) {
            if (Regex.IsMatch(st, start)) {
                res = Lcm(res, Steps(st, end, dirs, map));
            }
        }
        return res;
    }
    
    private static long Lcm(long a, long b) => a * b / Gcd(a, b);
    private static long Gcd(long a, long b) => b == 0 ? a : Gcd(b, a % b);
    
    private static long Steps(string st, string end, string dirs, Dictionary<string, (string, string)> map)
    {
        var i = 0;
        while (!Regex.IsMatch(st, end)) {
            var dir = dirs[i % dirs.Length];
            st = dir == 'L' ? map[st].Item1 : map[st].Item2;
            i++;
        }
        return i;
    }
}