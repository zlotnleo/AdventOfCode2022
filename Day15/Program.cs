using System.Text.RegularExpressions;

namespace Day15;

public partial class Program
{
    private record Sensor(int X, int Y, int R)
    {
        public (int min, int max)? GetXIntervalAtY(int y)
        {
            var radiusAtY = R - Math.Abs(Y - y);
            if (radiusAtY < 0)
            {
                // Range doesn't reach y
                return null;
            }
            return (X - radiusAtY, X + radiusAtY);
        }
    }

    [GeneratedRegex("^Sensor at x=(-?\\d+), y=(-?\\d+): closest beacon is at x=(-?\\d+), y=(-?\\d+)$", RegexOptions.Compiled)]
    private static partial Regex SensorRegex();

    private static (List<Sensor> sensors, List<(int, int)> beacons) GetSensorsAndBeacons()
    {
        var sensorRegex = SensorRegex();
        var sensors = new List<Sensor>();
        var beacons = new List<(int, int)>();
        foreach (var line in File.ReadAllLines("input.txt"))
        {
            if (sensorRegex.Match(line) is
                {
                    Success: true,
                    Groups:
                    [
                        _,
                        {Value: var sensorXStr},
                        {Value: var sensorYStr},
                        {Value: var beaconXStr},
                        {Value: var beaconYStr}
                    ]
                }
                && int.TryParse(sensorXStr, out var sensorX) && int.TryParse(sensorYStr, out var sensorY)
                && int.TryParse(beaconXStr, out var beaconX) && int.TryParse(beaconYStr, out var beaconY)
               )
            {
                sensors.Add(new Sensor(sensorX, sensorY, Math.Abs(sensorX - beaconX) + Math.Abs(sensorY - beaconY)));
                beacons.Add((beaconX, beaconY));
            }
            else
            {
                throw new Exception($"Invalid input \"{line}\"");
            }
        }

        return (sensors, beacons);
    }

    private static List<(int min, int max)> GetNonOverlappingIntervalsAtY(List<Sensor> sensors, int y)
    {
        var intervals = new List<(int, int)>();
        foreach (var sensor in sensors)
        {
            if (sensor.GetXIntervalAtY(y) is {} interval)
            {
                intervals.Add(interval);
            }
        }

        if (intervals.Count == 0)
        {
            return new List<(int, int)>();
        }

        intervals.Sort();
        var nonOverlappingIntervals = new List<(int, int)>{intervals[0]};
        for(var i = 1; i < intervals.Count; i++)
        {
            var (curMin, curMax) = intervals[i];
            var (lastMin, lastMax) = nonOverlappingIntervals[^1];
            if (curMin > lastMax)
            {
                nonOverlappingIntervals.Add((curMin, curMax));
            }
            else if (curMax > lastMax)
            {
                nonOverlappingIntervals[^1] = (lastMin, curMax);
            }
        }

        return nonOverlappingIntervals;
    }

    private static int Part1(List<Sensor> sensors, List<(int x, int y)> beacons)
    {
        const int y = 2000000;
        var nonOverlappingIntervals = GetNonOverlappingIntervalsAtY(sensors, y);
        var countTotalSeen = nonOverlappingIntervals.Select(t => t.max - t.min + 1).Sum();
        var beaconsInSeen = beacons.Count(b =>
            b.y == y && nonOverlappingIntervals.Any(i => i.min <= b.x && b.x <= i.max)
        );

        return countTotalSeen - beaconsInSeen;
    }

    private static long Part2(List<Sensor> sensors)
    {
        for (var y = 0; y <= 4000000; y++)
        {
            var nonOverlappingIntervals = GetNonOverlappingIntervalsAtY(sensors, y)
                .Where(i => i is (<= 4000000, >= 0))
                .ToArray();
            if (nonOverlappingIntervals is [var (_, x), _])
            {
                return (long) (x + 1) * 4000000 + y;
            }
        }

        return -1;
    }

    public static void Main()
    {
        var (sensors, beacons) = GetSensorsAndBeacons();
        Console.WriteLine(Part1(sensors, beacons));
        Console.WriteLine(Part2(sensors));
    }
}