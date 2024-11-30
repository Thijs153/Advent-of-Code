namespace AOC._2021;

public class Day18
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day18.txt");

    [Fact]
    public void Part1()
    {
        _input.Select(ParseNumber)
            .Aggregate(
                new Number(),
                (acc, number) => !acc.Any() ? number : Sum(acc, number),
                Magnitude
            )
            .Should().Be(4008L);
    }

    [Fact]
    public void Part2()
    {
        var numbers = _input.Select(ParseNumber).ToArray();
        (
            from i in Enumerable.Range(0, numbers.Length)
            from j in Enumerable.Range(0, numbers.Length)
            where i != j
            select Magnitude(Sum(numbers[i], numbers[j]))
        ).Max().Should().Be(4667L);
    }

    private static long Magnitude(Number number) {
        var iToken = 0;

        long ComputeRecursive() 
        {
            var token = number[iToken++];
            if (token.kind == TokenKind.Digit) 
            {
                // just the number
                return token.value;
            }

            // take left and right side of the pair
            var left = ComputeRecursive();
            var right = ComputeRecursive();
            iToken++; // don't forget to eat the closing parenthesis
            return 3 * left + 2 * right;
        }

        return ComputeRecursive();
    }


    // just wrap A and B in a new 'number' and reduce:
    private static Number Sum(Number numberA, Number numberB) => Reduce(Number.Pair(numberA, numberB));

    private static Number Reduce(Number number) 
    {
        while (Explode(number) || Split(number)) 
        { 
            // repeat until we cannot explode or split anymore
        }
        return number;
    }

    private static bool Explode(Number number) 
    {
        // exploding means we need to find the first pair in the number 
        // that is embedded in 4 other pairs and get rid of it:
        var depth = 0;
        for (var i = 0; i < number.Count; i++) 
        {
            if (number[i].kind == TokenKind.Open) {
                depth++;
                if (depth == 5) 
                {
                    // we are deep enough, let's to the reduce part

                    // find the digit to the left (if any) and increase:
                    for (var j = i - 1; j >= 0; j--)
                    {
                        if (number[j].kind != TokenKind.Digit) continue;
                        
                        number[j] = number[j] with { value = number[j].value + number[i + 1].value };
                        break;
                    }

                    // find the digit to the right (if any) and increase:
                    for (var j = i + 3; j < number.Count; j++)
                    {
                        if (number[j].kind != TokenKind.Digit) continue;
                        
                        number[j] = number[j] with { value = number[j].value + number[i + 2].value };
                        break;
                    }

                    // replace [a b] with 0:
                    number.RemoveRange(i, 4);
                    number.Insert(i, new Token(TokenKind.Digit));

                    // successful reduce:
                    return true;
                }
            } 
            else if (number[i].kind == TokenKind.Close) 
            {
                depth--;
            }
        }

        // couldn't reduce:
        return false;
    }

    private static bool Split(Number number) 
    {

        // splitting means we need to find a token with a high value and make a pair out of it:
        for (var i = 0; i < number.Count; i++) 
        {
            if (number[i].value >= 10) 
            {

                var v = number[i].value;
                number.RemoveRange(i, 1);
                number.InsertRange(i, Number.Pair(Number.Digit(v / 2), Number.Digit((v + 1) / 2)));

                // successful split:
                return true;
            }
        }
        // couldn't split:
        return false;
    }

    // tokenize the input to a list of '[' ']' and digit tokens
    private static Number ParseNumber(string st) 
    {
        var res = new Number();
        var n = "";
        foreach (var ch in st) {
            if (ch is >= '0' and <= '9') {
                n += ch;
            } else {
                if (n != "") {
                    res.Add(new Token(TokenKind.Digit, int.Parse(n)));
                    n = "";
                }
                if (ch == '[') {
                    res.Add(new Token(TokenKind.Open));
                } else if (ch == ']') {
                    res.Add(new Token(TokenKind.Close));
                }
            }
        }
        if (n != "") {
            res.Add(new Token(TokenKind.Digit, int.Parse(n)));
        }
        
        return res;
    }
    
    private enum TokenKind
    {
        Open,
        Close,
        Digit
    }

    private record Token(TokenKind kind, int value = 0);

    private class Number : List<Token>
    {
        public static Number Digit(int value) =>
            [new Token(TokenKind.Digit, value)];

        public static Number Pair(Number a, Number b)
        {
            var number = new Number { new Token(TokenKind.Open) };
            number.AddRange(a);
            number.AddRange(b);
            number.Add(new Token(TokenKind.Close));
            return number;
        }
    }
    
}