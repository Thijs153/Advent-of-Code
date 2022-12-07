using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

public class Day7
{
    private Dir _currentDir = default!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        foreach (string line in File.ReadLines("Inputs/Day7.txt"))
        {
            if (line == "$ ls") continue;
            if (line.StartsWith("$ cd"))
            {
                if (line == "$ cd /")
                {
                    _currentDir = Dir.Root;
                } 
                else if (line == "$ cd ..")
                {
                    _currentDir = _currentDir.Parent;
                }
                else
                {
                    _currentDir = GetOrCreateDir(line.Split(" ")[2]);
                }
            }
            else if (line.StartsWith("dir"))
            {
                GetOrCreateDir(line.Split(" ")[1]);
            }
            else
            {
                _currentDir.Files.Add(new F(line.Split(" ")[1], int.Parse(line.Split(" ")[0])));
            }
        }
    }
    
    [Test]
    public void Part1()
    {
        GetTotalDirs(Dir.Root).Should().Be(1555642);
    }

    [Test]
    public void Part2()
    {
        var freeSpace = 70000000 - Dir.Root.Size();
        var spaceToFree = 30000000 - freeSpace;

        GetAllDirs(Dir.Root)
            .OrderBy(d => d.Size())
            .First(x => x.Size() > spaceToFree)
            .Size()
            .Should().Be(5974547);
    }

    private static int GetTotalDirs(Dir d) => (d.Size() <= 100000 ? d.Size() : 0) + d.Dirs.Sum(GetTotalDirs);

    private static IEnumerable<Dir> GetAllDirs(Dir d)
    {
        foreach (Dir subDir in d.Dirs.SelectMany(GetAllDirs))
        {
            yield return subDir;
        }

        yield return d;
    }

    private Dir GetOrCreateDir(string dirName)
    {
        Dir? newDir = _currentDir.Dirs.FirstOrDefault(x => x.Name == dirName);

        if (newDir is null)
        {
            newDir = new Dir(dirName, _currentDir);
            _currentDir.Dirs.Add(newDir);
        }

        return newDir;
    }
    
    internal record Dir(string Name, IList<F> Files, IList<Dir> Dirs, Dir Parent)
    {
        public static readonly Dir Root = new("/", null);
        public Dir(string name, Dir parent) : this(name, new List<F>(), new List<Dir>(), parent) { }
        public int Size() => Files.Sum(x => x.Size) + Dirs.Sum(x => x.Size());
    };

    internal record F(string Name, int Size);
}