using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Day5;

public class Program
{
    private static readonly Regex MoveRegex = new("^move (\\d+) from (\\d+) to (\\d+)$");

    private static (List<Stack<char>> craneStacks, List<(int, int, int)> moves) ReadInput()
    {
        using var linesEnumerator = File.ReadAllLines("input.txt").AsEnumerable().GetEnumerator();

        var stackLines = new List<char[]>();
        var numberOfStacks = 0;
        while (linesEnumerator.MoveNext() && linesEnumerator.Current != "")
        {
            var currentStackLine = Enumerable.Range(0, linesEnumerator.Current.Length / 4 + 1)
                .Select(i => linesEnumerator.Current[4 * i + 1])
                .ToArray();
            numberOfStacks = Math.Max(numberOfStacks, currentStackLine.Length);
            stackLines.Add(currentStackLine);
        }

        stackLines.RemoveAt(stackLines.Count - 1);

        stackLines.Reverse();
        var craneStacks = Enumerable.Range(0, numberOfStacks)
            .Select(i => new Stack<char>(
                stackLines
                    .Select(stackLine => stackLine[i])
                    .Where(c => c != ' ')
            )).ToList();

        var moves = new List<(int, int, int)>();
        while (linesEnumerator.MoveNext())
        {
            var match = MoveRegex.Match(linesEnumerator.Current);
            Debug.Assert(match.Success);
            moves.Add((
                int.Parse(match.Groups[1].Value),
                int.Parse(match.Groups[2].Value),
                int.Parse(match.Groups[3].Value)
            ));
        }

        return (craneStacks, moves);
    }

    private static string Solve(List<Stack<char>> initialStacks, List<(int, int, int)> moves, bool multiCrate)
    {
        var stacks = initialStacks.Select(initialStack => new Stack<char>(initialStack.Reverse())).ToList();
        foreach (var (numberOfCrates, from, to) in moves)
        {
            var crates = new List<char>();
            for (var i = 0; i < numberOfCrates; i++)
            {
                crates.Add(stacks[from - 1].Pop());
            }

            if (multiCrate)
            {
                crates.Reverse();
            }

            foreach (var crate in crates)
            {
                stacks[to - 1].Push(crate);
            }
        }

        return new string(stacks.Select(stack => stack.Peek()).ToArray());
    }

    public static void Main()
    {
        var (stacks, moves) = ReadInput();
        Console.WriteLine(Solve(stacks, moves, false));
        Console.WriteLine(Solve(stacks, moves, true));
    }
}