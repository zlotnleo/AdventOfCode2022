namespace Day1;

public class Program
{
    private static List<int> GetElves()
    {
        var elves = new List<int> {0};
        foreach (var line in File.ReadAllLines("input.txt"))
        {
            if (line == "")
            {
                elves.Add(0);
            }
            else
            {
                elves[^1] += int.Parse(line);
            }
        }

        return elves;
    }

    private static int Part1(IEnumerable<int> elves) => elves.Max();

    private static int Part2(IEnumerable<int> elves) => elves.OrderByDescending(x => x).Take(3).Sum();

    public static void Main()
    {
        var numbers = GetElves();
        Console.WriteLine(Part1(numbers));
        Console.WriteLine(Part2(numbers));
    }
}