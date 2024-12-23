namespace AOC._2024;

using Graph = Dictionary<string, HashSet<string>>;
using Component = string;

public class Day23
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day23.txt");

    [Fact]
    public void Part1()
    {
        var graph = GetGraph(_input);
        var components = GetSeed(graph);
        components = Grow(graph, components);
        components = Grow(graph, components);

        components.Count(c => Members(c).Any(m => m.StartsWith("t")))
            .Should().Be(1467);
    }

    [Fact]
    public void Part2()
    {
        var graph = GetGraph(_input);
        var components = GetSeed(graph);

        while (components.Count > 1)
        {
            components = Grow(graph, components);
        }

        components.Single().Should().Be("di,gs,jw,kz,md,nc,qp,rp,sa,ss,uk,xk,yn");
    }
    
    private static HashSet<Component> GetSeed(Graph g) => g.Keys.ToHashSet();
    
    private static HashSet<Component> Grow(Graph g, HashSet<Component> components) => (
        from c in components.AsParallel()
        let members = Members(c)
        from neighbour in members.SelectMany(m => g[m]).Distinct()
        where !members.Contains(neighbour)
        where members.All(m => g[neighbour].Contains(m))
        select Extend(c, neighbour)
    ).ToHashSet();
    
    private static Component Extend(Component c, string item) =>
        string.Join(",", Members(c).Append(item).OrderBy(x => x));

    private static IEnumerable<string> Members(Component c) => c.Split(",");
    
    private static Graph GetGraph(string[] input)
    {
        var edges =
            from line in input
            let nodes = line.Split("-")
            from edge in new[] { (nodes[0], nodes[1]), (nodes[1], nodes[0]) }
            select (From: edge.Item1, To: edge.Item2);

        return (
            from e in edges
            group e by e.From
            into g
            select (g.Key, g.Select(e => e.To).ToHashSet())
        ).ToDictionary();
    }
}