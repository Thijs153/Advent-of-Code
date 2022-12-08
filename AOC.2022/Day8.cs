using FluentAssertions;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day8
{
    private List<List<int>> _input = new();

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _input = File.ReadLines("Inputs/Day8.txt").Select(x => x.Select(c => c - '0').ToList()).ToList();
    }

    [Test]
    public void Part1()
    {
        int visibleTrees = 0;

        visibleTrees += _input[0].Count * 2; // Setting top and bottom of the grid visible
        visibleTrees += (_input.Count - 2) * 2; // Setting edges of other rows visible

        for (int r = 1; r < _input.Count - 1; r++)
        {
            for (int c = 1; c < _input.Count - 1; c++)
            {
                if (IsVisible(r, c)) visibleTrees++;
            }
        }

        visibleTrees.Should().Be(1812);
    }

    [Test]
    public void Part2()
    {
        int max = 0;

        for (int r = 0; r < _input.Count; r++)
        {
            for (int c = 0; c < _input.Count; c++)
            {
                int score = ScenicScore(r, c);
                if (score > max) max = score;
            }
        }

        max.Should().Be(315495);
    }

    private bool IsVisible(int r, int c)
    {
        bool left = true;
        bool right = true;
        bool top = true;
        bool bottom = true;

        int currentTree = _input[r][c];
        
        // Left
        for (var k = 0; k < c; k++)
        {
            if (currentTree <= _input[r][k])
                left = false;
        }
        if (left) return true;

        // Right
        for (var k = _input.Count - 1; k > c; k--)
        {
            if (currentTree <= _input[r][k]) 
                right = false;
        }
        if (right) return true;
        
        // Top
        for (var k = 0; k < r; k++)
        {
            if (currentTree <= _input[k][c]) top = false;
        }
        if (top) return true;
        
        // Bottom
        for (var k = _input.Count - 1; k > r; k--)
        {
            if (currentTree <= _input[k][c]) bottom = false;
        }
        return bottom;
    }

    private int ScenicScore(int r, int c)
    {
        int left = 0;
        int right = 0;
        int top = 0;
        int bottom = 0;
        
        int currentTree = _input[r][c];
        
        // Left
        for (var k = c - 1; k >= 0; k--)
        {
            left++;
            if (currentTree <= _input[r][k]) break;
        }
        
        // Right
        for (var k = c + 1; k < _input.Count; k++)
        {
            right++;
            if (currentTree <= _input[r][k]) break;
        }
        
        // Top
        for (var k = r - 1; k >= 0; k--)
        {
            top++;
            if (currentTree <= _input[k][c]) break;
        }
        
        // Bottom
        for (var k = r + 1; k < _input.Count; k++)
        {
            bottom++;
            if (currentTree <= _input[k][c]) break;
        }

        return left * right * top * bottom;
    }
    
}