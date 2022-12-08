namespace Day8;

public class Program
{
    private static int[][] GetTrees() =>
        File.ReadAllLines("input.txt")
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();

    private readonly record struct Direction(int Dx, int Dy)
    {
        public static readonly Direction Up = new(0, -1);
        public static readonly Direction Down = new(0, 1);
        public static readonly Direction Left = new(-1, 0);
        public static readonly Direction Right = new(1, 0);
        public static readonly IList<Direction> All = new[] {Up, Down, Left, Right};
    }

    private static IEnumerable<(int tree, int x, int y)> TreesInDirectionStartingFrom(int[][] trees, int startX, int startY, Direction dir)
    {
        for (var coord = (x: startX, y: startY);
             0 <= coord.y && coord.y < trees.Length && 0 <= coord.x && coord.x < trees[coord.y].Length;
             coord = (coord.x + dir.Dx, coord.y + dir.Dy))
        {
            yield return (trees[coord.y][coord.x], coord.x, coord.y);
        }
    }

    private static int Part1(int[][] trees)
    {
        var edgeTrees = new List<(int x, int y, Direction dir)>();

        for (var y = 0; y < trees.Length; y++)
        {
            edgeTrees.Add((0, y, Direction.Right));
            edgeTrees.Add((trees[0].Length - 1, y, Direction.Left));
        }

        for (var x = 0; x < trees[0].Length; x++)
        {
            edgeTrees.Add((x, 0, Direction.Down));
            edgeTrees.Add((x, trees.Length - 1, Direction.Up));
        }

        var seenTrees = new HashSet<(int, int)>();
        foreach (var (x, y, dir) in edgeTrees)
        {
            var maxTreeSeen = -1;
            foreach (var (tree, tx, ty) in TreesInDirectionStartingFrom(trees, x, y, dir))
            {
                if (tree > maxTreeSeen)
                {
                    seenTrees.Add((tx, ty));
                    maxTreeSeen = tree;
                }
            }
        }

        return seenTrees.Count;
    }

    private static int Part2(int[][] trees)
    {
        var maxScenic = 0;

        for (var y = 0; y < trees.Length; y++)
        {
            for (var x = 0; x < trees[0].Length; x++)
            {
                var currentTree = trees[y][x];
                var scenicScore = Direction.All.Select(dir =>
                {
                    var treesInDirection = TreesInDirectionStartingFrom(trees, x, y, dir).Skip(1);
                    var count = 0;
                    foreach (var (tree, _, _) in treesInDirection)
                    {
                        count++;
                        if (tree >= currentTree)
                        {
                            break;
                        }
                    }

                    return count;

                }).Aggregate((a, b) => a * b);
                maxScenic = Math.Max(maxScenic, scenicScore);
            }
        }

        return maxScenic;
    }

    public static void Main()
    {
        var trees = GetTrees();
        Console.WriteLine(Part1(trees));
        Console.WriteLine(Part2(trees));
    }
}