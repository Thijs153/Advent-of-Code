using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

public class Day25
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day25.txt");

    [Test]
    public void Part1()
    {
        var r = new Random();
        // call Karger's algorithm in a loop until it finds a cut with 3 edges:
        var (cutSize, c1, c2) = FindCut(_input, r);
        while (cutSize != 3)
        {
            (cutSize, c1, c2) = FindCut(_input, r);
        }

        var res = c1 * c2;
        res.Should().Be(538368);
    }
    
    
    /*
     * The Karger's algorithm returns the size of one 'cut' of the graph.
     * It's a randomized algorithm that is 'likely' to find the minimal cut
     * in a reasonable time. The algorithm is extended to return the sizes
     * of the two components seperated by the cus as well.
     * This is for education purposes only, very inefficient.
     */
    private static (int size, int c1, int c2) FindCut(string[] input, Random r)
    {
        var graph = Parse(input);
        var componentSize = graph.Keys.ToDictionary(k => k, _ => 1);

        var rebind = (string oldNode, string newNode, string ignoreNode) =>
        {
            foreach (var neighbour in graph[oldNode].ToArray())
            {
                if (neighbour == ignoreNode)
                    continue;

                graph[neighbour].Remove(oldNode);
                graph[neighbour].Add(newNode);
            }
        };

        for (var id = 0; graph.Count > 2; id++)
        {
            /*
             * We decrease the number of nodes and edges by one. First
             * choose a random edge u-v. Introduce a new node that inherits
             * every edge of the nodes u and v (apart from u-v itself) then
             * remove u and v from the graph. It's component size is the sum
             * of the component sizes of u and v.
             */
            var u = graph.Keys.ElementAt(r.Next(graph.Count));
            var v = graph[u][r.Next(graph[u].Count)];

            var merged = $"merge-${id}";
            graph[merged] =
            [
                .. from n in graph[u] where n != v select n,
                .. from n in graph[v] where n != u select n
            ];
            rebind(u, merged, v);
            rebind(v, merged, u);

            graph.Remove(u);
            graph.Remove(v);

            componentSize[merged] = componentSize[u] + componentSize[v];
        }
        
        // at the end we have just two nodes with some edges between them,
        // the number of those edges equals to the size of the cut. Component size
        // tells the number of nodes in the two sides created by the cut.
        var nodeA = graph.Keys.First();
        var nodeB = graph.Keys.Last();
        return (graph[nodeA].Count, componentSize[nodeA], componentSize[nodeB]);
    }
    
    // Returns an adjacency list representation of the input. Edges are recorded
    // both ways, unlike in the input which contains them in one direction only.
    private static Dictionary<string, List<string>> Parse(string[] input)
    {
        var graph = new Dictionary<string, List<string>>();

        var registerEdge = (string u, string v) =>
        {
            if (!graph.ContainsKey(u))
            {
                graph[u] = [];
            }

            graph[u].Add(v);
        };

        foreach (var line in input)
        {
            var parts = line.Split(": ");
            var u = parts[0];
            var nodes = parts[1].Split(' ');
            foreach (var v in nodes)
            {
                registerEdge(u, v);
                registerEdge(v, u);
            }
        }

        return graph;
    }
}