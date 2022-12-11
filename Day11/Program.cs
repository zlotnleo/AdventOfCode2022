using System.Text.RegularExpressions;

namespace Day11;

public class Monkey
{
    public Queue<long> Items;
    public Func<long, long> Operation;
    public int TestDivisor;
    public int ThrowToMonkeyWhenTrue;
    public int ThrowToMonkeyWhenFalse;
    public int Inspections;

    public Monkey()
    {
    }

    public Monkey(Monkey monkey)
    {
        Items = new Queue<long>(monkey.Items);
        Operation = monkey.Operation;
        TestDivisor = monkey.TestDivisor;
        ThrowToMonkeyWhenTrue = monkey.ThrowToMonkeyWhenTrue;
        ThrowToMonkeyWhenFalse = monkey.ThrowToMonkeyWhenFalse;
        Inspections = monkey.Inspections;
    }
}

public partial class Program
{
    private const string MonkeyRegexStr = @"^Monkey \d+:$
^  Starting items:(?: (\d+),?)*$
^  Operation: new = old ([+*]) (old|\d+)$
^  Test: divisible by (\d+)$
^    If true: throw to monkey (\d+)$
^    If false: throw to monkey (\d+)$";

    [GeneratedRegex(MonkeyRegexStr, RegexOptions.Multiline)]
    private static partial Regex FullMonkeyRegex();

    private static List<Monkey> GetMonkeys() =>
        FullMonkeyRegex().Matches(File.ReadAllText("input.txt"))
            .Select(match => new Monkey {
                Items = new Queue<long>(
                    match.Groups[1].Captures.Select(c => long.Parse(c.Value))
                ),
                Operation = old =>
                {
                    var secondOperand = match.Groups[3].Value == "old" ? old : long.Parse(match.Groups[3].Value);
                    return match.Groups[2].Value switch
                    {
                        "+" => old + secondOperand,
                        "*" => old * secondOperand,
                    };
                },
                TestDivisor = int.Parse(match.Groups[4].Value),
                ThrowToMonkeyWhenTrue = int.Parse(match.Groups[5].Value),
                ThrowToMonkeyWhenFalse = int.Parse(match.Groups[6].Value)
            })
            .ToList();

    private static long Solve(List<Monkey> initialMonkeys, bool isPart1)
    {
        var monkeys = initialMonkeys.Select(m => new Monkey(m)).ToList();
        var lcm = monkeys.Select(m => m.TestDivisor).Aggregate((a, b) => a * b);
        var rounds = isPart1 ? 20 : 10000;
        for (var round = 1; round <= rounds; round++)
        {
            foreach (var monkey in monkeys)
            {
                while (monkey.Items.TryDequeue(out var worryLevel))
                {
                    monkey.Inspections++;
                    worryLevel = monkey.Operation(worryLevel);
                    if (isPart1)
                    {
                        worryLevel /= 3;
                    }
                    else
                    {
                        worryLevel %= lcm;
                    }

                    var nextMonkeyIndex = worryLevel % monkey.TestDivisor == 0
                        ? monkey.ThrowToMonkeyWhenTrue
                        : monkey.ThrowToMonkeyWhenFalse;
                    monkeys[nextMonkeyIndex].Items.Enqueue(worryLevel);
                }
            }
        }

        var inspections = monkeys.Select(monkey => monkey.Inspections).OrderByDescending(x => x).ToList();
        return (long) inspections[0] * inspections[1];
    }

    public static void Main()
    {
        var monkeys = GetMonkeys();
        Console.WriteLine(Solve(monkeys, true));
        Console.WriteLine(Solve(monkeys, false));
    }
}