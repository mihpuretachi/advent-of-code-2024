using System.Text.RegularExpressions;

namespace advent_of_code_2024.challenges;

internal class Day3
{
    public static List<string> ReadCommands()
    {
        var source = Utils.ReadChallengeSource(3);
        var commands = new List<string>();

        MatchCollection matchList = Regex.Matches(source, "(mul\\(\\d+\\,\\d+\\))|(do\\(\\))|(don\\'t\\(\\))");
        return matchList.Cast<Match>().Select(match => match.Value).ToList();
    }

    public static double ResolvePartOne()
    {
        var result = 0;
        var commands = ReadCommands();
        foreach (var command in commands)
        {
            if (command.StartsWith("mul"))
            {
                var numbers = Regex.Split(command, @"\D+").Where(value => !string.IsNullOrEmpty(value)).ToArray();
                var number1 = int.Parse(numbers[0]);
                var number2 = int.Parse(numbers[1]);
                result += number1 * number2;
            }
        }

        return result;
    }

    public static double ResolvePartTwo()
    {
        var result = 0;
        var commands = ReadCommands();
        var blockComputation = false;
        foreach (var command in commands)
        {
            if (command.StartsWith("don't"))
            {
                blockComputation = true;
                continue;
            }

            if (command.StartsWith("do"))
            {
                blockComputation = false;
                continue;
            }

            if (blockComputation)
            {
                continue;
            }

            var numbers = Regex.Split(command, @"\D+").Where(value => !string.IsNullOrEmpty(value)).ToArray();
            var number1 = int.Parse(numbers[0]);
            var number2 = int.Parse(numbers[1]);
            result += number1 * number2;
        }

        return result;
    }
}
