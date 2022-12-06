namespace Day6;

public class Program
{
    private static int Solve(string signal, int markerLength) =>
        Enumerable.Range(markerLength, signal.Length - markerLength + 1)
            .First(i => signal[(i - markerLength)..i].Distinct().Count() == markerLength);

    public static void Main()
    {
        var signal = File.ReadAllText("input.txt");
        Console.WriteLine(Solve(signal, 4));
        Console.WriteLine(Solve(signal, 14));
    }
}