using System.Collections;
using System.Text.RegularExpressions;

namespace AOC._2022;

public class Day16
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day16.txt");

    [Fact]
    public void Part1()
    {
        Solve(_input, true, 30)
            .Should().Be(1850);
    }

    [Fact]
    public void Part2()
    {
        Solve(_input, false, 26)
            .Should().Be(2306);
    }
    
    private int Solve(IEnumerable<string> input, bool singlePlayer, int time)
    {
        var map = Parse(input);
        var start = map.Valves.Single(x => x.Name == "AA");
        
        var valvesToOpen = new BitArray(map.Valves.Length);
        for (var i = 0; i < map.Valves.Length; i++)
        {
            if (map.Valves[i].FlowRate > 0)
                valvesToOpen[i] = true;
        }

        if (singlePlayer)
        {
            return MaxFlow(map, 0, 0, new Player(start, 0), new Player(start, int.MaxValue), valvesToOpen, time);
        }

        return MaxFlow(map, 0, 0, new Player(start, 0), new Player(start, 0), valvesToOpen, time);
    }
    
    private record Map(int[,] Distances, Valve[] Valves);
    private record Valve(int Id, string Name, int FlowRate, string[] Tunnels);
    private record Player(Valve Valve, int Distance);

    int MaxFlow(
        Map map,               // this is our map as per task input
        int maxFlow,           // is the current maximum we found
        int currentFlow,       // the flow produced by the currently investigated steps
        Player player0,        // this is the 'human' player
        Player player1,        // this can be a second player
        BitArray valvesToOpen, // these valves can still be opened
        int remainingTime      // the remaining time
    )
    {
        if (player0.Distance != 0 && player1.Distance != 0)
            throw new ArgumentException();

        var nextStatesByPlayer = new Player[2][];

        for (var iPlayer = 0; iPlayer < 2; iPlayer++)
        {
            var player = iPlayer == 0 ? player0 : player1;

            if (player.Distance > 0)
            {
                // this player just steps forward towards the valve
                nextStatesByPlayer[iPlayer] = new[] { player with { Distance = player.Distance - 1 } };
            } 
            else if (valvesToOpen[player.Valve.Id])
            {
                // the player is next to the valve, the valve is still closed, let's open:
                // (this takes 1 time, so multiply with remainingTime -1
                currentFlow += player.Valve.FlowRate * (remainingTime - 1);

                if (currentFlow > maxFlow)
                    maxFlow = currentFlow;

                valvesToOpen = new BitArray(valvesToOpen); // copy on write
                valvesToOpen[player.Valve.Id] = false;
                
                // in the next round this player will take some new target,
                // but it already used up it's 1 minute this round for opening the valve
                nextStatesByPlayer[iPlayer] = new[] { player };
            }
            else
            {
                // the valve is already open, let's try each valves that are still closed:
                // this is where branching happens
                
                var nextStates = new List<Player>();

                for (var i = 0; i < valvesToOpen.Length; i++)
                {
                    if (!valvesToOpen[i]) continue;
                    
                    var nextValve = map.Valves[i];
                    var distance = map.Distances[player.Valve.Id, nextValve.Id];
                    // the player moves in this time slot towards the valve, so use distance - 1
                    nextStates.Add(new Player(nextValve, distance - 1));
                }

                nextStatesByPlayer[iPlayer] = nextStates.ToArray();
            }
        }
        
        // ran out of time, cannot improve maxFlow
        remainingTime--;

        if (remainingTime < 1)
            return maxFlow;
        
        // there is not enough juice left for the remaining time to improve on maxFlow
        // we can shortcut here
        if (currentFlow + Residue(valvesToOpen, map, remainingTime) <= maxFlow)
            return maxFlow;
        
        // all is left is going over every possible step combinations for each player:
        for (var i0 = 0; i0 < nextStatesByPlayer[0].Length; i0++)
        {
            for (var i1 = 0; i1 < nextStatesByPlayer[1].Length; i1++)
            {
                player0 = nextStatesByPlayer[0][i0];
                player1 = nextStatesByPlayer[1][i1];
                
                // there is no point in walking to the same valve
                // if one of the players has other things to do:
                if ((nextStatesByPlayer[0].Length > 1 || nextStatesByPlayer[1].Length > 1) 
                    && player0.Valve == player1.Valve)
                    continue;
                
                // this is another optimization, if both players are walking
                // we can advance time until one of them reaches target:
                var advance = 0;
                if (player0.Distance > 0 && player1.Distance > 0)
                {
                    advance = Math.Min(player0.Distance, player1.Distance);
                    player0 = player0 with { Distance = player0.Distance - advance };
                    player1 = player1 with { Distance = player1.Distance - advance };
                }

                maxFlow = MaxFlow(
                    map, maxFlow, currentFlow, player0, player1, valvesToOpen, remainingTime - advance);

            }
        }

        return maxFlow;
    }

    private int Residue(BitArray valvesToOpen, Map map, int remainingTime)
    {
        var flow = 0;
        for (var i = 0; i < valvesToOpen.Length; i++)
        {
            if (remainingTime <= 0)
                break;

            flow += map.Valves[i].FlowRate * (remainingTime - 1);

            if ((i & 1) == 0)
                remainingTime--;

        }

        return flow;
    }

    private Map Parse(IEnumerable<string> input)
    {
        // Valve BB has flow rate=0; tunnels lead to valve CC
        // Valve CC has flow rate=10; tunnels lead to valves DD, EE
        var valveList = new List<Valve>();
        foreach (var line in input) {
            var name = Regex.Match(line, "Valve (.*) has").Groups[1].Value;
            var flow = int.Parse(Regex.Match(line, @"\d+").Groups[0].Value);
            var tunnels = Regex.Match(line, "to valves? (.*)").Groups[1].Value.Split(", ").ToArray();
            valveList.Add(new Valve(0, name, flow, tunnels));
        }
        var valves = valveList
            .OrderByDescending(valve => valve.FlowRate)
            .Select((v, i) => v with { Id = i })
            .ToArray();

        return new Map(ComputeDistances(valves), valves);
    }

    private int[,] ComputeDistances(Valve[] valves)
    {
        var distances = new int[valves.Length, valves.Length];
        for (var i = 0; i < valves.Length; i++)
        {
            for (var j = 0; j < valves.Length; j++)
            {
                distances[i, j] = int.MaxValue;
            }
        }
        
        foreach (var valve in valves) {
            foreach (var target in valve.Tunnels) {
                var targetNode = valves.Single(x => x.Name == target);
                distances[valve.Id, targetNode.Id] = 1;
                distances[targetNode.Id, valve.Id] = 1;
            }
        }

        var n = distances.GetLength(0);
        var done = false;
        while (!done) {
            done = true;
            for (var source = 0; source < n; source++) {
                for (var target = 0; target < n; target++) {
                    if (source != target) {
                        for (var through = 0; through < n; through++) {
                            if (distances[source, through] == int.MaxValue || distances[through, target] == int.MaxValue) {
                                continue;
                            }
                            var cost = distances[source, through] + distances[through, target];
                            if (cost < distances[source, target]) {
                                done = false;
                                distances[source, target] = cost;
                                distances[target, source] = cost;
                            }
                        }
                    }
                }
            }
        }
        return distances;
    }
    
}