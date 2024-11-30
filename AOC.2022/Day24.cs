namespace AOC._2022;

public class Day24
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day24.txt");

    /*
     * We do a standard A* algorithm, the only trick is that
     * the 'map' always changes as blizzards move, so our position
     * is now a space-time coordinate.
     * I used an efficient 'Maps' class that can be queried with these.
     */
    
    [Fact]
    public void Part1()
    {
        var (entry, exit, maps) = Parse(_input);
        var pos = WalkTo(entry, exit, maps);
        
        pos.time.Should().Be(305);
    }

    [Fact]
    public void Part2()
    {
        var (entry, exit, maps) = Parse(_input);
        var pos = WalkTo(entry, exit, maps);
        pos = WalkTo(pos, entry, maps);
        pos = WalkTo(pos, exit, maps);

        pos.time.Should().Be(905);
    }
    
    // Standard A* algorithm
    private static Pos WalkTo(Pos start, Pos goal, Maps maps)
    {
        var q = new PriorityQueue<Pos, int>();

        int f(Pos pos)
        {
            // estimate the remaining step count with Manhattan distance
            var dist =
                Math.Abs(goal.iRow - pos.iRow) +
                Math.Abs(goal.iCol - pos.iCol);
            return pos.time + dist;
        }
        
        q.Enqueue(start, f(start));
        HashSet<Pos> seen = new();

        while (q.Count > 0)
        {
            var pos = q.Dequeue();
            
            if (pos.iRow == goal.iRow && pos.iCol == goal.iCol)
                return pos;

            foreach (var nextPos in NextPositions(pos, maps))
            {
                if (seen.Contains(nextPos)) 
                    continue;
                
                seen.Add(nextPos);
                q.Enqueue(nextPos, f(nextPos));
            }
        }

        throw new Exception();
    }
    
    // Increase time, look for free neighbours
    private static IEnumerable<Pos> NextPositions(Pos pos, Maps maps)
    {
        pos = pos with { time = pos.time + 1 };
        foreach (var nextPos in new Pos[] {
             pos,
             pos with {iRow = pos.iRow -1 },
             pos with {iRow = pos.iRow +1 },
             pos with {iCol = pos.iCol -1 },
             pos with {iCol = pos.iCol +1 },
         })
        {
            if (maps.Get(nextPos) == '.')
            {
                yield return nextPos;
            }
        }
    }

    private static (Pos entry, Pos exit, Maps maps) Parse(string[] input)
    {
        var maps = new Maps(input);
        var entry = new Pos(0, 0, 1);
        var exit = new Pos(int.MaxValue, maps.cRow - 1, maps.cCol - 2);
        return (entry, exit, maps);
    }
    
    private record Pos(int time, int iRow, int iCol);
    
    private class Maps
    {
        private readonly string[] map;
        public readonly int cRow;
        public readonly int cCol;

        public Maps(string[] input)
        {
            map = input;
            cRow = map.Length;
            cCol = map[0].Length;
        }

        public char Get(Pos pos)
        {
            if (pos.iRow == 0 && pos.iCol == 1)
            {
                return '.';
            }

            if (pos.iRow == cRow - 1 && pos.iCol == cCol - 2)
            {
                return '.';
            }
            
            if (pos.iRow <= 0 || pos.iRow >= cRow - 1 || 
                pos.iCol <= 0 || pos.iCol >= cCol - 1
            ) 
            {
                return '#';
            }
            
            // blizzards have a horizontal and a vertical loop
            // it's easy to check the original positions with going back in time
            // using modular arithmetic
            var hMod = cCol - 2;
            var vMod = cRow - 2;
            
            var iColW = (pos.iCol - 1 + hMod - (pos.time % hMod)) % hMod + 1;
            var iColE = (pos.iCol - 1 + hMod + (pos.time % hMod)) % hMod + 1;
            var iColN = (pos.iRow - 1 + vMod - (pos.time % vMod)) % vMod + 1;
            var iColS = (pos.iRow - 1 + vMod + (pos.time % vMod)) % vMod + 1;
            
            return 
                map[pos.iRow][iColW] == '>' ? '>':
                map[pos.iRow][iColE] == '<' ? '<':
                map[iColN][pos.iCol] == 'v' ? 'v':
                map[iColS][pos.iCol] == '^' ? '^':
                '.';
        }
    }
}

