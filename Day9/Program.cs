namespace Day9;

public class Program
{
    private static readonly IReadOnlyDictionary<char, (int dx, int dy)> Directions = new Dictionary<char, (int dx, int dy)>
    {
        {'U', (0, 1)},
        {'D', (0, -1)},
        {'L', (-1, 0)},
        {'R', (1, 0)},
    };

    private static List<(char, int)> GetMoves() =>
        File.ReadAllLines("input.txt")
            .Select(line => (line[0], int.Parse(line[2..])))
            .ToList();

    private static (int, int) GetNextTailPosition((int x, int y) tail, (int x, int y) head)
    {
        var dx = head.x - tail.x;
        var dy = head.y - tail.y;
        if (Math.Abs(dx) <= 1 && Math.Abs(dy) <= 1)
        {
            return tail;
        }

        return (tail.x + Math.Sign(dx), tail.y + Math.Sign(dy));
    }

    private static int Solve(List<(char, int)> moves, int ropeLength)
    {
        var rope = Enumerable.Repeat((x: 0, y: 0), ropeLength).ToList();
        var tailLocations = new HashSet<(int, int)> {rope[^1]};
        foreach (var (dir, steps) in moves)
        {
            var (dx, dy) = Directions[dir];
            for (var step = 0; step < steps; step++)
            {
                rope[0] = (rope[0].x + dx, rope[0].y + dy);
                for (var i = 1; i < rope.Count; i++)
                {
                    rope[i] = GetNextTailPosition(rope[i], rope[i - 1]);
                }

                tailLocations.Add(rope[^1]);
            }
        }

        return tailLocations.Count;
    }

    public static void Main()
    {
        var moves = GetMoves();
        Console.WriteLine(Solve(moves, 2));
        Console.WriteLine(Solve(moves, 10));
    }
}