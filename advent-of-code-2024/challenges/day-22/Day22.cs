namespace advent_of_code_2024.challenges;

internal class Day22
{   
    internal static IList<long> ReadSecrets()
    {
        return Utils.ReadLinesFromChallengeSource(22)
            .Select(v => long.Parse(v))
            .ToList();
    }



    public static long ResolvePartOne()
    {
        var secrets = ReadSecrets();

        for (var i = 0; i < 2000; i++)
        {
            secrets = secrets.AsParallel().Select(CalculateNextSecret).ToList();
        }

        return secrets.Sum();
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }

    private static long CalculateNextSecret(long secret) {
        var result = Prune(Mix(secret * 64, secret));

        result = Prune(Mix((long)Math.Floor((double)result / 32) , result));

        result = Prune(Mix(result * 2048, result));

        return result;
    }

    private static long Mix(long previousValue, long currentValue)
    {
        return currentValue ^ previousValue;
    }

    private static long Prune(long value)
    {
        return value % 16777216;
    }
}