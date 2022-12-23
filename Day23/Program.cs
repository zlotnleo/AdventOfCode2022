namespace Day23;

public class Program
{
    // 0 1 2
    //
    // 3 # 4
    //
    // 5 6 7
    private static readonly (int dx, int dy, int shift)[] NeighboursWithShifts =
    {
        (-1, -1, 0), (0, -1, 1), (1, -1, 2),
        (-1,  0, 3),             (1,  0, 4),
        (-1,  1, 5), (0,  1, 6), (1,  1, 7)
    };

    private static readonly (int, int, uint)[] ProposedDirectionsWithMasks =
    {
        (0, -1, 1u << 0 | 1u << 1 | 1u << 2),
        (0,  1, 1u << 5 | 1u << 6 | 1u << 7),
        (-1, 0, 1u << 0 | 1u << 3 | 1u << 5),
        ( 1, 0, 1u << 2 | 1u << 4 | 1u << 7)
    };

    private static uint GetNeighbours(int x, int y, HashSet<(int, int)> elves) =>
        NeighboursWithShifts
            .Where(t => elves.Contains((x + t.dx, y + t.dy)))
            .Select(t => 1u << t.shift)
            .Aggregate(0u, (a, b) => a | b);

    private static HashSet<(int, int)> GetElves()
    {
        var lines = File.ReadAllLines("input.txt");
        var elves = new HashSet<(int, int)>();
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '#')
                {
                    elves.Add((x, y));
                }
            }
        }

        return elves;
    }

    private static int Solve(HashSet<(int x, int y)> elves, bool isPart2)
    {
        for (var round = 0; isPart2 || round < 10; round++)
        {
            var nextElves = new HashSet<(int, int)>();
            var proposedPositions = new Dictionary<(int, int), List<(int, int)>>();
            foreach (var (elfX, elfY) in elves)
            {
                var neighbours = GetNeighbours(elfX, elfY, elves);
                if (neighbours == 0u)
                {
                    nextElves.Add((elfX, elfY));
                    continue;
                }

                var elfProposedPosition = false;
                for (var i = 0; i < ProposedDirectionsWithMasks.Length && !elfProposedPosition; i++)
                {
                    var (moveDx, moveDy, bitMask) = ProposedDirectionsWithMasks[(i + round) % ProposedDirectionsWithMasks.Length];
                    if ((neighbours & bitMask) == 0)
                    {
                        var proposedPosition = (elfX + moveDx, elfY + moveDy);
                        if (!proposedPositions.TryGetValue(proposedPosition, out var elvesProposingPosition))
                        {
                            elvesProposingPosition = proposedPositions[proposedPosition] = new List<(int, int)>();
                        }

                        elvesProposingPosition.Add((elfX, elfY));
                        elfProposedPosition = true;
                    }
                }

                if (!elfProposedPosition)
                {
                    nextElves.Add((elfX, elfY));
                }
            }

            foreach (var (proposedPosition, elvesProposingPosition) in proposedPositions)
            {
                if (elvesProposingPosition.Count == 1)
                {
                    nextElves.Add(proposedPosition);
                }
                else
                {
                    nextElves.UnionWith(elvesProposingPosition);
                }
            }

            if (elves.All(elf => nextElves.Contains(elf)) && isPart2)
            {
                return round + 1;
            }

            elves = nextElves;
        }

        var minX = elves.Min(e => e.x);
        var maxX = elves.Max(e => e.x);
        var minY = elves.Min(e => e.y);
        var maxY = elves.Max(e => e.y);
        var emptyTiles = (maxX - minX + 1) * (maxY - minY + 1) - elves.Count;
        return emptyTiles;
    }

    public static void Main()
    {
        var elves = GetElves();
        Console.WriteLine(Solve(elves, false));
        Console.WriteLine(Solve(elves, true));
    }
}