namespace Day20;

public class Program
{
    private class Node
    {
        public long Value;
        public Node? Prev;
        public Node? Next;

        public override string ToString()
        {
            return $"{Prev?.Value.ToString() ?? "_"} <= {Value} => {Next?.Value.ToString() ?? "~"}";
        }
    }

    private static List<long> GetInput() => File.ReadAllLines("input.txt").Select(long.Parse).ToList();

    private static long PosMod(long a, long b)
    {
        var r = a % b;
        return r switch {< 0 => r + b, _ => r};
    }

    private static Node? SkipAhead(Node? from, long steps)
    {
        var node = from;
        for (var i = 0; i < steps; i++)
        {
            node = node?.Next;
        }

        return node;
    }

    private static long Solve(List<long> input, long multiplier, int repeats)
    {
        Node? lastNode = null;
        var nodes = input.Select(value =>
        {
            var node = new Node
            {
                Value = value * multiplier,
                Prev = lastNode
            };
            if (lastNode != null)
            {
                lastNode.Next = node;
            }

            lastNode = node;
            return node;
        }).ToList();
        nodes[0].Prev = nodes[^1];
        nodes[^1].Next = nodes[0];

        for (var repeat = 0; repeat < repeats; repeat++)
        {
            foreach (var node in nodes)
            {
                if (node.Value == 0)
                {
                    continue;
                }

                node.Prev!.Next = node.Next;
                node.Next!.Prev = node.Prev;

                var insertAfter = SkipAhead(node, PosMod(node.Value, nodes.Count - 1));

                node.Next = insertAfter!.Next;
                node.Next!.Prev = node;
                node.Prev = insertAfter;
                insertAfter.Next = node;
            }
        }

        var result = 0L;
        var answerNode = nodes.First(n => n.Value == 0);
        for (var i = 0; i < 3; i++)
        {
            answerNode = SkipAhead(answerNode, PosMod(1000, nodes.Count));
            result += answerNode!.Value;
        }

        return result;
    }

    public static void Main()
    {
        var input = GetInput();
        Console.WriteLine(Solve(input, 1, 1));
        Console.WriteLine(Solve(input, 811589153, 10));
    }
}