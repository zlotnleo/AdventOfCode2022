namespace Day3;

public class Program
{
    private static string[] GetRucksacks() => File.ReadAllLines("input.txt");

    private static int GetPriority(char c) => c switch
    {
        >= 'a' and <= 'z' => c - 'a' + 1,
        >= 'A' and <= 'Z' => c - 'A' + 27,
    };

    private static int Part1(string[] rucksacks) =>
        rucksacks.Sum(r => GetPriority(
            r[..(r.Length / 2)].Intersect(r[(r.Length / 2)..]).Single()
        ));

    private static int Part2(string[] rucksacks) =>
        rucksacks.Select((r, index) => (r, index))
            .GroupBy(t => t.index / 3)
            .Sum(grouping => GetPriority(
                grouping.Select(t => t.r.AsEnumerable())
                    .Aggregate(Enumerable.Intersect)
                    .Single()
            ));

    public static void Main()
    {
        var rucksacks = GetRucksacks();
        Console.WriteLine(Part1(rucksacks));
        Console.WriteLine(Part2(rucksacks));
    }
}