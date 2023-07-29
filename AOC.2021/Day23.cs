using FluentAssertions;
using NUnit.Framework;

namespace AOC._2021;

[TestFixture]
public class Day23
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day23.txt");

    [Test]
    public void Part1()
    {
        Solve(_input).Should().Be(11516);
    }

    [Test]
    public void Part2()
    {
        Solve(Upscale(_input)).Should().Be(40272);
    }

    private static string[] Upscale(string[] input)
    {
        var lines = input.ToList();
        lines.Insert(3, "  #D#C#B#A#");
        lines.Insert(4, "  #D#B#A#C#");

        return lines.ToArray();
    }

    private int Solve(string[] input)
    {
        var maze = Maze.Parse(input);

        var q = new PriorityQueue<Maze, int>();
        var cost = new Dictionary<Maze, int>();
        
        q.Enqueue(maze, 0);
        cost.Add(maze, 0);

        while (q.Count > 0)
        {
            maze = q.Dequeue();

            if (maze.Finished())
            {
                return cost[maze];
            }

            foreach (var n in Neighbours(maze))
            {
                if (cost[maze] + n.cost < cost.GetValueOrDefault(n.maze, int.MaxValue))
                {
                    cost[n.maze] = cost[maze] + n.cost;
                    q.Enqueue(n.maze, cost[n.maze]);
                }
            }
        }

        throw new Exception();
    }

    private static int StepCost(char actor) =>
        actor switch
        {
            'A' => 1,
            'B' => 10,
            'C' => 100,
            _ => 1000
        };

    private static int GetIColDst(char ch) =>
        ch switch
        {
            'A' => 3,
            'B' => 5,
            'C' => 7,
            'D' => 9,
            _ => throw new Exception()
        };
    
    private (Maze maze, int cost) HallwayToRoom(Maze maze) {
        for (var iCol = 1; iCol < 12; iCol++) {
            var ch = maze.ItemAt(new Point(1, iCol));

            if (ch == '.') {
                continue;
            }

            var iColDst = GetIColDst(ch);

            if (!maze.CanMoveToDoor(iCol, iColDst) || !maze.CanEnterRoom(ch))
            {
                continue;
            }
            
            var steps = Math.Abs(iColDst - iCol);
            var pt = new Point(1, iColDst);

            while (maze.ItemAt(pt.Below) == '.') {
                pt = pt.Below;
                steps++;
            }

            var l = HallwayToRoom(maze.Move(new Point(1, iCol), pt));
            return (l.maze, l.cost + steps * StepCost(ch));
        }
        return (maze, 0);
    }
    
    private static IEnumerable<(Maze maze, int cost)> RoomToHallway(Maze maze) {
        var hallwayColumns = new[] { 1, 2, 4, 6, 8, 10, 11 };

        foreach (var roomColumn in new[] { 3, 5, 7, 9 }) {

            if (maze.FinishedColumn(roomColumn)) {
                continue;
            }

            var stepsV = 0;
            var ptSrc = new Point(1, roomColumn);
            while (maze.ItemAt(ptSrc) == '.') {
                ptSrc = ptSrc.Below;
                stepsV++;
            }

            var ch = maze.ItemAt(ptSrc);
            if (ch == '#') {
                continue;
            }

            foreach (var dj in new[] { -1, 1 }) {
                var stepsH = 0;
                var ptDst = new Point(1, roomColumn);
                while (maze.ItemAt(ptDst) == '.') {

                    if (hallwayColumns.Contains(ptDst.iCol)) {
                        yield return (maze.Move(ptSrc, ptDst), (stepsV + stepsH) * StepCost(ch));
                    }

                    ptDst = dj == -1 
                        ? ptDst.Left 
                        : ptDst.Right;
                    
                    stepsH++;
                }
            }
        }
    }

    private IEnumerable<(Maze maze, int cost)> Neighbours(Maze maze) {
        var hallwayToRoom = HallwayToRoom(maze);
        return hallwayToRoom.cost != 0 ? new[] { hallwayToRoom } : RoomToHallway(maze);
    }
    
    private record Point(int iRow, int iCol)
    {
        public Point Below => this with { iRow = iRow + 1 };
        public Point Above => this with { iRow = iRow - 1 };
        public Point Left => this with { iCol = iCol - 1 };
        public Point Right => this with { iCol = iCol + 1 };
    }

    private record Maze(int a, int b, int c, int d)
    {
        private const int ColumnMaskA = (1 << 11) | (1 << 15) | (1 << 19) | (1 << 23);
        private const int ColumnMaskB = (1 << 12) | (1 << 16) | (1 << 20) | (1 << 24);
        private const int ColumnMaskC = (1 << 13) | (1 << 17) | (1 << 21) | (1 << 25);
        private const int ColumnMaskD = (1 << 14) | (1 << 18) | (1 << 22) | (1 << 26);
        
        public static Maze Parse(string[] map)
        {
            var maze = new Maze(ColumnMaskA, ColumnMaskB, ColumnMaskC, ColumnMaskD);
            foreach (var iRow in Enumerable.Range(0, map.Length))
            {
                foreach (var iCol in Enumerable.Range(0, map[0].Length))
                {
                    maze = maze.SetItem(new(iRow, iCol),
                        iRow < map.Length && iCol < map[iRow].Length ? map[iRow][iCol] : '#');
                }
            }

            return maze;
        }
        private static int BitFromPoint(Point pt) =>
            (pt.iRow, pt.iCol) switch
            {
                (1, 1) => 1 << 0,
                (1, 2) => 1 << 1,
                (1, 3) => 1 << 2,
                (1, 4) => 1 << 3,
                (1, 5) => 1 << 4,
                (1, 6) => 1 << 5,
                (1, 7) => 1 << 6,
                (1, 8) => 1 << 7,
                (1, 9) => 1 << 8,
                (1, 10) => 1 << 9,
                (1, 11) => 1 << 10,

                (2, 3) => 1 << 11,
                (2, 5) => 1 << 12,
                (2, 7) => 1 << 13,
                (2, 9) => 1 << 14,

                (3, 3) => 1 << 15,
                (3, 5) => 1 << 16,
                (3, 7) => 1 << 17,
                (3, 9) => 1 << 18,

                (4, 3) => 1 << 19,
                (4, 5) => 1 << 20,
                (4, 7) => 1 << 21,
                (4, 9) => 1 << 22,

                (5, 3) => 1 << 23,
                (5, 5) => 1 << 24,
                (5, 7) => 1 << 25,
                (5, 9) => 1 << 26,

                _ => 1 << 31,
            };

        public bool CanEnterRoom(char ch) =>
            ch switch
            {
                'A' => (b & ColumnMaskA) == 0 && (c & ColumnMaskA) == 0 && (d & ColumnMaskA) == 0,
                'B' => (a & ColumnMaskB) == 0 && (c & ColumnMaskB) == 0 && (d & ColumnMaskB) == 0,
                'C' => (a & ColumnMaskC) == 0 && (b & ColumnMaskC) == 0 && (d & ColumnMaskC) == 0,
                'D' => (a & ColumnMaskD) == 0 && (b & ColumnMaskD) == 0 && (c & ColumnMaskD) == 0,
                _ => throw new Exception()
            };

        public bool CanMoveToDoor(int iColFrom, int iColTo)
        {
            Point Step(Point pt)
            {
                return iColFrom < iColTo ? pt.Right : pt.Left;
            }

            var pt = Step(new Point(1, iColFrom));
            while (pt.iCol != iColTo)
            {
                if (ItemAt(pt) != '.')
                {
                    return false;
                }

                pt = Step(pt);
            }

            return true;
        }

        public bool FinishedColumn(int iCol) =>
            iCol switch
            {
                3 => a == ColumnMaskA,
                5 => b == ColumnMaskB,
                7 => c == ColumnMaskC,
                9 => d == ColumnMaskD,
                _ => throw new Exception()
            };

        public bool Finished() =>
            FinishedColumn(3) && FinishedColumn(5) && FinishedColumn(7) && FinishedColumn(9);

        public char ItemAt(Point pt)
        {
            var bit = BitFromPoint(pt);
            
            return bit == 1 << 31 ? '#' :
                (a & bit) != 0 ? 'A' :
                (b & bit) != 0 ? 'B' :
                (c & bit) != 0 ? 'C' :
                (d & bit) != 0 ? 'D' :
                '.';
        }

        public Maze Move(Point from, Point to) =>
            SetItem(to, ItemAt(from)).SetItem(from, '.');

        private Maze SetItem(Point pt, char ch)
        {
            if (ch == '#')
            {
                return this;
            }

            var bit = BitFromPoint(pt);
            if (bit == 1 << 31)
            {
                return this;
            }
            
            return ch switch {
                '.' => new Maze(
                    a & ~bit,
                    b & ~bit,
                    c & ~bit,
                    d & ~bit
                ),
                'A' => new Maze(a | bit, b & ~bit, c & ~bit, d & ~bit),
                'B' => new Maze(a & ~bit, b | bit, c & ~bit, d & ~bit),
                'C' => new Maze(a & ~bit, b & ~bit, c | bit, d & ~bit),
                'D' => new Maze(a & ~bit, b & ~bit, c & ~bit, d | bit),
                _ => throw new Exception()
            };
        }
    }
}