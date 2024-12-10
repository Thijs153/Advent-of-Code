namespace AOC._2024;

using FileSystem = LinkedList<Day09.Block>;
using Node = LinkedListNode<Day09.Block>;

public class Day09
{
    private readonly string _input = File.ReadAllText("Inputs/Day09.txt");

    [Fact]
    public void Part1()
    {
        Checksum(CompactFs(Parse(_input), fragmentsEnabled: true))
            .Should().Be(6323641412437L);
    }
    
    [Fact]
    public void Part2()
    {
        Checksum(CompactFs(Parse(_input), fragmentsEnabled: false))
            .Should().Be(6351801932670L);
    }

    // Moves used blocks of the filesystem towards the beginning of the disk using RelocateBlock.
    private static FileSystem CompactFs(FileSystem fileSystem, bool fragmentsEnabled)
    {
        var (i, j) = (fileSystem.First, fileSystem.Last);
        while (i != j)
        {
            if (i.Value.fileId != -1)
            {
                i = i.Next;
            }
            else if (j.Value.fileId == -1)
            {
                j = j.Previous;
            }
            else
            {
                RelocateBlock(fileSystem, i, j, fragmentsEnabled);
                j = j.Previous;
            }
        }

        return fileSystem;
    }

    /*
     * Relocates the contents of block 'j' to a free space starting after the given node 'start'
     * - Searches for the first suitable free block after 'start'
     * - If a block of equal size is found, 'j' is moved entirely to that block.
     * - If a larger block is found, part of it is used for 'j', and the remainder is split into a new free block.
     * - If a smaller block is found and fragmentation is enabled, a portion of 'j' is moved to fit, leaving the remainder in place.
     */
    private static void RelocateBlock(FileSystem fileSystem, Node start, Node j, bool fragmentsEnabled)
    {
        for (var i = start; i != j; i = i.Next)
        {
            if (i.Value.fileId != -1)
            {
                // noop - not a free space.
            } 
            else if (i.Value.length == j.Value.length)
            {
                (i.Value, j.Value) = (j.Value, i.Value);
                return;
            }
            else if (i.Value.length > j.Value.length)
            {
                var d = i.Value.length - j.Value.length;
                i.Value = j.Value;
                j.Value = j.Value with { fileId = -1 };
                fileSystem.AddAfter(i, new Block(-1, d));
                return;
            }
            else if (i.Value.length < j.Value.length && fragmentsEnabled)
            {
                var d = j.Value.length - i.Value.length;
                i.Value = i.Value with { fileId = j.Value.fileId };
                j.Value = j.Value with { length = d };
                fileSystem.AddAfter(j, i.Value with { fileId = -1 });
            }
        }
    }

    private static long Checksum(FileSystem fileSystem)
    {
        var result = 0L;
        var l = 0;
        for (var i = fileSystem.First; i != null; i = i.Next)
        {
            for (var k = 0; k < i.Value.length; k++)
            {
                if (i.Value.fileId != -1)
                {
                    result += l * i.Value.fileId;
                }

                l++;
            }
        }

        return result;
    }

    private static FileSystem Parse(string input) =>
        new(input.Select((c, i) => new Block(i % 2 == 1 ? -1 : i / 2, c - '0')));

    public record struct Block(int fileId, int length);
}