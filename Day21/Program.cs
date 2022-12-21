namespace Day21;

public class Program
{
    public record Monkey
    {
        public record Number(long Value) : Monkey;

        public record Operation(string Monkey1, string Monkey2, char Op) : Monkey;

        public record Human : Monkey;

        private Monkey()
        {
        }
    }

    private static Dictionary<string, Monkey> GetMonkeys()
    {
        return File.ReadAllLines("input.txt")
            .Select(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries) switch
            {
                [[.. var name, ':'], var numStr] when long.TryParse(numStr, out var num) =>
                    (name, monkey: new Monkey.Number(num) as Monkey),
                [[.. var name, ':'], var monkey1, var op, var monkey2] =>
                    (name, monkey: new Monkey.Operation(monkey1, monkey2, op[0]))
            })
            .ToDictionary(t => t.name, t => t.monkey);
    }

    private static Dictionary<string, long?> GetMonkeyValues(Dictionary<string, Monkey> monkeys)
    {
        var values = new Dictionary<string, long?>();

        long? GetValue(string monkeyName) => values[monkeyName] = monkeys[monkeyName] switch
        {
            Monkey.Number(var value) => value,
            Monkey.Operation(var monkey1, var monkey2, '+') => GetValue(monkey1) + GetValue(monkey2),
            Monkey.Operation(var monkey1, var monkey2, '-') => GetValue(monkey1) - GetValue(monkey2),
            Monkey.Operation(var monkey1, var monkey2, '*') => GetValue(monkey1) * GetValue(monkey2),
            Monkey.Operation(var monkey1, var monkey2, '/') => GetValue(monkey1) / GetValue(monkey2),
            Monkey.Human => null
        };

        GetValue("root");

        return values;
    }

    private static long Part1(Dictionary<string, Monkey> monkeys) =>
        GetMonkeyValues(monkeys)["root"]!.Value;

    private static long Part2(Dictionary<string, Monkey> initialMonkeys)
    {
        var monkeys = new Dictionary<string, Monkey>(initialMonkeys)
        {
            ["humn"] = new Monkey.Human()
        };
        var monkeyValues = GetMonkeyValues(monkeys);

        long MakeEqual(string monkeyName, long value)
        {
            if (monkeyName == "humn")
            {
                return value;
            }

            var (monkey1, monkey2, op) = (Monkey.Operation) monkeys[monkeyName];
            return (op, monkeyValues[monkey1], monkeyValues[monkey2]) switch
            {
                ('+', {} v1, null) => MakeEqual(monkey2, value - v1),
                ('+', null, {} v2) => MakeEqual(monkey1, value - v2),
                ('-', {} v1, null) => MakeEqual(monkey2, v1 - value),
                ('-', null, {} v2) => MakeEqual(monkey1, value + v2),
                ('*', {} v1, null) => MakeEqual(monkey2, value / v1),
                ('*', null, {} v2) => MakeEqual(monkey1, value / v2),
                ('/', {} v1, null) => MakeEqual(monkey2, v1 / value),
                ('/', null, {} v2) => MakeEqual(monkey1, v2 * value)
            };
        }

        var (m1, m2, _) = (Monkey.Operation)monkeys["root"];
        return (monkeyValues[m1], monkeyValues[m2]) switch
        {
            ({} v1, null) => MakeEqual(m2, v1),
            (null, {} v2) => MakeEqual(m1, v2),
        };
    }

    public static void Main()
    {
        var monkeys = GetMonkeys();
        Console.WriteLine(Part1(monkeys));
        Console.WriteLine(Part2(monkeys));
    }
}