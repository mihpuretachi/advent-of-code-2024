namespace advent_of_code_2024.challenges;

internal class Day19
{
    static (IList<string>, IList<string>) ReadTowelsSetup()
    {
        var lines = Utils.ReadLinesFromChallengeSource(19);

        IList<string> availableTowels = [];
        IList<string> requests = [];

        for (int i = 0; i < lines.Count; i++)
        {
            if (i == 1)
            {
                continue;
            }

            var line = lines[i];

            if (i == 0)
            {
                availableTowels = line.Split(", ").ToList();
                continue;
            }

            requests.Add(line);
        }

        return (availableTowels, requests);
    }

    public static long ResolvePartOne()
    {
        var (availableTowels, requests) = ReadTowelsSetup();

        return requests.Count(r => HasCombination(r, availableTowels, new Dictionary<string, bool>()));
    }

    public static long ResolvePartTwo()
    {
        var (availableTowels, requests) = ReadTowelsSetup();

        var combinations = requests.Select(request => CountCombinations(request, availableTowels, new Dictionary<string, long>()));

        return combinations.Sum();
    }

    static bool HasCombination(string request, IList<string> availableTowels, IDictionary<string, bool> cache)
    {
        if (cache.TryGetValue(request, out var cachedResult))
        {
            return cachedResult;
        }

        foreach (var towel in availableTowels)
        {
            if (request.StartsWith(towel))
            {
                if (request.Length == towel.Length)
                {
                    cache.TryAdd(request, true);
                    return true;
                }
                else
                {
                    var result = HasCombination(request.Substring(towel.Length), availableTowels, cache);
                    cache.TryAdd(request, result);
                    
                    if (!result)
                    {
                        continue;
                    }
                    
                    return true;
                }
            }
        }

        cache.TryAdd(request, false);
        return false;
    }

    static long CountCombinations(string request, IList<string> availableTowels, IDictionary<string, long> cache)
    {
        if (cache.TryGetValue(request, out var cachedResult))
        {
            return cachedResult;
        }

        long result = 0;

        foreach (var towel in availableTowels)
        {
            if (request.StartsWith(towel))
            {
                if (request.Length == towel.Length)
                {
                    result += 1;
                }
                else
                {
                    result += CountCombinations(request.Substring(towel.Length), availableTowels, cache);
                }
            }
        }

        cache.TryAdd(request, result);

        return result;
    }
}