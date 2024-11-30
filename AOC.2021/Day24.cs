namespace AOC._2021;

public class Day24
{
    private readonly string _input = File.ReadAllText("Inputs/Day24.txt").ReplaceLineEndings("\n");

    [Fact]
    public void Part1()
    {
        GetSerials(_input).max.Should().Be("29991993698469");
    }

    [Fact]
    public void Part2()
    {
        GetSerials(_input).min.Should().Be("14691271141118");
    }

    private static (string min, string max) GetSerials(string input) {

        var digits = Enumerable.Range(1, 9).ToArray();

        // The input has 14 code blocks, each dealing with one digit.
        // The blocks define 7 pairs of `a`, `b` digits and a `shift` between them.
        // The input is valid if for each pair the condition `a + shift = b` holds.
        var stmBlocks = input.Split("inp w\n")[1..]; 

        // Extracts the numeric argument of a statement:
        int GetArgFromLine(int iBlock, Index iLine) => int.Parse(stmBlocks[iBlock].Split('\n')[iLine].Split(' ')[^1]);

        // A stack will contain the index of an `a` digit when we find its corresponding `b`.
        var stack = new Stack<int>();
       
        // We will fill up the result when `b` is found.
        var max = Enumerable.Repeat(int.MinValue, 14).ToArray();
        var min = Enumerable.Repeat(int.MaxValue, 14).ToArray();
        
        for (var j = 0; j < 14; j++) {
            if (stmBlocks[j].Contains("div z 1")) { 
                // j points to an `a` digit.
                stack.Push(j);
            } else { 
                // j points to a `b` digit. 
              
                // `a` is at i.
                var i = stack.Pop(); 

                // A part of shift is hidden in each of the two blocks:
                var shift = GetArgFromLine(i, ^4) + GetArgFromLine(j, 4);

                // Find the best a and b so that the equation holds
                foreach (var a in digits) {

                    var b = a + shift;

                    if (digits.Contains(b)) {
                        if (a > max[i]) {
                            (max[i], max[j]) = (a, b);
                        }
                        if (a < min[i]) {
                            (min[i], min[j]) = (a, b);
                        }
                    }
                }
            }
        }

        // That's all folks
        return (string.Join("", min), string.Join("", max));
    }
}