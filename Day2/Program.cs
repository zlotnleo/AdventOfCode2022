namespace Day2;

public class Program
{
    private static List<(char, char)> GetMoves() =>
        File.ReadAllLines("input.txt")
            .Select(line => (line[0], line[2]))
            .ToList();

    private static int ScoreGame(int elfMove, int myMove)
    {
        var scoreForShape = myMove + 1;
        var scoreForOutcome = 3 * ((myMove - elfMove + 1 + 3) % 3);
        return scoreForShape + scoreForOutcome;
    }

    private static int Part1(List<(char ElfMove, char MyMove)> moves) =>
        moves.Sum(t => ScoreGame(t.ElfMove - 'A', t.MyMove - 'X'));

    private static int Part2(List<(char ElfMove, char Outcome)> moves) =>
        moves.Sum(t => ScoreGame(t.ElfMove - 'A', (t.ElfMove - 'A' + t.Outcome - 'Y' + 3) % 3));

    public static void Main()
    {
        var moves = GetMoves();
        Console.WriteLine(Part1(moves));
        Console.WriteLine(Part2(moves));
    }
}