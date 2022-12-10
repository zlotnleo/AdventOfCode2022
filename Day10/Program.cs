using System.Text;

namespace Day10;

public interface IInstruction
{
    public int Cycles { get; }
}

public record Noop : IInstruction
{
    public int Cycles => 1;
}

public record Addx(int Value) : IInstruction
{
    public int Cycles => 2;
}

public class Program
{
    private static List<IInstruction> GetInstructions() =>
        File.ReadAllLines("input.txt")
            .Select(line => line.Split(' ') switch
            {
                ["noop"] => new Noop() as IInstruction,
                ["addx", var arg] when int.TryParse(arg, out var value) => new Addx(value),
                _ => throw new Exception($"invalid instruction {line}")
            }).ToList();

    private static (int, string) Solve(List<IInstruction> instructions)
    {
        var signalStrengthSum = 0;

        const int width = 40;
        var screen = new StringBuilder();

        var x = 1;
        var instructionCycles = 0;
        var instructionIndex = 0;
        for (var cycle = 1; instructionIndex < instructions.Count; cycle++)
        {
            if (cycle % 40 == 20)
            {
                signalStrengthSum += cycle * x;
            }

            var currentPixelPosition = (cycle - 1) % width;
            screen.Append(Math.Abs(currentPixelPosition - x) <= 1 ? '#' : ' ');
            if (currentPixelPosition == width - 1)
            {
                screen.Append('\n');
            }

            instructionCycles++;
            var instruction = instructions[instructionIndex];
            if (instructionCycles == instruction.Cycles)
            {
                instructionCycles = 0;
                instructionIndex++;
                switch (instruction)
                {
                    case Addx(var value):
                        x += value;
                        break;
                    case Noop:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(instruction));
                }
            }
        }

        return (signalStrengthSum, screen.ToString());
    }

    public static void Main()
    {
        var instructions = GetInstructions();
        var (part1, part2) = Solve(instructions);
        Console.WriteLine(part1);
        Console.WriteLine(part2);
    }
}