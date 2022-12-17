namespace Day17;

public class Program
{
    private static readonly (int dx, int dy)[][] RocksOffsets =
    {
        new[] {(0, 0), (1, 0), (2, 0), (3, 0)},
        new[] {(0, 1), (1, 0), (1, 1), (1, 2), (2, 1)},
        new[] {(0, 0), (1, 0), (2, 0), (2, 1), (2, 2)},
        new[] {(0, 0), (0, 1), (0, 2), (0, 3)},
        new[] {(0, 0), (0, 1), (1, 0), (1, 1)},
    };

    private static long Solve(char[] jets, long rocksCount)
    {
        const int width = 7;
        var tower = new HashSet<(long, long)>();
        var towerHeight = 0L;
        var jetIndex = 0;

        var finalRockType = (rocksCount - 1) % RocksOffsets.Length;
        var seenHeightAndRockIndexByJetIndexForFinalRockType = new Dictionary<int, (long, long)>();

        for (var rockIndex = 0L; rockIndex < rocksCount; rockIndex++)
        {
            var rockType = rockIndex % RocksOffsets.Length;
            var rocks = RocksOffsets[rockType]
                .Select(r => (x: 2L + r.dx, y: towerHeight + 3 + r.dy))
                .ToList();

            var falling = true;
            while (falling)
            {
                var dx = jets[jetIndex] switch {'<' => -1, '>' => 1};
                jetIndex = (jetIndex + 1) % jets.Length;

                var afterSidewaysMove = rocks.Select(r => r with {x = r.x + dx}).ToList();
                if (afterSidewaysMove.All(r => r.x is >= 0 and < width && !tower.Contains(r)))
                {
                    rocks = afterSidewaysMove;
                }

                var afterDownMove = rocks.Select(r => r with {y = r.y - 1}).ToList();
                if (afterDownMove.All(r => r.y >= 0 && !tower.Contains(r)))
                {
                    rocks = afterDownMove;
                }
                else
                {
                    falling = false;
                }
            }

            tower.UnionWith(rocks);
            towerHeight = Math.Max(towerHeight, rocks.Max(r => r.y) + 1);

            if (rockType == finalRockType)
            {
                if (!seenHeightAndRockIndexByJetIndexForFinalRockType.TryGetValue(jetIndex, out var value))
                {
                    seenHeightAndRockIndexByJetIndexForFinalRockType[jetIndex] = (towerHeight, rockIndex);
                }
                else
                {
                    var (lastHeight, lastRockIndex) = value;
                    var rocksLeft = rocksCount - 1 - rockIndex;
                    var period = rockIndex - lastRockIndex;
                    if (rocksLeft % period == 0)
                    {
                        return towerHeight + (towerHeight - lastHeight) * rocksLeft / period;
                    }
                }
            }
        }

        return towerHeight;
    }

    public static void Main()
    {
        var jets = File.ReadAllText("input.txt").ToCharArray();
        Console.WriteLine(Solve(jets, 2022));
        Console.WriteLine(Solve(jets, 1000000000000));
    }
}