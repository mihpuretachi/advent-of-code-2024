namespace advent_of_code_2024.challenges;

internal class Day19
{
    static (IDictionary<string, bool>, IList<string>) ReadTowelsSetup()
    {
        var lines = Utils.ReadLinesFromChallengeSource(19);

        IDictionary<string, bool> towelPatterns = new Dictionary<string, bool>();
        IList<string> desiredTowels = [];

        for (int i = 0; i < lines.Count; i++)
        {
            if (i == 1)
            {
                continue;
            }

            var line = lines[i];

            if (i == 0)
            {
                towelPatterns = line.Split(", ").ToDictionary(v => v, v => true);
                continue;
            }

            desiredTowels.Add(line);
        }

        return (towelPatterns, desiredTowels);
    }

    public static long ResolvePartOne()
    {
        var (patterns, desiredTowels) = ReadTowelsSetup();

        return desiredTowels
            //.AsParallel()
            .Count(desiredPattern =>
            {
                var newCache = patterns.ToDictionary();
                var result = HasPossibleCombination(desiredPattern, newCache);
                Console.WriteLine($"Resolved line {desiredPattern} with {result}");
                return result;
            });
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }

    static bool HasPossibleCombination(string desiredPattern, IDictionary<string, bool> cache)
    {
        var count = 1;
        while (count <= desiredPattern.Length)
        {
            var section = new string(desiredPattern.Take(count).ToArray());
            if (cache.ContainsKey(section))
            {
                if (!cache[section])
                {
                    return false;
                }

                var remainingPattern = new string(desiredPattern.Skip(count).Take(desiredPattern.Length - count).ToArray());
                if (remainingPattern.Length == 0)
                {
                    return true;
                }

                if (cache.ContainsKey(remainingPattern) && !cache[remainingPattern])
                {
                    count++;
                    continue;
                }

                var result = HasPossibleCombination(remainingPattern, cache);

                if (result == true)
                {
                    if (!cache.ContainsKey(remainingPattern))
                    {
                        cache.Add(remainingPattern, true);
                    }
                    return true;
                }
                else if (!cache.ContainsKey(remainingPattern))
                {
                    cache.Add(remainingPattern, false);
                }
            }
            count++;
        }

        return false;
    }
}