using System.Text.RegularExpressions;

namespace Day4;

public class Program
{
    private static readonly Regex PairsRegex = new("^(\\d+)-(\\d+),(\\d+)-(\\d+)$", RegexOptions.Compiled);

    private readonly record struct Assignment(int Min, int Max)
    {
        public Assignment(string min, string max) : this(int.Parse(min), int.Parse(max))
        {
        }

        public bool Contains(Assignment other) => Min <= other.Min && other.Max <= Max;

        public bool Overlaps(Assignment other) => Min <= other.Max && other.Min <= Max;
    }

    private static List<(Assignment, Assignment)> GetInput() =>
        File.ReadAllLines("input.txt")
            .Select(line => PairsRegex.Match(line))
            .Select(match => (
                new Assignment(match.Groups[1].Value, match.Groups[2].Value),
                new Assignment(match.Groups[3].Value, match.Groups[4].Value)
            ))
            .ToList();

    private static int Part1(IEnumerable<(Assignment A1, Assignment A2)> assignmentPairs) =>
        assignmentPairs.Count(p =>
            p.A1.Contains(p.A2) || p.A2.Contains(p.A1)
        );

    private static int Part2(IEnumerable<(Assignment A1, Assignment A2)> assignmentPairs) =>
        assignmentPairs.Count(p => p.A1.Overlaps(p.A2));

    public static void Main()
    {
        var assignmentPairs = GetInput();
        Console.WriteLine(Part1(assignmentPairs));
        Console.WriteLine(Part2(assignmentPairs));
    }
}