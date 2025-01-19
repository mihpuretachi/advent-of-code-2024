namespace advent_of_code_2024.challenges;

internal class Day11
{
    static Arrangement CreateArrangement()
    {
        var source = Utils.ReadChallengeSource(11);
        return new Arrangement(source.Split(" ").Select(int.Parse).ToList());
    }

    public static double ResolvePartOne()
    {
        var arrangement = CreateArrangement();
        return arrangement.Blink(25);
    }

    public static double ResolvePartTwo()
    {
        var arrangement = CreateArrangement();
        return arrangement.Blink(75);
    }
}

class Arrangement(IList<int> pebbles)
{
    public IList<int> Pebbles = pebbles;

    // Key: (pebble, times)
    // Value: Sum of pebbles
    private IDictionary<(long, int), long> cache = new Dictionary<(long, int), long>();

    public long Blink(int times)
    {
        return Pebbles.Select(p => CalculatePebblesAfterBlink(p, times)).Sum();
    }

    private long CalculatePebblesAfterBlink(long pebble, int times)
    {

        if (times == 0)
        {
            return 1;
        }
        else if (cache.TryGetValue((pebble, times), out var cachedResult))
        {
            return cachedResult;
        }
        else
        {
            long result;

            if (pebble == 0)
            {
                result = CalculatePebblesAfterBlink(1, times - 1);
            }
            else
            {
                var numberString = pebble.ToString();
                if (numberString.Length % 2 == 0)
                {
                    var halfSize = numberString.Length / 2;
                    var leftNumber = int.Parse(numberString.Substring(0, halfSize));
                    var rightNumber = int.Parse(numberString.Substring(halfSize, halfSize));
                    result = CalculatePebblesAfterBlink(leftNumber, times - 1) + CalculatePebblesAfterBlink(rightNumber, times - 1);
                }
                else
                {
                    result = CalculatePebblesAfterBlink(pebble * 2024, times - 1);
                }
            }

            cache.TryAdd((pebble, times), result);
            return result;
        }
    }
}
