using System.Diagnostics;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day10
{
    private readonly List<string> _input = File.ReadAllLines("Inputs/Day10.txt").ToList();

    [Test]
    public void Part1()
    {
        HashSet<int> cycles = new() { 20, 60, 100, 140, 180, 220 };
        Signal()
            .Where(signal => cycles.Contains(signal.cycle))
            .Select(signal => signal.x * signal.cycle)
            .Sum()
            .Should().Be(12740);
    }

    [Test]
    public void Part2()
    {
        var screen = "";
        foreach (var (cycle, spriteMiddle) in Signal()) {
            var screenColumn = (cycle - 1) % 40;

            screen += Math.Abs(spriteMiddle - screenColumn) < 2 ? "#" : " ";

            if (screenColumn == 39) {
                screen += "\n";
            }
        }

        screen.Should().Be("###  ###  ###   ##  ###   ##   ##  #### \n" + 
                           "#  # #  # #  # #  # #  # #  # #  # #    \n" + 
                           "#  # ###  #  # #  # #  # #  # #    ###  \n" +
                           "###  #  # ###  #### ###  #### # ## #    \n" +
                           "# #  #  # #    #  # # #  #  # #  # #    \n" +
                           "#  # ###  #    #  # #  # #  #  ### #    \n");
    }

    private IEnumerable<(int cycle, int x)> Signal()
    {
        var (cycle, x) = (1, 1);
        foreach (var parts in _input.Select(line => line.Split(" ")))
        {
            if (parts[0] == "noop")
            {
                yield return (cycle++, x);
                continue;
            }
            
            yield return (cycle++, x);
            yield return (cycle++, x);
            x += int.Parse(parts[1]);
        }
    }
    
}

