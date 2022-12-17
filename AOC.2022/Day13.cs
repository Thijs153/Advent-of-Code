using System.Text.Json.Nodes;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day13
{
    private readonly List<string> _input = File.ReadAllLines("Inputs/Day13.txt").ToList();

    [Test]
    public void Part1()
    {
        GetPackets()
            .Chunk(2)
            .Select((pair, index) => Compare(pair[0], pair[1]) < 0 ? index + 1 : 0)
            .Sum()
            .Should().Be(5366);
    }

    [Test]
    public void Part2()
    {
        var divider = GetPackets("[[2]]\n[[6]]").ToList();
        var packets = GetPackets().Concat(divider).ToList();
        packets.Sort(Compare);
        
        ((packets.IndexOf(divider[0]) + 1) * (packets.IndexOf(divider[1]) + 1))
            .Should().Be(23391);
    }

    private static IEnumerable<JsonNode> GetPackets(string input) =>
        from line in input.Split("\n")
        where !string.IsNullOrEmpty(line)
        select JsonNode.Parse(line);

    private IEnumerable<JsonNode> GetPackets() =>
        from line in _input
        where !string.IsNullOrEmpty(line)
        select JsonNode.Parse(line);

    private static int Compare(JsonNode nodeA, JsonNode nodeB)
    {
        if (nodeA is JsonValue && nodeB is JsonValue)
        {
            return (int)nodeA - (int)nodeB;
        }

        var arrayA = nodeA as JsonArray ?? new JsonArray((int)nodeA);
        var arrayB = nodeB as JsonArray ?? new JsonArray((int)nodeB);
        
        return arrayA.Zip(arrayB)
            .Select(p => Compare(p.First!, p.Second!))
            .FirstOrDefault(c => c != 0, arrayA.Count - arrayB.Count);
    }
}