namespace AOC._2023;

public class Day22
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day22.txt");

    [Fact]
    public void Part1() => Kaboom(_input).Count(x => x == 0).Should().Be(480);

    [Fact]
    public void Part2() => Kaboom(_input).Sum().Should().Be(84021);
    
    // disintegrates the blocks one by one and returns how many blocks would
    // start falling because of that.
    private static IEnumerable<int> Kaboom(string[] input)
    {
        var blocks = Fall(ParseBlocks(input));
        var supports = GetSupports(blocks);

        foreach (var disintegratedBlock in blocks)
        {
            var q = new Queue<Block>();
            q.Enqueue(disintegratedBlock);

            var falling = new HashSet<Block>();
            while (q.TryDequeue(out var block))
            {
                falling.Add(block);

                var blocksStartFalling =
                    from blockT in supports.blocksAbove[block]
                    where supports.blocksBelow[blockT].IsSubsetOf(falling)
                    select blockT;

                foreach (var blockT in blocksStartFalling)
                {
                    q.Enqueue(blockT);
                }
            }

            yield return falling.Count - 1; // -1: disintegratedBlock doesn't count;
        }
    }

    // applies 'gravity' to the blocks.
    private static Block[] Fall(Block[] blocks)
    {
        // sort them in Z first so that we can work in bottom to top order
        blocks = blocks.OrderBy(block => block.Bottom).ToArray();

        for (var i = 0; i < blocks.Length; i++)
        {
            var newBottom = 1;
            for (var j = 0; j < i; j++)
            {
                if (IntersectsXy(blocks[i], blocks[j]))
                {
                    newBottom = Math.Max(newBottom, blocks[j].Top + 1);
                }
            }

            var fall = blocks[i].Bottom - newBottom;
            blocks[i] = blocks[i] with
            {
                z = new Range(blocks[i].Bottom - fall, blocks[i].Top - fall)
            };
        }

        return blocks;
    }
    
    // calculate upper and lower neighbours for each block
    private static Supports GetSupports(Block[] blocks)
    {
        var blocksAbove = blocks.ToDictionary(b => b, _ => new HashSet<Block>());
        var blocksBelow = blocks.ToDictionary(b => b, _ => new HashSet<Block>());

        for (var i = 0; i < blocks.Length; i++)
        {
            for (var j = i + 1; j < blocks.Length; j++)
            {
                var zNeighbours = blocks[j].Bottom == 1 + blocks[i].Top;
                if (zNeighbours && IntersectsXy(blocks[i], blocks[j]))
                {
                    blocksBelow[blocks[j]].Add(blocks[i]);
                    blocksAbove[blocks[i]].Add(blocks[j]);
                }
            }
        }

        return new Supports(blocksAbove, blocksBelow);
    }
    
    private static bool IntersectsXy(Block blockA, Block blockB) =>
        Intersects(blockA.x, blockB.x) && Intersects(blockA.y, blockB.y);
    
    private static bool Intersects(Range r1, Range r2) =>
        r1.begin <= r2.end && r2.begin <= r1.end;

    private static Block[] ParseBlocks(string[] input) => (
        from line in input
        let numbers = line.Split(',', '~').Select(int.Parse).ToArray()
        select new Block(
            x: new Range(numbers[0], numbers[3]),
            y: new Range(numbers[1], numbers[4]),
            z: new Range(numbers[2], numbers[5])
        )
    ).ToArray();

    private record Range(int begin, int end);

    private record Block(Range x, Range y, Range z) {
        public int Top => z.end;
        public int Bottom => z.begin;
    }
    
    private record Supports(
        Dictionary<Block, HashSet<Block>> blocksAbove,
        Dictionary<Block, HashSet<Block>> blocksBelow
    );
}