using System.Text.RegularExpressions;

namespace Day19;

public partial class Program
{
    private record Blueprint(int Id,
        int OreRobotOre, int ClayRobotOre,
        int ObsidianRobotOre, int ObsidianRobotClay,
        int GeodeRobotOre, int GeodeRobotObsidian);

    [GeneratedRegex("^Blueprint (\\d+): Each ore robot costs (\\d+) ore. Each clay robot costs (\\d+) ore. Each obsidian robot costs (\\d+) ore and (\\d+) clay. Each geode robot costs (\\d+) ore and (\\d+) obsidian.$", RegexOptions.Compiled)]
    private static partial Regex BlueprintRegex();

    private static List<Blueprint> GetBlueprints()
    {
        var blueprintRegex = BlueprintRegex();
        return File.ReadAllLines("input.txt")
            .Select(line =>
            {
                var values = blueprintRegex.Match(line).Groups.Values.Skip(1).Select(g => int.Parse(g.Value)).ToList();
                return new Blueprint(values[0], values[1], values[2], values[3], values[4], values[5], values[6]);
            }).ToList();
    }

    private static int GetMaxGeodesProduced(Blueprint blueprint, int time)
    {
        var maxOreRobots = Math.Max(
            Math.Max(blueprint.OreRobotOre, blueprint.ClayRobotOre),
            Math.Max(blueprint.ObsidianRobotOre, blueprint.GeodeRobotOre)
        );
        var maxClayRobots = blueprint.ObsidianRobotClay;
        var maxObsidianRobots = blueprint.GeodeRobotObsidian;

        var visitedWithTimeRemaining = new Dictionary<(int, int, int, int, int, int, int, int), int>();
        var stack = new Stack<(int, int, int, int, int, int, int, int, int)>();
        stack.Push((1, 0, 0, 0, 0, 0, 0, 0, time));
        var maxGeodes = 0;
        while (stack.TryPop(out var value))
        {
            var (oreRobots, ore, clayRobots, clay, obsidianRobots, obsidian, geodeRobots, geodes, timeRemaining) = value;

            maxGeodes = Math.Max(maxGeodes, geodes);
            if (timeRemaining == 0)
            {
                continue;
            }

            // Upper bound on geodes assuming we make a geode breaking robot on each remaining step
            var geodesUpperBound = geodes + (2 * geodeRobots + timeRemaining - 1) * timeRemaining / 2;
            if (geodesUpperBound < maxGeodes)
            {
                continue;
            }

            // Limit the resource amount to the maximum number required
            // to maximise the chance of a duplicate state being detected
            // The upper bound is the amount required to maintain the resource at the maximum
            // level used up each minute for all of the remaining time
            // assuming no more robots producing this resource are made

            var oreLimit = maxOreRobots + (timeRemaining - 1) * (maxOreRobots - oreRobots);
            if (ore > oreLimit)
            {
                ore = oreLimit;
            }

            var clayLimit = maxClayRobots + (timeRemaining - 1) * (maxClayRobots - clayRobots);
            if (clay > clayLimit)
            {
                clay = clayLimit;
            }

            var obsidianLimit = maxObsidianRobots + (timeRemaining - 1) * (maxObsidianRobots - obsidianRobots);
            if (obsidian > obsidianLimit)
            {
                obsidian = obsidianLimit;
            }

            var visitedKey = (oreRobots, ore, clayRobots, clay, obsidianRobots, obsidian, geodeRobots, geodes);
            if (visitedWithTimeRemaining.TryGetValue(visitedKey, out var lastTimeRemaining)
                && lastTimeRemaining >= timeRemaining)
            {
                continue;
            }

            visitedWithTimeRemaining[visitedKey] = timeRemaining;

            if (ore >= blueprint.OreRobotOre && oreRobots < maxOreRobots)
            {
                stack.Push((
                    oreRobots + 1, ore + oreRobots - blueprint.OreRobotOre,
                    clayRobots, clay + clayRobots,
                    obsidianRobots, obsidian + obsidianRobots,
                    geodeRobots, geodes + geodeRobots,
                    timeRemaining - 1
                ));
            }

            if (ore >= blueprint.ClayRobotOre && clayRobots < maxClayRobots)
            {
                stack.Push((
                    oreRobots, ore + oreRobots - blueprint.ClayRobotOre,
                    clayRobots + 1, clay + clayRobots,
                    obsidianRobots, obsidian + obsidianRobots,
                    geodeRobots, geodes + geodeRobots,
                    timeRemaining - 1
                ));
            }

            if (ore >= blueprint.ObsidianRobotOre && clay >= blueprint.ObsidianRobotClay && obsidianRobots < maxObsidianRobots)
            {
                stack.Push((
                    oreRobots, ore + oreRobots - blueprint.ObsidianRobotOre,
                    clayRobots, clay + clayRobots - blueprint.ObsidianRobotClay,
                    obsidianRobots + 1, obsidian + obsidianRobots,
                    geodeRobots, geodes + geodeRobots,
                    timeRemaining - 1
                ));
            }

            if (ore >= blueprint.GeodeRobotOre && obsidian >= blueprint.GeodeRobotObsidian)
            {
                stack.Push((
                    oreRobots, ore + oreRobots - blueprint.GeodeRobotOre,
                    clayRobots, clay + clayRobots,
                    obsidianRobots, obsidian + obsidianRobots - blueprint.GeodeRobotObsidian,
                    geodeRobots + 1, geodes + geodeRobots,
                    timeRemaining - 1
                ));
            }
            else
            {
                stack.Push((
                    oreRobots, ore + oreRobots,
                    clayRobots, clay + clayRobots,
                    obsidianRobots, obsidian + obsidianRobots,
                    geodeRobots, geodes + geodeRobots,
                    timeRemaining - 1
                ));
            }
        }

        return maxGeodes;
    }

    private static int Part1(List<Blueprint> blueprints) => blueprints.Sum(b => b.Id * GetMaxGeodesProduced(b, 24));

    private static int Part2(List<Blueprint> blueprints) => blueprints.Take(3).Select(b => GetMaxGeodesProduced(b, 32))
        .Aggregate((a, b) => a * b);

    public static void Main()
    {
        var blueprints = GetBlueprints();
        Console.WriteLine(Part1(blueprints));
        Console.WriteLine(Part2(blueprints));
    }
}