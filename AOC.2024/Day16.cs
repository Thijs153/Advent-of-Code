using System.Numerics;

namespace AOC._2024;

using Map = Dictionary<Complex, char>;
using State = (Complex Position, Complex Direction);

public class Day16
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day16.txt");

    private static readonly Complex North = -Complex.ImaginaryOne;
    private static readonly Complex South = Complex.ImaginaryOne;
    private static readonly Complex East = 1;
    private static readonly Complex West = -1;
    private static readonly Complex[] Directions = [North, South, East, West];

    [Fact]
    public void Part1() => FindBestScore(GetMap(_input)).Should().Be(103512);

    [Fact]
    public void Part2() => FindBestSpots(GetMap(_input)).Should().Be(554);
    
    private static int FindBestScore(Map map) => Dijkstra(map, Goal(map))[Start(map)];

    private static int FindBestSpots(Map map)
    {
        var dist = Dijkstra(map, Goal(map));
        var start = Start(map);
        
        // track the shortest paths using the distance map as guideline.
        var queue = new PriorityQueue<State, int>();
        queue.Enqueue(start, dist[start]);

        var bestSpots = new HashSet<State> { start };
        while (queue.TryDequeue(out var state, out var remainingScore))
        {
            foreach (var (next, score) in Steps(map, state, forward: true))
            {
                var nextRemainingScore = remainingScore - score;
                if (!bestSpots.Contains(next) && dist[next] == nextRemainingScore)
                {
                    bestSpots.Add(next);
                    queue.Enqueue(next, nextRemainingScore);
                }
            }
        }

        return bestSpots.DistinctBy(state => state.Position).Count();
    }
    
    private static Dictionary<State, int> Dijkstra(Map map, Complex goal)
    {
        var dist = new Dictionary<State, int>();
        
        var queue = new PriorityQueue<State, int>();
        foreach (var direction in Directions)
        {
            queue.Enqueue((goal, direction), 0);
            dist[(goal, direction)] = 0;
        }

        while (queue.TryDequeue(out var current, out var totalDistance))
        {
            var steps = Steps(map, current, forward: false).ToList();
            foreach (var (next, score) in steps)
            {
                var nextCost = totalDistance + score;
                if (nextCost < dist.GetValueOrDefault(next, int.MaxValue))
                {
                    queue.Remove(next, out _, out _);
                    dist[next] = nextCost;
                    queue.Enqueue(next, nextCost);
                }
            }
        }

        return dist;
    }
    
    /*
     * returns the possible next or previous states and the associated costs for a given state.
     * in forward mode we scan the possible states from the start state towards the goal.
     * in backward mode we are working backwards from the goal to the start.
     */
    private static IEnumerable<(State, int Cost)> Steps(Map map, State state, bool forward)
    {
        foreach (var direction in Directions)
        {
            if (direction == state.Direction)
            {
                var position = forward ? state.Position + direction : state.Position - direction;
                if (map.GetValueOrDefault(position) != '#')
                {
                    yield return ((Position: position, Direction: direction), 1);
                }
            } 
            else if (direction != -state.Direction)
            {
                yield return (state with { Direction = direction }, 1000);
            }
        }   
    }
    
    private static Map GetMap(string[] input) => (
        from y in Enumerable.Range(0, input.Length)
        from x in Enumerable.Range(0, input[0].Length)
        select new KeyValuePair<Complex, char>(Complex.ImaginaryOne * y + x, input[y][x])
    ).ToDictionary();

    private static Complex Goal(Map map) => map.Keys.Single(k => map[k] == 'E');
    private static State Start(Map map) => (map.Keys.Single(k => map[k] == 'S'), East);
}