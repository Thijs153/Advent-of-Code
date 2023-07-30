using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2020;

[TestFixture]
public class Day04
{
    private readonly string _input = File.ReadAllText("Inputs/Day04.txt").ReplaceLineEndings("\n");

    [Test]
    public void Part1()
    {
        ValidCount(_input, cred => _rxs.All(kvp => cred.ContainsKey(kvp.Key)))
            .Should().Be(242);
    }

    [Test]
    public void Part2()
    {
        ValidCount(_input, cred => _rxs.All(kvp =>
                cred.TryGetValue(kvp.Key, out var value) && 
                Regex.IsMatch(value, "^(" + kvp.Value + ")$")))
            .Should().Be(186);
    }
    
    private readonly Dictionary<string, string> _rxs = new (){
        {"byr", "19[2-9][0-9]|200[0-2]"},
        {"iyr", "201[0-9]|2020"},
        {"eyr", "202[0-9]|2030"},
        {"hgt", "1[5-8][0-9]cm|19[0-3]cm|59in|6[0-9]in|7[0-6]in"},
        {"hcl", "#[0-9a-f]{6}"},
        {"ecl", "amb|blu|brn|gry|grn|hzl|oth"},
        {"pid", "[0-9]{9}"},
    };

    private static int ValidCount(string input, Func<Dictionary<string, string>, bool> isValid) =>
        input.Split("\n\n")
            .Select(block => block
                .Split("\n ".ToCharArray())
                .Select(part => part.Split(":"))
                .ToDictionary(parts => parts[0], parts => parts[1]))
            .Count(isValid);
}