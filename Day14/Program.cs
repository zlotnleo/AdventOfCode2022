namespace Day14;

public class Program
{
    private static (HashSet<(int, int)>, int) GetCave()
    {
        var rocks = new HashSet<(int, int)>();
        var globalMaxY = 0;
        foreach (var line in File.ReadAllLines("input.txt"))
        {
            var vertexStrings = line.Split(" -> ");
            var firstCoordStrings = vertexStrings[0].Split(',');
            var lastCoord = (x: int.Parse(firstCoordStrings[0]), y: int.Parse(firstCoordStrings[1]));
            for (var i = 1; i < vertexStrings.Length; i++)
            {
                var nextCoordStrings = vertexStrings[i].Split(',');
                var nextCoord = (x: int.Parse(nextCoordStrings[0]), y: int.Parse(nextCoordStrings[1]));

                var minX = Math.Min(lastCoord.x, nextCoord.x);
                var maxX = Math.Max(lastCoord.x, nextCoord.x);
                var minY = Math.Min(lastCoord.y, nextCoord.y);
                var maxY = Math.Max(lastCoord.y, nextCoord.y);

                for (var x = minX; x <= maxX; x++)
                {
                    for (var y = minY; y <= maxY; y++)
                    {
                        rocks.Add((x, y));
                    }
                }

                globalMaxY = Math.Max(globalMaxY, maxY);
                lastCoord = nextCoord;
            }
        }

        return (rocks, globalMaxY);
    }

    private static (int, int) Solve(HashSet<(int, int)> cave, int maxY)
    {
        const int sourceX = 500;
        const int sourceY = 0;

        var sandMoves = new[] {(0, 1), (-1, 1), (1, 1)};

        int? part1Answer = null;
        int? part2Answer = null;
        for (var sandNumber = 0; !part1Answer.HasValue || !part2Answer.HasValue; sandNumber++)
        {
            var sand = (x: sourceX, y: sourceY);
            var falling = true;
            while (falling)
            {
                var moved = false;
                foreach (var (dx, dy) in sandMoves)
                {
                    var newSandLocation = (sand.x + dx, y: sand.y + dy);
                    if (!cave.Contains(newSandLocation) && newSandLocation.y < maxY + 2 )
                    {
                        sand = newSandLocation;
                        moved = true;
                        break;
                    }
                }

                if (!moved)
                {
                    falling = false;
                    cave.Add((sand.x, sand.y));
                }

                if (!part1Answer.HasValue && sand.y > maxY)
                {
                    part1Answer = sandNumber;
                }

                if (!part2Answer.HasValue && sand.y == 0)
                {
                    part2Answer = sandNumber + 1;
                }
            }
        }

        return (part1Answer.Value, part2Answer.Value);
    }

    public static void Main()
    {
        var (rocks, maxY) = GetCave();
        var (part1, part2) = Solve(rocks, maxY);
        Console.WriteLine(part1);
        Console.WriteLine(part2);
    }
}