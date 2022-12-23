using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day22
{
    private readonly string _input = File.ReadAllText("Inputs/Day22.txt");
    
    /*
        The cube is unfolded like this. Each letter identifies an 50x50 square in the input:
                 AB
                 C 
                DE
                F 
        A topology map tells us how cube sides are connected. For example in case of part 1
        the line "A -> B0 C0 B0 E0" means that if we go to the right from A we get to B,
        C is down, moving to the left we find B again, and moving up from A we get to E.
        The order of directions is always right, down, left and up.
        The number next to the letter tells us how many 90 degrees we need to rotate the
        destination square to point upwards. In case of part 1 we don't need to rotate
        so the number is always zero. In part 2 there is "A -> B0 C0 D2 F1" which means that
        if we are about to move up from A we get to F, but F is rotated to the right 1 
        times, likewise D2 means that D is on the left of A and it is up side down.
        This mapping was generated from a paper model.
    */

    [Test]
    public void Part1()
    {
        Solve(
            _input,
            "A -> B0 C0 B0 E0\n" +
            "B -> A0 B0 A0 B0\n" +
            "C -> C0 E0 C0 A0\n" +
            "D -> E0 F0 E0 F0\n" +
            "E -> D0 A0 D0 C0\n" +
            "F -> F0 D0 F0 D0\n"
        ).Should().Be(164014);
    }

    [Test]
    public void Part2()
    {
        Solve(
            _input,
            "A -> B0 C0 D2 F1\n" +
            "B -> E2 C1 A0 F0\n" +
            "C -> B3 E0 D3 A0\n" +
            "D -> E0 F0 A2 C1\n" +
            "E -> B2 F1 D0 C0\n" +
            "F -> E3 B0 A3 D0\n"
        ).Should().Be(47525);
    }
    
    private const int blockSize = 50;
    private const int right = 0;
    private const int down = 1;
    private const int left = 2;
    private const int up = 3;
    
    private static int Solve(string input, string topology)
    {
        var (map, commands) = Parse(input);
        var state = new State("A", new Coord(0, 0), right);

        foreach (var cmd in commands)
        {
            switch (cmd)
            {
                case Left:
                    state = state with { dir = (state.dir + 3) % 4 };
                    break;
                case Right:
                    state = state with { dir = (state.dir + 1) % 4 };
                    break;
                case Forward(var n):
                    for (var i = 0; i < n; i++) {
                        var stateNext = Step(topology, state);
                        var global = ToGlobal(stateNext);
                        if (map[global.iRow][global.iCol] == '.') {
                            state = stateNext;
                        } else {
                            break;
                        }
                    }
                    break;
            }
        }

        return 1000 * (ToGlobal(state).iRow + 1) + 4 * (ToGlobal(state).iCol + 1) + state.dir;
    }
    
    private static Coord ToGlobal(State state) =>
        state.block switch
        {
            "A" => state.coord + new Coord(0, blockSize),
            "B" => state.coord + new Coord(0, 2 * blockSize),
            "C" => state.coord + new Coord(blockSize, blockSize),
            "D" => state.coord + new Coord(2 * blockSize, 0),
            "E" => state.coord + new Coord(2 * blockSize, blockSize),
            "F" => state.coord + new Coord(3 * blockSize, 0),
            _ => throw new Exception()
        };

    private static State Step(string topology, State state)
    {
        bool wrapsAround(Coord coord) =>
            coord.iCol is < 0 or >= blockSize || coord.iRow is < 0 or >= blockSize;

        var (srcBlock, coord, dir) = state;
        var dstBlock = srcBlock;
        
        // take one step, if there is no wrap around we are alright
        coord = dir switch {
            left => coord with { iCol = coord.iCol - 1 },
            down => coord with { iRow = coord.iRow + 1 },
            right => coord with { iCol = coord.iCol + 1 },
            up => coord with { iRow = coord.iRow - 1 },
            _ => throw new Exception()
        };

        if (wrapsAround(coord))
        {
            // check the topology, select the dstBlock and rotate coord and dir as much as needed
            // ex. if srcBlock: "C", dir: 2

            var line = topology.Split('\n').Single(x => x.StartsWith(srcBlock));
            // line: C -> B3 E0 D3 A0

            var mapping = line.Split(" -> ")[1].Split(" ");
            // mapping: [B3, E0, D3, A0]

            var neighbour = mapping[dir];
            // neighbour: D3

            dstBlock = neighbour[..1];
            // dstBlock: D;

            var rotate = int.Parse(neighbour[1..]);
            // rotate: 3
            
            // go back to the 0..49 range first, then rotate as much as needed
            coord = coord with
            {
                iRow = (coord.iRow + blockSize) % blockSize,
                iCol = (coord.iCol + blockSize) % blockSize,
            };

            for (var i = 0; i < rotate; i++)
            {
                coord = coord with { iRow = coord.iCol, iCol = blockSize - coord.iRow - 1 };
                dir = (dir + 1) % 4;
            }
        }

        return new State(dstBlock, coord, dir);
    }
    
    private static (string[] map, Cmd[] path) Parse(string input)
    {
        input = input.ReplaceLineEndings("\n"); // replacing "\r\n" with "\n"
        var blocks = input.Split("\n\n");
        var map = blocks[0].Split("\n");
        var commands = Regex
            .Matches(blocks[1], @"(\d+)|L|R")
            .Select<Match, Cmd>(m =>
                m.Value switch
                {
                    "L" => new Left(),
                    "R" => new Right(),
                    string n => new Forward(int.Parse(n))
                })
            .ToArray();

        return (map, commands);
    }

    private record State(string block, Coord coord, int dir);
    
    private record Coord(int iRow, int iCol)
    {
        public static Coord operator +(Coord a, Coord b) =>
            new(a.iRow + b.iRow, a.iCol + b.iCol);

        public static Coord operator -(Coord a, Coord b) =>
            new(a.iRow - b.iRow, a.iCol - b.iCol);
    }
    
    private interface Cmd {}

    private record Forward(int n) : Cmd;

    private record Right : Cmd;

    private record Left : Cmd;
}