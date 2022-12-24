namespace Day24;

public class Program
{
    private static readonly (int dx, int dy)[] Moves = {(1, 0), (-1, 0), (0, 1), (0, -1), (0, 0)};

    private static (int width, int height, int startX, int endX, List<(int x, int y, int dx, int dy)> blizzards) GetInput()
    {
        var lines = File.ReadAllLines("input.txt");
        var height = lines.Length - 2;
        var width = lines[0].Length - 2;
        var startX = lines[0].IndexOf('.') - 1;
        var endX = lines[^1].IndexOf('.') - 1;
        var blizzards = new List<(int x, int y, int dx, int dy)>();
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                switch (lines[y + 1][x + 1])
                {
                    case '<':
                        blizzards.Add((x, y, -1, 0));
                        break;
                    case '>':
                        blizzards.Add((x, y, 1, 0));
                        break;
                    case 'v':
                        blizzards.Add((x, y, 0, 1));
                        break;
                    case '^':
                        blizzards.Add((x, y, 0, -1));
                        break;
                }
            }
        }

        return (width, height, startX, endX, blizzards);
    }

    private static int PosMod(int a, int b)
    {
        var r = a % b;
        return r switch {< 0 => r + b, _ => r};
    }

    private static int Gcd(int a, int b)
    {
        while (b != 0)
        {
            (a, b) = (b, a % b);
        }

        return a;
    }

    private static int Lcm(int a, int b)
    {
        return a / Gcd(a, b) * b;
    }

    private static int GetTimeAfterPath(
        int width, int height, int startX, int startY, int startTime, int endX, int endY,
        int timePeriod, HashSet<(int, int, int)> blizzardCache)
    {
        var visited = new HashSet<(int, int, int)>();
        var queue = new Queue<(int x, int y, int time)>();
        queue.Enqueue((startX, startY, startTime % timePeriod));
        while (queue.TryDequeue(out var value))
        {
            var (x, y, time) = value;

            if (!visited.Add((x, y, time % timePeriod)))
            {
                continue;
            }

            if (x == endX && y == endY)
            {
                return time;
            }

            foreach (var (dx, dy) in Moves)
            {
                var (newX, newY) = (x + dx, y + dy);

                if ((newX < 0 || newX >= width || newY < 0 || newY >= height) && (x != startX || y != startY)
                    || blizzardCache.Contains((newX, newY, (time + 1) % timePeriod)))
                {
                    continue;
                }

                queue.Enqueue((newX, newY, time + 1));
            }
        }

        throw new Exception("No path");
    }

    private static (int, int) Solve(int width, int height, int startX, int endX, List<(int x, int y, int dx, int dy)> blizzards)
    {
        var timePeriod = Lcm(width, height);

        var blizzardCache = new HashSet<(int x, int y, int time)>();
        for (var time = 0; time < timePeriod; time++)
        {
            blizzardCache.UnionWith(blizzards.Select(b =>
                (PosMod(b.x + b.dx * time, width), PosMod(b.y + b.dy * time, height), time)
            ));
        }

        var part1 = GetTimeAfterPath(width, height, startX, -1, 0, endX, height - 1, timePeriod, blizzardCache) + 1;
        var tempTime = GetTimeAfterPath(width, height, endX, height, part1, startX, 0, timePeriod, blizzardCache) + 1;
        var part2 = GetTimeAfterPath(width, height, startX, -1, tempTime, endX, height - 1, timePeriod, blizzardCache) + 1;
        return (part1, part2);
    }

    public static void Main()
    {
        var (width, height, startX, endX, blizzards) = GetInput();
        var (part1, part2) = Solve(width, height, startX, endX, blizzards);
        Console.WriteLine(part1);
        Console.WriteLine(part2);
    }
}