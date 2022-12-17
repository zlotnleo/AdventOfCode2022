using System.Text.RegularExpressions;

namespace Day16;

public partial class Program
{
    [GeneratedRegex("^Valve ([A-Z]+) has flow rate=(\\d+); tunnels? leads? to valves? (?:([A-Z]+)(?:, )?)+$")]
    private static partial Regex ValveRegex();

    private static Dictionary<string, (int, List<string>)> GetValves()
    {
        var valveRegex = ValveRegex();

        return File.ReadAllLines("input.txt")
            .Select(line => valveRegex.Match(line) switch
            {
                {
                    Success: true, Groups:
                    [
                        _,
                        {Value: var valve},
                        {Value: var flowRateStr},
                        {Captures: var captures}
                    ]
                } when int.TryParse(flowRateStr, out var flowRate) => (
                    Key: valve,
                    Value: (flowRate, captures.Select(c => c.Value).ToList())
                )
            }).ToDictionary(t => t.Key, t => t.Value);
    }

    private static Dictionary<string, (int, Dictionary<string, int>)> OptimiseValves(Dictionary<string, (int FlowRate, List<string>)> valves)
    {
        var optimisedValves = new Dictionary<string, (int, Dictionary<string, int>)>();

        var valvesToRemove = valves.Where(kvp => kvp.Value.FlowRate == 0)
            .Select(kvp => kvp.Key)
            .ToHashSet();

        var startingValves = new HashSet<string>(valves.Keys);
        startingValves.ExceptWith(valvesToRemove);
        startingValves.Add("AA");

        foreach (var startingValve in startingValves)
        {
            var distancesFromStartingValve = new Dictionary<string, int>();
            optimisedValves[startingValve] = (valves[startingValve].FlowRate, distancesFromStartingValve);
            var visited = new HashSet<string>();
            var queue = new Queue<(string, int)>();
            queue.Enqueue((startingValve, 0));
            while (queue.TryDequeue(out var value))
            {
                var (valve, steps) = value;
                if (!visited.Add(valve))
                {
                    continue;
                }

                if (valve != startingValve && !valvesToRemove.Contains(valve))
                {
                    distancesFromStartingValve[valve] = steps;
                }

                var (_, adjacentValves) = valves[valve];
                foreach (var adjacentValve in adjacentValves)
                {
                    queue.Enqueue((adjacentValve, steps + 1));
                }
            }
        }

        return optimisedValves;
    }

