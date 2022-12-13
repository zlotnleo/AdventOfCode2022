namespace Day13;

public interface IPacketData
{
}

public record IntPacket(int Value) : IPacketData;

public class ListPacket : List<IPacketData>, IPacketData
{
    public ListPacket()
    {
    }

    public ListPacket(IEnumerable<IPacketData> items) : base(items)
    {
    }

    public ListPacket Slice(int start, int length) => new(this.Skip(start).Take(length));
}

public class Program
{
    private static IPacketData ParsePacket(IEnumerator<char> chars)
    {
        if (chars.Current == '[')
        {
            chars.MoveNext();
            return ParseListPacket(chars);
        }

        return ParseInt(chars);
    }

    private static ListPacket ParseListPacket(IEnumerator<char> chars)
    {
        var packets = new ListPacket();
        while (chars.Current != ']')
        {
            packets.Add(ParsePacket(chars));
            if (chars.Current == ',')
            {
                chars.MoveNext();
            }
        }

        chars.MoveNext();

        return packets;
    }

    private static IntPacket ParseInt(IEnumerator<char> chars)
    {
        var value = 0;
        while (chars.Current is >= '0' and <= '9')
        {
            value = value * 10 + (chars.Current - '0');
            chars.MoveNext();
        }

        return new IntPacket(value);
    }

    private static List<IPacketData> GetPacketPairs() =>
        File.ReadAllLines("input.txt")
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line =>
            {
                using var enumerator = line.GetEnumerator();
                enumerator.MoveNext();
                return ParsePacket(enumerator);
            }).ToList();

    private static int Compare(IPacketData left, IPacketData right) => (left, right) switch
    {
        (IntPacket(var l), IntPacket(var r)) => l.CompareTo(r),
        (IntPacket i, ListPacket l) => Compare(new ListPacket(new IPacketData[] {i}), l),
        (ListPacket l, IntPacket i) => Compare(l, new ListPacket(new IPacketData[] {i})),
        (ListPacket and [], ListPacket and []) => 0,
        (ListPacket and [], _) => -1,
        (_, ListPacket and []) => 1,
        (ListPacket and [var l, .. var lRest], ListPacket and [var r, ..var rRest]) =>
            Compare(l, r) switch
            {
                0 => Compare(lRest, rRest),
                var c => c
            }
    };

    private static int Part1(List<IPacketData> packets) =>
        packets.Select((p, i) => (Packet: p, OriginalIndex: i))
            .GroupBy(t => t.OriginalIndex / 2)
            .Select(g => g.ToList())
            .Select((g, i) => (LeftPacket: g[0].Packet, RightPacket: g[1].Packet, PairIndex: i))
            .Where(t => Compare(t.LeftPacket, t.RightPacket) < 0)
            .Sum(t => t.PairIndex + 1);

    private static int Part2(List<IPacketData> packets)
    {
        var index1 = 1 + packets.Count(packet =>
            Compare(packet, new ListPacket {new ListPacket {new IntPacket(2)}}) < 0
        );
        var index2 = 2 + packets.Count(packet =>
            Compare(packet, new ListPacket {new ListPacket {new IntPacket(6)}}) < 0
        );
        return index1 * index2;
    }

    public static void Main()
    {
        var packets = GetPacketPairs();
        Console.WriteLine(Part1(packets));
        Console.WriteLine(Part2(packets));
    }
}