namespace Day22;

public class Program
{
    private enum Tile
    {
        Wall,
        Empty
    }

    private record Move
    {
        public record Forward(int Steps) : Move;

        public record TurnRight : Move;

        public record TurnLeft : Move;

        private Move()
        {
        }
    }

    private enum Direction
    {
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    private static readonly (int dx, int dy)[] Directions = {(1, 0), (0, 1), (-1, 0), (0, -1)};

    private static (Dictionary<(int, int), Tile> map, List<Move> moves) GetInput()
    {
        var lines = File.ReadAllLines("input.txt");
        var map = new Dictionary<(int, int), Tile>();
        for (var y = 0; y < lines.Length - 2; y++)
        {
            for (var x = 0; x < lines[y].Length; x++)
            {
                if (lines[y][x] == '.')
                {
                    map[(x, y)] = Tile.Empty;
                }
                else if (lines[y][x] == '#')
                {
                    map[(x, y)] = Tile.Wall;
                }
            }
        }

        var moves = new List<Move>();
        foreach (var c in lines[^1])
        {
            switch (c)
            {
                case 'R':
                    moves.Add(new Move.TurnRight());
                    break;
                case 'L':
                    moves.Add(new Move.TurnLeft());
                    break;
                case >= '0' and <= '9' when moves is [.., Move.Forward(var steps)]:
                    moves[^1] = new Move.Forward(steps * 10 + c - '0');
                    break;
                case >= '0' and <= '9':
                    moves.Add(new Move.Forward(c - '0'));
                    break;
            }
        }

        return (map, moves);
    }

    private static (int x, int y, Direction direction) GetNextPosition1(
        Dictionary<(int x, int y), Tile> map, int x, int y, Direction direction)
    {
        var (dx, dy) = Directions[(int)direction];
        var newX = x + dx;
        var newY = y + dy;

        if (!map.ContainsKey((newX, newY)))
        {
            (newX, newY) = direction switch
            {
                Direction.Right => map.Keys.Where(c => c.y == y).MinBy(c => c.x),
                Direction.Down => map.Keys.Where(c => c.x == x).MinBy(c => c.y),
                Direction.Left => map.Keys.Where(c => c.y == y).MaxBy(c => c.x),
                Direction.Up => map.Keys.Where(c => c.x == x).MaxBy(c => c.y),
            };
        }

        return (newX, newY, direction);
    }

    private static (int x, int y, Direction direction) GetNextPosition2(
        Dictionary<(int x, int y), Tile> map, int x, int y, Direction direction)
    {
        var (dx, dy) = Directions[(int)direction];
        var newX = x + dx;
        var newY = y + dy;

        if (!map.ContainsKey((newX, newY)))
        {
            // Same letter mean sides connect
            //
            // face x,y -> 0   1   2
            //      |
            //      v       +-b-+-c-+
            //      0       a   |   d
            //              +---+-e-+
            //      1       g   e
            //          +-g-+---+
            //      2   a   |   d
            //          +---+-f-+
            //      3   b   f
            //          +-c-+
            var faceX = x / 50;
            var faceY = y / 50;
            (newX, newY, direction) = (faceX, faceY, direction) switch
            {
                (1, 0, Direction.Up) => (0, 3 * 50 + x % 50, Direction.Right),
                (1, 0, Direction.Left) => (0, 3 * 50 - y % 50 - 1, Direction.Right),
                (2, 0, Direction.Up) => (x % 50, 4 * 50 - 1, Direction.Up),
                (2, 0, Direction.Right) => (2 * 50 - 1, 3 * 50 - y % 50 - 1, Direction.Left),
                (2, 0, Direction.Down) => (2 * 50 - 1, 50 + x % 50, Direction.Left),
                (1, 1, Direction.Left) => (y % 50, 2 * 50, Direction.Down),
                (1, 1, Direction.Right) => (2 * 50 + y % 50, 50 - 1, Direction.Up),
                (0, 2, Direction.Up) => (50, 50 + x % 50, Direction.Right),
                (0, 2, Direction.Left) => (50, 50 - y % 50 - 1, Direction.Right),
                (1, 2, Direction.Right) => (3 * 50 - 1, 50 - y % 50 - 1, Direction.Left),
                (1, 2, Direction.Down) => (50 - 1, 3 * 50 + x % 50, Direction.Left),
                (0, 3, Direction.Left) => (50 + y % 50, 0, Direction.Down),
                (0, 3, Direction.Right) => (50 + y % 50, 3 * 50 - 1, Direction.Up),
                (0, 3, Direction.Down) => (2 * 50 + x % 50, 0, Direction.Down)
            };
        }

        return (newX, newY, direction);
    }

    private static int Solve(Dictionary<(int x, int y), Tile> map, List<Move> moves,
        Func<Dictionary<(int, int), Tile>, int, int, Direction, (int, int, Direction)> getNextPosition)
    {
        var start = map.Where(kvp => kvp.Key.y == 0 && kvp.Value == Tile.Empty).MinBy(kvp => kvp.Key.x).Key;

        var (finalX, finalY, finalDir) = moves.Aggregate((start.x, start.y, direction: Direction.Right), (t, move) =>
        {
            var (x, y, direction) = t;
            switch (move)
            {
                case Move.TurnRight:
                    return (x, y, (Direction)(((int)direction + 1) % 4));
                case Move.TurnLeft:
                    return (x, y, (Direction)(((int)direction + 3) % 4));
                case Move.Forward(var steps):
                    for (var step = 0; step < steps; step++)
                    {
                        var (newX, newY, newDirection) = getNextPosition(map, x, y, direction);
                        if (map[(newX, newY)] == Tile.Wall)
                        {
                            break;
                        }

                        (x, y, direction) = (newX, newY, newDirection);
                    }

                    return (x, y, direction);
                default:
                    throw new ArgumentOutOfRangeException(nameof(move));
            }
        });

        return 1000 * (finalY + 1) + 4 * (finalX + 1) + (int)finalDir;
    }

    public static void Main()
    {
        var (map, moves) = GetInput();
        Console.WriteLine(Solve(map, moves, GetNextPosition1));
        Console.WriteLine(Solve(map, moves, GetNextPosition2));
    }
}