    private static int Solve(
        Dictionary<string, (int FlowRate, Dictionary<string, int>)> valves, int maxSteps, bool isElephantHelping)
    {
        var valveShiftLookup = valves.Select((valve, i) => (valve.Key, Value: i))
            .ToDictionary(t => t.Key, t => t.Value);

        var allValvesOpen = valves.Where(kvp => kvp.Value.FlowRate != 0)
            .Select(kvp => 1UL << valveShiftLookup[kvp.Key])
            .Aggregate((a, b) => a | b);

        var maxPressureReleased = 0;
        var stack = new Stack<(string? myValve, int? myStepsLeft, string? elephantValve, int? elephantStepsLeft,
            ulong openValves, int step, int pressurePerMinute, int totalPressure)>();

        var (_, initialMoves) = valves["AA"];
        foreach (var (myValveAfterInitial, myDistanceToValve) in initialMoves)
        {
            if (isElephantHelping)
            {
                foreach (var (elephantValveAfterInitial, elephantDistanceToValve) in initialMoves)
                {
                    if (myValveAfterInitial != elephantValveAfterInitial)
                    {
                        stack.Push((myValveAfterInitial, myDistanceToValve,
                            elephantValveAfterInitial, elephantDistanceToValve,
                            0, 1, 0, 0));
                    }
                }
            }
            else
            {
                stack.Push((myValveAfterInitial, myDistanceToValve,
                    null, null,
                    0, 1, 0, 0));
            }
        }

        while (stack.TryPop(out var value))
        {
            var (myValve, myStepsLeft, elephantValve, elephantStepsLeft, openValves, step, pressurePerMinute, totalPressure) = value;

            var totalPressureIfStoppedHere = totalPressure + pressurePerMinute * (maxSteps - step);
            maxPressureReleased = Math.Max(maxPressureReleased, totalPressureIfStoppedHere);

            if (step == maxSteps || openValves == allValvesOpen || (myValve == null && elephantValve == null))
            {
                continue;
            }

            var stepsUntilEitherCanOpenValve = (myStepsLeft, elephantStepsLeft) switch
            {
                ({}, {}) => Math.Min(myStepsLeft.Value, elephantStepsLeft.Value),
                ({}, null) => myStepsLeft.Value,
                (null, {}) => elephantStepsLeft.Value
            };

            myStepsLeft -= stepsUntilEitherCanOpenValve;
            elephantStepsLeft -= stepsUntilEitherCanOpenValve;
            step += stepsUntilEitherCanOpenValve;
            totalPressure += pressurePerMinute * stepsUntilEitherCanOpenValve;

            if (step + 1 > maxSteps)
            {
                continue;
            }

            switch (myValve, myStepsLeft, elephantValve, elephantStepsLeft)
            {
                case ({}, 0, _, null or > 0):
                {
                    var mask = 1UL << valveShiftLookup[myValve];
                    var nextOpenValves = openValves | mask;
                    var (myValveFlowRate, myNextValves) = valves[myValve];
                    var nextPressurePerMinute = pressurePerMinute + myValveFlowRate;
                    var anyAdded = false;
                    foreach (var (myNextValve, myNextStepsLeft) in myNextValves)
                    {
                        var isMyNextValveClosed = (nextOpenValves & (1UL << valveShiftLookup[myNextValve])) == 0;
                        if (isMyNextValveClosed && myNextValve != elephantValve)
                        {
                            stack.Push((myNextValve, myNextStepsLeft, elephantValve, elephantStepsLeft - 1,
                                nextOpenValves, step + 1, nextPressurePerMinute, totalPressure + nextPressurePerMinute));
                            anyAdded = true;
                        }
                    }

                    if (!anyAdded)
                    {
                        stack.Push((null, null, elephantValve, elephantStepsLeft - 1,
                            nextOpenValves, step + 1, nextPressurePerMinute, totalPressure + nextPressurePerMinute));
                    }
                    break;
                }
                case (_, null or > 0, {}, 0):
                {
                    var mask = 1UL << valveShiftLookup[elephantValve];
                    var nextOpenValves = openValves | mask;
                    var (elephantValveFlowRate, elephantNextValves) = valves[elephantValve];
                    var nextPressurePerMinute = pressurePerMinute + elephantValveFlowRate;
                    var anyAdded = false;
                    foreach (var (elephantNextValve, elephantNextStepsLeft) in elephantNextValves)
                    {
                        var isElephantNextValveClosed = (nextOpenValves & (1UL << valveShiftLookup[elephantNextValve])) == 0;
                        if (isElephantNextValveClosed && elephantNextValve != myValve)
                        {
                            stack.Push((myValve, myStepsLeft - 1, elephantNextValve, elephantNextStepsLeft,
                                nextOpenValves, step + 1, nextPressurePerMinute, totalPressure + nextPressurePerMinute));
                            anyAdded = true;
                        }
                    }

                    if (!anyAdded)
                    {
                        stack.Push((myValve, myStepsLeft - 1, null, null,
                            nextOpenValves, step + 1, nextPressurePerMinute, totalPressure + nextPressurePerMinute));
                    }
                    break;
                }
                case ({}, 0, {}, 0):
                {
                    var myMask = 1UL << valveShiftLookup[myValve];
                    var (myValveFlowRate, myNextValves) = valves[myValve];
                    var elephantMask = 1UL << valveShiftLookup[elephantValve];
                    var (elephantValveFlowRate, elephantNextValves) = valves[elephantValve];

                    var nextPressurePerMinute = pressurePerMinute + myValveFlowRate + elephantValveFlowRate;
                    var nextOpenValves = openValves | myMask | elephantMask;

                    foreach (var (myNextValve, myNextStepsLeft) in myNextValves)
                    {
                        var isMyNextValveClosed = (nextOpenValves & (1UL << valveShiftLookup[myNextValve])) == 0;
                        if (isMyNextValveClosed)
                        {
                            foreach (var (elephantNextValve, elephantNextStepsLeft) in elephantNextValves)
                            {
                                var isElephantNextValveClosed = (nextOpenValves & (1UL << valveShiftLookup[elephantNextValve])) == 0;
                                if (isElephantNextValveClosed && elephantNextValve != myNextValve)
                                {
                                    stack.Push((myNextValve, myNextStepsLeft, elephantNextValve, elephantNextStepsLeft,
                                        nextOpenValves, step + 1, nextPressurePerMinute, totalPressure + nextPressurePerMinute));
                                }
                            }
                        }
                    }
                    break;
                }
                default:
                    throw new Exception("Unexpected state");
            }
        }

        return maxPressureReleased;
    }

    public static void Main()
    {
        var valves = GetValves();
        var optimisedValves = OptimiseValves(valves);
        Console.WriteLine(Solve(optimisedValves, 30, false));
        Console.WriteLine(Solve(optimisedValves, 26, true));
    }
}