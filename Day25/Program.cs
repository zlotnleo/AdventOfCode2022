namespace Day25;

public class Program
{
    private static string[] GetInput() => File.ReadAllLines("input.txt");

    private static long SnafuToInt(string snafu) =>
        snafu.Aggregate(0L, (number, snafuDigit) => number * 5 + snafuDigit switch
        {
            '=' => -2,
            '-' => -1,
            '0' => 0,
            '1' => 1,
            '2' => 2,
        });

    private static string IntToSnafu(long number)
    {
        var digits = new List<char>();
        while (number > 0)
        {
            var snafuDigit = (number % 5) switch
            {
                (0 or 1 or 2) and var m => (char) ('0' + m),
                3 => '=',
                4 => '-'
            };
            digits.Add(snafuDigit);
            if (snafuDigit is '-' or '=')
            {
                number += 5;
            }

            number /= 5;
        }

        digits.Reverse();
        return new string(digits.ToArray());
    }

    private static string Solve(string[] snafus) =>
        IntToSnafu(snafus.Select(SnafuToInt).Sum());

    public static void Main()
    {
        var snafus = GetInput();
        Console.WriteLine(Solve(snafus));
    }
}