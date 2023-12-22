using System.Numerics;
using FluentAssertions;
using NUnit.Framework;

namespace AOC._2023;

public class Day21
{
    private readonly string[] _input = File.ReadAllLines("Inputs/Day21.txt");

    [Test]
    public void Part1()
    {
        var map = ParseMap(_input);
        var positions = new HashSet<Complex> { Center };

        for (var i = 0; i < 64; i++)
        {
            positions = Step(map, positions);
        }

        positions.Count.Should().Be(3764);
    }

    [Test]
    public void Part2()
    {
        const int steps = 26501365; // 202300 * 131 + 65
        const int bufferSize = 270; // anything that is > 260
        var buffers = new Dictionary<Complex, CircularBuffer<long>>();

        foreach (var entry in EntryPoints)
        {
            buffers[entry] = new CircularBuffer<long>(bufferSize);
        }

        buffers[Center][1] = 1;
        for (var i = 1; i < steps; i++)
        {
            foreach (var cb in buffers.Values)
            {
                cb[^3] += cb[^1];
            }

            buffers[Center].Shift(0);
            foreach (var item in Middles)
            {
                buffers[item].Shift(i % 131 == 65 ? 1 : 0);
            }
            foreach (var item in Corners)
            {
                buffers[item].Shift(i % 131 == 0 ? i / 131 : 0);
            }
        }
        
        var map = ParseMap(_input);
        var res = 0L;

        foreach (var entry in EntryPoints)
        {
            var positions = new HashSet<Complex> { entry };
            for (var i = 0; i < bufferSize; i++)
            {
                res += positions.Count * buffers[entry][i];
                positions = Step(map, positions);
            }
        }

        res.Should().Be(622926941971282);
    }

    private static readonly Complex Center = new Complex(65, 65);
    
    private static readonly Complex[] Corners = [
        new Complex(0, 0), new Complex(0, 130),
        new Complex(130, 130), new Complex(130, 0),
    ];

    private static readonly Complex[] Middles = [
        new Complex(65, 0), new Complex(65, 130),
        new Complex(0, 65), new Complex(130, 65),
    ];

    private static readonly Complex[] EntryPoints = [Center, ..Corners, ..Middles];
    private static readonly Complex[] Dirs = [1, -1, Complex.ImaginaryOne, -Complex.ImaginaryOne];
    
    private static HashSet<Complex> Step(Dictionary<Complex, char> map, HashSet<Complex> pos)
    {
        var res = new HashSet<Complex>();
        foreach (var p in pos)
        {
            foreach (var dir in new[] { 1, -1, Complex.ImaginaryOne, -Complex.ImaginaryOne})
            {
                var pT = p + dir;
                if (map.TryGetValue(pT, out var value) && value != '#')
                {
                    res.Add(pT);
                }
            }
        }

        return res;
    }
    
    private static Dictionary<Complex, char> ParseMap(string[] input) => (
        from iRow in Enumerable.Range(0, input.Length)
        from iCol in Enumerable.Range(0, input[0].Length)
        select new KeyValuePair<Complex, char>(
            new Complex(iCol, iRow), input[iRow][iCol]
        )
    ).ToDictionary();

    private class CircularBuffer<T>(int size)
    {
        public int Count => size;
        private readonly T[] _items = new T[size];
        private int _i;

        public T this[int index]
        {
            get => _items[(_i + index) % size];
            set => _items[(_i + index) % size] = value;
        }

        public T Shift(T t)
        {
            _i = (_i + size - 1) % size;
            var res = _items[_i];
            _items[_i] = t;
            return res;
        }
    }
}