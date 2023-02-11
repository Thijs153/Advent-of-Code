using System.ComponentModel.Design;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day21
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day21.txt");

    [Test]
    public void Part1()
    {
        var threeRoll = DeterministicThrows().Chunk(3).Select(x => x.Sum());
        
        // take turns until the active player wins:
        var round = 0;
        var (active, other) = Parse(_input);
        foreach (var steps in threeRoll)
        {
            round++;
            active = active.Move(steps);
            if (active.score >= 1000)
            {
                break;
            }

            (active, other) = (other, active);
        }

        var result = other.score * 3 * round;

        result.Should().Be(998088);
    }

    [Test]
    public void Part2()
    {
        // win counts tells us how many times the active and the other player wins
        // if they are starting from the given positions and scores.
        
        // this function needs to be cached, because we don't have time till eternity ;).
        var cache = new Dictionary<(Player, Player), (long, long)>();

        (long activeWins, long otherWins) winCounts((Player active, Player other) players)
        {
            if (players.other.score >= 21)
            {
                return (0, 1);
            }

            if (!cache.ContainsKey(players))
            {
                var (activeWins, otherWins) = (0L, 0L);
                foreach (var steps in DiracThrows())
                {
                    var wins = winCounts((players.other, players.active.Move(steps)));
                    // they are switching roles here ^
                    // hence the return value needs to be swapped as well
                    activeWins += wins.otherWins;
                    otherWins += wins.activeWins;
                }

                cache[players] = (activeWins, otherWins);
            }

            return cache[players];
        }

        var wins = winCounts(Parse(_input));
        var result = Math.Max(wins.activeWins, wins.otherWins);

        result.Should().Be(306621346123766);
    }

    private static IEnumerable<int> DeterministicThrows() =>
        from i in Enumerable.Range(1, int.MaxValue)
        select (i - 1) % 100 + 1;

    private static IEnumerable<int> DiracThrows() =>
        from i in new[] { 1, 2, 3 }
        from j in new[] { 1, 2, 3 }
        from k in new[] { 1, 2, 3 }
        select i + j + k;
    

    private static (Player active, Player other) Parse(string[] input)
    {
        var players = (
            from line in input
            let parts = line.Split(": ")
            select new Player(0, int.Parse(parts[1]))
        ).ToArray();
        return (players[0], players[1]);
    }
    
    private record Player(int score, int pos)
    {
        public Player Move(int steps)
        {
            var newPos = (pos - 1 + steps) % 10 + 1;
            return new Player(score + newPos, newPos);
        }
    }
}