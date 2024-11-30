using System.Numerics;
using Map = System.Collections.Generic.Dictionary<System.Numerics.Complex, char>;
using Node = long;

namespace AOC._2023;

public class Day23
{
    private readonly string _input = File.ReadAllText("Inputs/Day23.txt").ReplaceLineEndings("\n");

    /*
     * Instead of dealing with the 'map' tiles directly, we convert it into a graph.
     * Nodes: the entry tile, the exit, and the crossroad tiles.
     * Edges: two nodes are connected if there is a direct path between them that
     *        doesn't contain crossroads.
     * This reduces a problem to ~3- nodes and 120 edges for the Part2 case
     * which can be solved using a dynamic programming approach.
     */
    
    [Fact]
    public void Part1() => Solve(_input).Should().Be(2294);

    [Fact]
    public void Part2() => Solve(RemoveSlopes(_input)).Should().Be(6418);
    
    private static readonly Complex Up = -Complex.ImaginaryOne;
    private static readonly Complex Down = Complex.ImaginaryOne;
    private static readonly Complex Left = -1;
    private static readonly Complex Right = 1;
    private static readonly Complex[] Dirs = [Up, Down, Left, Right];
    
    private static readonly Dictionary<char, Complex[]> Exits = new() {
        ['<'] = [Left],
        ['>'] = [Right],
        ['^'] = [Up],
        ['v'] = [Down],
        ['.'] = Dirs,
        ['#'] = []
    };

    private static string RemoveSlopes(string st) =>
        string.Join("", st.Select(ch => ">v<^".Contains(ch) ? '.' : ch));
    
    private static int Solve(string input)
    {
        var (nodes, edges) = MakeGraph(input);
        var (start, goal) = (nodes.First(), nodes.Last());
        
        // Dynamic programming using a cache, 'visited' is a bitset of 'nodes'.
        var cache = new Dictionary<(Node, long), int>();

        int LongestPath(Node node, long visited)
        {
            if (node == goal)
            {
                return 0;
            }

            if ((visited & node) != 0)
            {
                return int.MinValue;
            }

            var key = (node, visited);
            if (!cache.ContainsKey(key))
            {
                cache[key] = edges
                    .Where(e => e.start == node)
                    .Select(e => e.distance + LongestPath(e.end, visited | node))
                    .Max();
            }

            return cache[key];
        }

        return LongestPath(start, 0);
    }
    
    private static (Node[], Edge[]) MakeGraph(string input)
    {
        var map = ParseMap(input);
        
        // row-major order: 'entry' node comes first and 'exit' is last
        var nodePos = (
            from pos in map.Keys
            orderby pos.Imaginary, pos.Real
            where IsFree(map, pos) && !IsRoad(map, pos)
            select pos
        ).ToArray();

        var nodes = (
            from i in Enumerable.Range(0, nodePos.Length) select 1L << i
        ).ToArray();

        var edges = (
            from i in Enumerable.Range(0, nodePos.Length)
            from j in Enumerable.Range(0, nodePos.Length)
            where i != j
            let distance = Distance(map, nodePos[i], nodePos[j])
            where distance > 0
            select new Edge(nodes[i], nodes[j], distance)
        ).ToArray();

        return (nodes, edges);
    }
    
    // Length of the road between two crossroads; -1 if not neighbours
    private static int Distance(Map map, Complex crossroadA, Complex crossroadB)
    {
        var q = new Queue<(Complex, int)>();
        q.Enqueue((crossroadA, 0));

        var visited = new HashSet<Complex> { crossroadA };
        while (q.Count != 0)
        {
            var (pos, dist) = q.Dequeue();
            foreach (var dir in Exits[map[pos]])
            {
                var posT = pos + dir;
                if (posT == crossroadB)
                {
                    return dist + 1;
                }

                if (IsRoad(map, posT) && visited.Add(posT))
                {
                    q.Enqueue((posT, dist + 1));
                }
            }
        }

        return -1;
    }
    
    private static bool IsFree(Map map, Complex p) =>
        map.ContainsKey(p) && map[p] != '#';
    
    private static bool IsRoad(Map map, Complex p) =>
        IsFree(map, p) && Dirs.Count(d => IsFree(map, p + d)) == 2;
    
    private static Map ParseMap(string input)
    {
        var lines = input.Split("\n");
        return (
            from iRow in Enumerable.Range(0, lines.Length)
            from iCol in Enumerable.Range(0, lines[0].Length)
            let pos = new Complex(iCol, iRow)
            select new KeyValuePair<Complex, char>(pos, lines[iRow][iCol])
        ).ToDictionary();
    }

    private record Edge(Node start, Node end, int distance);
}