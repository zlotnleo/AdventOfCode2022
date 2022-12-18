namespace Day18;

public class Program
{
    private static HashSet<(int, int, int)> GetInput() =>
        File.ReadAllLines("input.txt")
            .Select(line => line.Split(',').Select(int.Parse).ToArray() switch {
                [var x, var y, var z] => (x, y, z)
            }).ToHashSet();

    private static readonly (int dx, int dy, int dz)[] Directions =
    {
        (1, 0, 0), (-1, 0, 0), (0, 1, 0), (0, -1, 0), (0, 0, 1), (0, 0, -1)
    };

    private static int Part1(HashSet<(int x, int y, int z)> cubes) =>
        cubes.Sum(cube =>
            Directions.Count(direction =>
                !cubes.Contains((cube.x + direction.dx, cube.y + direction.dy, cube.z + direction.dz)
            ))
        );

    private static int Part2(HashSet<(int x, int y, int z)> cubes)
    {
        var minX = cubes.Min(c => c.x) - 1;
        var maxX = cubes.Max(c => c.x) + 1;
        var minY = cubes.Min(c => c.y) - 1;
        var maxY = cubes.Max(c => c.y) + 1;
        var minZ = cubes.Min(c => c.z) - 1;
        var maxZ = cubes.Max(c => c.z) + 1;

        var stack = new Stack<(int, int, int)>();
        var visited = new HashSet<(int, int, int)>();
        stack.Push((minX, minY, minZ));
        var seenCubeFacesCount = 0;
        while (stack.TryPop(out var value))
        {
            var (x, y, z) = value;
            if (x < minX || x > maxX || y < minY || y > maxY || z < minZ || z > maxZ)
            {
                continue;
            }

            if (!visited.Add(value))
            {
                continue;
            }

            foreach (var (dx, dy, dz) in Directions)
            {
                var position = (x + dx, y + dy, z + dz);
                if (cubes.Contains(position))
                {
                    seenCubeFacesCount++;
                }
                else
                {
                    stack.Push(position);
                }
            }
        }

        return seenCubeFacesCount;
    }

    public static void Main()
    {
        var input = GetInput();
        Console.WriteLine(Part1(input));
        Console.WriteLine(Part2(input));
    }
}