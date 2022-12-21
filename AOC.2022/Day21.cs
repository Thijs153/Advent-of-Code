using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using NUnit.Framework;

namespace AOC._2022;

[TestFixture]
public class Day21
{

    private readonly string[] _input = File.ReadAllLines("Inputs/Day21.txt");
    
    [Test]
    public void Part1()
    {
        Parse(_input, "root", false)
            .Simplify()
            .ToString()
            .Should().Be("282285213953670");
    }

    [Test]
    public void Part2()
    {
        var expr = Parse(_input, "root", true) as Eq;
        while (expr?.Left is not Var)
        {
            expr = Solve(expr!);
        }

        expr.Right.ToString().Should().Be("3699945358564");
    }
    

    // One step in rearranging the equation to <variable> = <constant> form.
    // It is supposed that there is only one variable occurrence in the whole
    // expression tree.
    private static Eq Solve(Eq eq) =>
        eq.Left switch {
            Op(Const l, "+", Expr r) => new Eq(r, new Op(eq.Right, "-", l).Simplify()),
            Op(Const l, "*", Expr r) => new Eq(r, new Op(eq.Right, "/", l).Simplify()),
            Op(Expr  l, "+", Expr r) => new Eq(l, new Op(eq.Right, "-", r).Simplify()),
            Op(Expr  l, "-", Expr r) => new Eq(l, new Op(eq.Right, "+", r).Simplify()),
            Op(Expr  l, "*", Expr r) => new Eq(l, new Op(eq.Right, "/", r).Simplify()),
            Op(Expr  l, "/", Expr r) => new Eq(l, new Op(eq.Right, "*", r).Simplify()),
            Const                    => new Eq(eq.Right, eq.Left),
            _ => eq
        };
    
    private static Expr Parse(string[] input, string name, bool part2)
    {
        var context = new Dictionary<string, string[]>();
        foreach (var line in input)
        {
            var parts = line.Split(" ");
            context[parts[0].TrimEnd(':')] = parts.Skip(1).ToArray();
        }

        Expr buildExpr(string name)
        {
            var parts = context[name];
            if (part2)
            {
                if (name == "humn")
                    return new Var("humn");
                
                if (name == "root")
                    return new Eq(buildExpr(parts[0]), buildExpr(parts[2]));
            }

            if (parts.Length == 1)
                return new Const(long.Parse(parts[0]));
            
            return new Op(buildExpr(parts[0]), parts[1], buildExpr(parts[2]));
        }

        return buildExpr(name);
    }

    private interface Expr
    {
        Expr Simplify();
    }

    private record Const(long Value) : Expr
    {
        public override string ToString() => Value.ToString();
        public Expr Simplify() => this;
    }

    private record Var(string Name) : Expr
    {
        public override string ToString() => Name;
        public Expr Simplify() => this;
    }

    private record Eq(Expr Left, Expr Right) : Expr
    {
        public override string ToString() => $"{Left} == {Right}";
        public Expr Simplify() => new Eq(Left.Simplify(), Right.Simplify());
    }

    private record Op(Expr Left, string op, Expr Right) : Expr
    {
        public override string ToString() => $"({Left}) {op} ({Right})";

        public Expr Simplify()
        {
            return (Left.Simplify(), op, Right.Simplify()) switch
            {
                (Const l, "+", Const r) => new Const(l.Value + r.Value),
                (Const l, "-", Const r) => new Const(l.Value - r.Value),
                (Const l, "*", Const r) => new Const(l.Value * r.Value),
                (Const l, "/", Const r) => new Const(l.Value / r.Value),
                (Expr l, _, Expr r) => new Op(l, op, r),
            };
        }
    } 
}