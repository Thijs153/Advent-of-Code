using System.Text.RegularExpressions;

namespace AOC._2022;

public class Day19
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day19.txt");

    [Fact]
    public void Part1()
    {
        var res = 0;
        foreach (var blueprint in Parse(_input))
        {
            res += blueprint.Id * MaxGeodes(blueprint, 24);
        }

        res.Should().Be(2301);
    }

    [Fact]
    public void Part2()
    {
        var res = 1;
        foreach (var blueprint in Parse(_input).Where(bp => bp.Id <= 3))
        {
            res *= MaxGeodes(blueprint, 32);
        }

        res.Should().Be(10336);
    }
    
    private static int MaxGeodes(Blueprint bluePrint, int timeLimit)
    {
        return MaxGeodes(
            bluePrint,
            new State(
                RemainingTime: timeLimit,
                Available: new Material(Ore: 0, 0, 0, 0),
                Produced: new Material(Ore: 1, 0, 0, 0)
            ),
            new Dictionary<State, int>()
        );
    }
    
    // Returns the maximum mine-able geodes under the given state constraints,
    // Recursion with a cache.
    private static int MaxGeodes(Blueprint bluePrint, State state, IDictionary<State, int> cache)
    {
        if (state.RemainingTime == 0)
            return state.Available.Geode;

        if (!cache.ContainsKey(state))
        {
            cache[state] = (
                from afterFactory in NextSteps(bluePrint, state)
                let afterMining = afterFactory with {
                    RemainingTime = state.RemainingTime - 1,
                    Available = afterFactory.Available + state.Produced
                }
                select MaxGeodes(bluePrint, afterMining, cache)
            ).Max();
        }

        return cache[state];
    }
    
    // Returns all possible factory steps
    private static IEnumerable<State> NextSteps(Blueprint bluePrint, State state)
    {
        var now = state.Available;
        var prev = now - state.Produced;

        // We consider building a minder only if we couldn't build it in the previous step
        
        // Geode
        if (!CanBuild(bluePrint.Geode, prev) && CanBuild(bluePrint.Geode, now))
        {
            yield return Build(state, bluePrint.Geode);
            
            // Building a geode miner asap seems to be an optimal choice
            // no need to try anything else.
            yield break;
        }
        // Obsidian
        if (!CanBuild(bluePrint.Obsidian, prev) && CanBuild(bluePrint.Obsidian, now))
            yield return Build(state, bluePrint.Obsidian);
        // Clay
        if (!CanBuild(bluePrint.Clay, prev) && CanBuild(bluePrint.Clay, now))
            yield return Build(state, bluePrint.Clay);
        // Ore
        if (!CanBuild(bluePrint.Ore, prev) && CanBuild(bluePrint.Ore, now))
            yield return Build(state, bluePrint.Ore);

        yield return state;
    }

    private static bool CanBuild(Robot robot, Material availableMaterial) => 
        availableMaterial >= robot.Cost;
    
    private static State Build(State state, Robot robot) =>
        state with
        {
            Available = state.Available - robot.Cost,
            Produced = state.Produced + robot.Produces
        };

    private static IEnumerable<Blueprint> Parse(string[] input)
    {
        foreach (var line in input)
        {
            var numbers = Regex.Matches(line, @"(\d+)")
                .Select(x => int.Parse(x.Value)).ToArray();
            
            yield return new Blueprint(
                Id: numbers[0],
                Ore: new Robot(
                    Cost: new Material(Ore: numbers[1], Clay: 0, Obsidian: 0, Geode: 0),
                    Produces: new Material(Ore: 1, Clay: 0, Obsidian: 0, Geode: 0)
                ),
                Clay: new Robot(
                    Cost: new Material(Ore: numbers[2], Clay: 0, Obsidian: 0, Geode: 0),
                    Produces: new Material(Ore: 0, Clay: 1, Obsidian: 0, Geode: 0)
                ),
                Obsidian: new Robot(
                    Cost: new Material(Ore: numbers[3], Clay: numbers[4], Obsidian: 0, Geode: 0),
                    Produces: new Material(Ore: 0, Clay: 0, Obsidian: 1, Geode: 0)
                ),
                Geode: new Robot(
                    Cost: new Material(Ore: numbers[5], Clay: 0, Obsidian: numbers[6], Geode: 0),
                    Produces: new Material(Ore: 0, Clay: 0, Obsidian: 0, Geode: 1)
                )
            );
        }
    }

    private record Material(int Ore, int Clay, int Obsidian, int Geode)
    {
        public static Material operator +(Material a, Material b)
        {
            return new Material(
                a.Ore + b.Ore,
                a.Clay + b.Clay,
                a.Obsidian + b.Obsidian,
                a.Geode + b.Geode
            );
        }

        public static Material operator -(Material a, Material b)
        {
            return new Material(
                a.Ore - b.Ore,
                a.Clay - b.Clay,
                a.Obsidian - b.Obsidian,
                a.Geode - b.Geode
            );
        }
        
        public static bool operator <=(Material a, Material b) {
            return
                a.Ore <= b.Ore &&
                a.Clay <= b.Clay &&
                a.Obsidian <= b.Obsidian &&
                a.Geode <= b.Geode;
        }

        public static bool operator >=(Material a, Material b) {
            return
                a.Ore >= b.Ore &&
                a.Clay >= b.Clay &&
                a.Obsidian >= b.Obsidian &&
                a.Geode >= b.Geode;
        }
    }
    private record Robot(Material Cost, Material Produces);

    private record State(int RemainingTime, Material Available, Material Produced);

    private record Blueprint(int Id, Robot Ore, Robot Clay, Robot Obsidian, Robot Geode);
}