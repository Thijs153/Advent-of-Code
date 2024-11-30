namespace AOC._2022;

public class Day08
{
    private readonly List<List<int>> _input = File.ReadLines("Inputs/Day08.txt").Select(x => x.Select(c => c - '0').ToList()).ToList();

    [Fact]
    public void Part1()
    {
        var visibleTrees = 0;

        visibleTrees += _input[0].Count * 2; // Setting top and bottom of the grid visible
        visibleTrees += (_input.Count - 2) * 2; // Setting edges of other rows visible

        for (var r = 1; r < _input.Count - 1; r++)
        {
            for (var c = 1; c < _input.Count - 1; c++)
            {
                if (IsVisible(r, c)) visibleTrees++;
            }
        }

        visibleTrees.Should().Be(1812);
    }

    [Fact]
    public void Part2()
    {
        var max = 0;

        for (var r = 0; r < _input.Count; r++)
        {
            for (var c = 0; c < _input.Count; c++)
            {
                var score = ScenicScore(r, c);
                if (score > max) max = score;
            }
        }

        max.Should().Be(315495);
    }

    private bool IsVisible(int r, int c)
    {
        var left = true;
        var right = true;
        var top = true;
        var bottom = true;

        var currentTree = _input[r][c];
        
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
        var left = 0;
        var right = 0;
        var top = 0;
        var bottom = 0;
        
        var currentTree = _input[r][c];
        
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