namespace AOC._2022;

public class Day17
{
    private readonly string _input = File.ReadAllText("Inputs/Day17.txt");

    [Fact]
    public void Part1()
    {
        new Tunnel(_input)
            .AddRocks(2022).Height
            .Should().Be(3179);
    }

    [Fact]
    public void Part2()
    {
        new Tunnel(_input)
            .AddRocks(1000000000000).Height
            .Should().Be(1567723342929);
    }
}

internal class Tunnel
{
    private const int LinesToStore = 100;
    private readonly List<string> _lines;
    private long _linesNotStored;

    public long Height => _lines.Count + _linesNotStored - 1;

    private readonly IEnumerator<string[]> _rocks;
    private readonly IEnumerator<char> _jets;

    public Tunnel(string jets)
    {
        var rocks = new[]{
            "####".Split("\n"),
            " # \n###\n # ".Split("\n"),
            "  #\n  #\n###".Split("\n"),
            "#\n#\n#\n#".Split("\n"),
            "##\n##".Split("\n")
        };

        _rocks = Loop(rocks).GetEnumerator();
        _jets = Loop(jets.Trim()).GetEnumerator();
        _lines = new List<string>() { "+-------+" };
    }

    public Tunnel AddRocks(long rocks)
    {
        /*
         * We are adding rocks one by one until we find a recurring pattern
         *
         * Then we can jump forward full periods with just increasing the height
         * of the cave: the top of the cave should look the same after a full period
         * so no need to simulate the rocks anymore.
         *
         * Then we just add the remaining rocks.
         */

        var seen = new Dictionary<string, (long rocks, long height)>();
        while (rocks > 0)
        {
            var hash = string.Join("\n", _lines);
            if (seen.TryGetValue(hash, out var cache))
            {
                // we have seen this pattern.
                // compute length of the period, and how much does it
                // add to the height of the cave:
                var heightOfPeriod = Height - cache.height;
                var periodLenght = cache.rocks - rocks;
                
                // advance forward as much as possible
                _linesNotStored += (rocks / periodLenght) * heightOfPeriod;
                rocks %= periodLenght;
                break;
            }

            seen[hash] = (rocks, Height);
            AddRock();
            rocks--;
        }

        while (rocks > 0)
        {
            AddRock();
            rocks--;
        }

        return this;
    }

    private void AddRock()
    {
        // Adds one rock to the cave
        _rocks.MoveNext();
        var rock = _rocks.Current;
        
        // make room: 3 lines + the height of the rock
        for (var i = 0; i < rock.Length + 3; i++)
        {
            _lines.Insert(0, "|       |");
        }
        
        // simulate falling
        var (rockX, rockY) = (3, 0);
        while (true)
        {
            _jets.MoveNext();
            if (_jets.Current == '>' && !Hit(rock, rockX + 1, rockY))
            {
                rockX++;
            }
            else if (_jets.Current == '<' && !Hit(rock, rockX - 1, rockY))
            {
                rockX--;
            }
            
            if (Hit(rock, rockX, rockY + 1))
                break;

            rockY++;
        }

        Draw(rock, rockX, rockY);
    }

    private bool Hit(IReadOnlyList<string> rock, int x, int y)
    {
        // tells if a rock hits the walls of the cave or another rock

        var (crow, ccol) = (rock.Count, rock[0].Length);
        for (var irow = 0; irow < crow; irow++)
        {
            for (var icol = 0; icol < ccol; icol++) {
                if (rock[irow][icol] == '#' && _lines[irow + y][icol + x] != ' ') {
                    return true;
                }
            }
        }

        return false;
    }

    private void Draw(IReadOnlyList<string> rock, int rockX, int rockY)
    {
        // draws a rock pattern into the cave at the given x,y coordinates

        var (crow, ccol) = (rock.Count, rock[0].Length);
        for (var irow = 0; irow < crow; irow++)
        {
            var chars = _lines[irow + rockY].ToArray();
            for (var icol = 0; icol < ccol; icol++)
            {
                if (rock[irow][icol] == '#') {
                    if (chars[icol + rockX] != ' ') {
                        throw new Exception();
                    }
                    chars[icol + rockX] = '#';
                }
            }

            _lines[rockY + irow] = string.Join("", chars);
        }
        
        // remove empty lines from the top
        while (!_lines[0].Contains('#'))
        {
            _lines.RemoveAt(0);
        }
        
        // keep the tail
        if (_lines.Count > LinesToStore)
        {
            var r = _lines.Count - LinesToStore - 1;
            _lines.RemoveRange(LinesToStore, r);
            _linesNotStored += r;
        }
    }

    private static IEnumerable<T> Loop<T>(IEnumerable<T> items) {
        while (true) {
            foreach (var item in items) {
                yield return item;
            }
        }
    }
}