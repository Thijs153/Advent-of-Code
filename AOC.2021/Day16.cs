using System.Collections;

namespace AOC._2021;

public class Day16
{
    private readonly string _input = File.ReadAllText("Inputs/Day16.txt");

    [Fact]
    public void Part1()
    {
        GetTotalVersion(GetPacket(Parse(_input)))
            .Should().Be(996);
    }

    [Fact]
    public void Part2()
    {
        Evaluate(GetPacket(Parse(_input)))
            .Should().Be(96257984154);
    }

    private static int GetTotalVersion(Packet packet) =>
        packet.version + packet.packets.Select(GetTotalVersion).Sum();

    private static long Evaluate(Packet packet)
    {
        var parts = packet.packets.Select(Evaluate).ToArray();
        return packet.type switch
        {
            0 => parts.Sum(),
            1 => parts.Aggregate(1L, (acc, x) => acc * x),
            2 => parts.Min(),
            3 => parts.Max(),
            4 => packet.payload,
            5 => parts[0] > parts[1] ? 1 : 0,
            6 => parts[0] < parts[1] ? 1 : 0,
            7 => parts[0] == parts[1] ? 1 : 0,
            _ => throw new Exception()
        };
    }
    
    // convert hex string to bit sequence reader
    private static BitSequenceReader Parse(string input) => new BitSequenceReader(
        new BitArray((
            from hexChar in input
            // get the 4 bits out of a hex char:
            let value = Convert.ToInt32(hexChar.ToString(), 16)
            // convert to bitmask
            from mask in new[] {8,4,2,1}
            select (mask & value) != 0
        ).ToArray()
    ));
    
    // make sense of the bit sequence
    private static Packet GetPacket(BitSequenceReader reader)
    {
        var version = reader.ReadInt(3);
        var type = reader.ReadInt(3);
        var packets = new List<Packet>();
        var payload = 0L;

        if (type == 0x4)
        {
            // literal, payload is encoded in the following bits in 5 bit long chunks:
            while (true)
            {
                var isLast = reader.ReadInt(1) == 0;
                payload = payload * 16 + reader.ReadInt(4);
                if (isLast) break;
            }
        } else if (reader.ReadInt(1) == 0)
        {
            // operator, the next 'length' long bit sequence encodes the sub packages:
            var length = reader.ReadInt(15);
            var subPackages = reader.GetBitSequenceReader(length);
            while (subPackages.Any())
            {
                packets.Add(GetPacket(subPackages));
            }
        }
        else
        {
            // operator with 'packetCount' sub packages:
            var packetCount = reader.ReadInt(11);
            packets.AddRange(from _ in Enumerable.Range(0, packetCount) select GetPacket(reader));
        }

        return new Packet(version, type, payload, packets.ToArray());
    }
    
    private class BitSequenceReader
    {
        private readonly BitArray bits;
        private int ptr;

        public BitSequenceReader(BitArray bits)
        {
            this.bits = bits;
        }

        public bool Any() => ptr < bits.Length;

        public BitSequenceReader GetBitSequenceReader(int bitCount)
        {
            var bitArray = new BitArray(bitCount);
            for (var i = 0; i < bitCount; i++)
            {
                bitArray.Set(i, bits[ptr++]);
            }

            return new BitSequenceReader(bitArray);
        }

        public int ReadInt(int bitCount)
        {
            var res = 0;
            for (var i = 0; i < bitCount; i++)
            {
                res = res * 2 + (bits[ptr++] ? 1 : 0);
            }

            return res;
        }

    }
    private record Packet(int version, int type, long payload, Packet[] packets);
}