namespace Day12;

public class Program
{
    private static ((int, int) start, (int, int) end, Dictionary<(int, int), char> map) GetInput()
    {
        var start = (-1, -1);
        var end = (-1, -1);
        var map = new Dictionary<(int, int), char>();
        var lines = File.ReadAllLines("input.txt");
        for (var y = 0; y < lines.Length; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                var c = lines[y][x];
                if (c == 'S')
                {
                    start = (x, y);
                    c = 'a';
                } else if (c == 'E')
                {
                    end = (x, y);
                    c = 'z';
                }

                map[(x, y)] = c;
            }
        }

        return (start, end, map);
    }

    private static readonly (int, int)[] Directions = {
        (1, 0), (-1, 0), (0, 1), (0, -1)
    };

    private static int? Solve((int, int)[] starts, (int, int) end, Dictionary<(int, int), char> map)
    {
        var visited = new HashSet<(int, int)>();
        var queue = new Queue<((int x, int y), int)>();
        foreach (var start in starts)
        {
            queue.Enqueue((start, 0));
        }
        while (queue.TryDequeue(out var value))
        {
            var (coord, steps) = value;
            if (coord == end)
            {
                return steps;
            }

            if (!visited.Add(coord))
            {
                continue;
            }

            foreach (var (dx, dy) in Directions)
            {
                var newCoord = (coord.x + dx, coord.y + dy);
                if (map.TryGetValue(newCoord, out var newCoordHeight) && newCoordHeight - map[coord] <= 1)
                {
                    queue.Enqueue((newCoord, steps + 1));
                }
            }
        }

        return null;
    }

    public static void Main()
    {
        var (start, end, map) = GetInput();
        Console.WriteLine(Solve(new[] {start}, end, map));

        var part2Starts = map.Where(kvp => kvp.Value == 'a').Select(kvp => kvp.Key).ToArray();
        Console.WriteLine(Solve(part2Starts, end, map));
    }
}