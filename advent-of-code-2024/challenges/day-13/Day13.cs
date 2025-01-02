using System.Numerics;
using System.Text.RegularExpressions;

namespace advent_of_code_2024.challenges;

internal class Day13
{
    static IList<Machine> ReadMachines()
    {
        var lines = Utils.ReadLinesFromChallengeSource(13);

        (long, long) buttonA = (0, 0);
        (long, long) buttonB = (0, 0);
        (long, long) prize = (0, 0);

        var machines = new List<Machine>();

        foreach (var line in lines)
        {
            if (line.Contains("A"))
            {
                MatchCollection matchList = Regex.Matches(line, "\\d+");
                var values = matchList.Cast<Match>().Select(match => match.Value).ToList();
                buttonA = (long.Parse(values[0]), long.Parse(values[1]));
            }
            else if (line.Contains("B"))
            {
                MatchCollection matchList = Regex.Matches(line, "\\d+");
                var values = matchList.Cast<Match>().Select(match => match.Value).ToList();
                buttonB = (long.Parse(values[0]), long.Parse(values[1]));
            }
            else if (line.Contains("Prize"))
            {
                MatchCollection matchList = Regex.Matches(line, "\\d+");
                var values = matchList.Cast<Match>().Select(match => match.Value).ToList();
                prize = (long.Parse(values[0]), long.Parse(values[1]));
            }
            else
            {
                machines.Add(new Machine(buttonA, buttonB, prize));
            }
        }

        // Last line is not empty so:
        machines.Add(new Machine(buttonA, buttonB, prize));

        return machines;
    }

    public static long ResolvePartOne()
    {
        return ReadMachines()
            .Select(m => m.CalculateWinPossibility())
            .Where(p => p.Item1)
            .Sum(p => p.Item2);
    }

    public static long ResolvePartTwo()
    {
        var machinesResults = ReadMachines()
            .Select(m => m.CalculateWinPossibility(true))
            .ToList();           

        long result = 0;
        foreach (var machineResult in machinesResults.Where(r => r.Item1))
        {
            result += machineResult.Item2;
        }
        return result;
    }
}

internal class Machine((long, long) aEffect, (long, long) bEffect, (long, long) prizePosition)
{
    public (long, long) AEffect = aEffect;
    public (long, long) BEffect = bEffect;
    public (long, long) PrizePosition = prizePosition;

    public (bool, long) CalculateWinPossibility(bool needMeasurementAdjustment = false)
    {
        if (needMeasurementAdjustment)
        {
            PrizePosition.Item1 += 10000000000000;
            PrizePosition.Item2 += 10000000000000;
        }

        double tapsOnBButton = 1.0 * (PrizePosition.Item2 * AEffect.Item1 - PrizePosition.Item1 * AEffect.Item2) / (BEffect.Item2 * AEffect.Item1 - BEffect.Item1 * AEffect.Item2);
        double tapsOnAButton = 1.0 * (PrizePosition.Item1 - tapsOnBButton * BEffect.Item1) / AEffect.Item1;


        if (tapsOnAButton % 1 < 0.001 &&
            tapsOnBButton % 1 < 0.001 &&
            tapsOnAButton >= 0 &&
            tapsOnBButton >= 0)
        {
            var roundedTapsInAButton = (long)Math.Floor(tapsOnAButton);
            var roundedTapsInBButton = (long)Math.Floor(tapsOnBButton);

            // Console.WriteLine($"{roundedTapsInAButton} taps on A({AEffect.Item1}, {AEffect.Item2}) + {roundedTapsInBButton} taps on B({BEffect.Item1}, {BEffect.Item2}) will result in ({PrizePosition.Item1}, {PrizePosition.Item2})");
            return (true, roundedTapsInAButton * 3 + roundedTapsInBButton);
        }

        return (false, 0);
    }
}