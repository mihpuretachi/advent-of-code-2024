using System.Text.RegularExpressions;

namespace advent_of_code_2024.challenges;

internal class Day14
{
    private static IList<Robot> ReadRobots()
    {
        var source = Utils.ReadLinesFromChallengeSource(14);
        var robots = new List<Robot>();

        foreach (var line in source)
        {
            MatchCollection matchList = Regex.Matches(line, "-?\\d+");
            var values = matchList.Cast<Match>().Select(match => match.Value).ToList();

            var initialPosition = (int.Parse(values[0]), int.Parse(values[1]));
            var velocity = (int.Parse(values[2]), int.Parse(values[3]));
            var robot = new Robot(initialPosition, velocity);
            robots.Add(robot);
        }

        return robots;
    }

    public static long ResolvePartOne()
    {
        var robots = ReadRobots();
        var mapConfig = (101, 103);

        for (var i = 0; i < 100; i++)
        {
            foreach (var robot in robots)
            {
                robot.Walk(mapConfig);
            }
        }

        var groups = robots.Where(r => !r.IsInSafestArea(mapConfig))
            .GroupBy(r => r.CalculateQuandrant(mapConfig))
            .Select(group => new
            {
                Quadrant = group.Key,
                Count = group.Count()
            })
            .ToList();
        var result = 1;
        foreach (var group in groups)
        {
            result *= group.Count;
        }

        return result;
    }

    public static long ResolvePartTwo()
    {
        var robots = ReadRobots();
        var mapConfig = (101, 103);

        //PrintRobots(robots, mapConfig);
        var count = 0;

        while (!robots.Any(r => HasOtherRobotsBelow(r.CurrentPosition, 9, robots)))
        {
            foreach (var robot in robots)
            {
                robot.Walk(mapConfig);
            }
            count++;
        }

        Console.WriteLine($"After {count} second(s)");
        Console.WriteLine();
        PrintRobots(robots, mapConfig);

        return count;
    }

    private static bool HasOtherRobotsBelow((int, int) position, int robotsQuantity, IList<Robot> robots)
    {
        var hasRobots = true;

        for (var i = 1; i <= robotsQuantity; i++)
        {
            if (!robots.Any(r => r.CurrentPosition == (position.Item1, position.Item2 + i)))
            {
                hasRobots = false;
                break;
            }
        }


        return hasRobots;
    }

    private static void PrintRobots(IList<Robot> robots, (int, int) mapConfig)
    {
        for (var y = 0; y < mapConfig.Item2; y++)
        {
            for (var x = 0; x < mapConfig.Item1; x++)
            {
                var hasRobot = robots.Any(r => r.CurrentPosition.Item1 == x && r.CurrentPosition.Item2 == y);
                Console.Write(hasRobot ? "■" : ".");
            }
            Console.WriteLine();
        }
    }
    private static string CreateHash(IList<Robot> robots)
    {
        var orderedRobots = robots
            .Distinct()
            .OrderBy(r => r.CurrentPosition.Item1)
            .ThenBy(r => r.CurrentPosition.Item2)
            .ToList();
        return string.Concat(orderedRobots.Select(r => $"({r.CurrentPosition.Item1},{r.CurrentPosition.Item2})|"));
    }
}

class Robot((int, int) initialPosition, (int, int) velocity)
{
    public (int, int) CurrentPosition = initialPosition;
    private (int, int) Velocity = velocity;

    public void Walk((int, int) mapConfig)
    {
        var mostDistanceX = mapConfig.Item1 - 1;
        var mostDistanceY = mapConfig.Item2 - 1;

        var nextPossibleX = CurrentPosition.Item1 + Velocity.Item1;
        if (nextPossibleX > mostDistanceX)
        {
            nextPossibleX -= mapConfig.Item1;
        }
        else if (nextPossibleX < 0)
        {
            nextPossibleX += mapConfig.Item1;
        }

        var nextPossibleY = CurrentPosition.Item2 + Velocity.Item2;
        if (nextPossibleY > mostDistanceY)
        {
            nextPossibleY -= mapConfig.Item2;
        }
        else if (nextPossibleY < 0)
        {
            nextPossibleY += mapConfig.Item2;
        }

        CurrentPosition = (nextPossibleX, nextPossibleY);
    }

    public bool IsInSafestArea((int, int) mapConfig)
    {
        var halfX = (mapConfig.Item1 - 1) / 2;
        var halfY = (mapConfig.Item2 - 1) / 2;


        return CurrentPosition.Item1 == halfX ||
            CurrentPosition.Item2 == halfY;
    }

    public int CalculateQuandrant((int, int) mapConfig)
    {
        if (CurrentPosition.Item1 < mapConfig.Item1 / 2)
        {
            if (CurrentPosition.Item2 < mapConfig.Item2 / 2)
            {
                return 1;
            }
            else
            {
                return 3;
            }
        }
        else
        {
            if (CurrentPosition.Item2 < mapConfig.Item2 / 2)
            {
                return 2;
            }
            else
            {
                return 4;
            }
        }
    }

    public override bool Equals(object? other)
    {
        return other != null &&
            other is Robot otherRobot &&
            CurrentPosition.Item1 == otherRobot.CurrentPosition.Item1 &&
            CurrentPosition.Item2 == otherRobot.CurrentPosition.Item2;
    }

    public override int GetHashCode()
    {
        return CurrentPosition.Item1.GetHashCode() * CurrentPosition.Item2.GetHashCode() * 13;
    }
}