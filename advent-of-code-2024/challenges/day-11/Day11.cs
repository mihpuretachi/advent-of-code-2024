namespace advent_of_code_2024.challenges;

internal class Day11
{
    static Arrangement CreateArrangement()
    {
        var source = Utils.ReadChallengeSource(11);
        return new Arrangement(source.Split(" ").Select(v => new Pebble(long.Parse(v))).ToList());
    }

    public static double ResolvePartOne()
    {
        var arrangement = CreateArrangement();
        for (int i = 0; i < 25; i++)
        {
            //Console.WriteLine($"After {i} blinks: {arrangement.PebblesString}");
            Console.WriteLine($"Blink: {i}");
            arrangement.Blink();
        }
        return arrangement.Pebbles.Count;
    }

    public static double ResolvePartTwo()
    {
        var arrangement = CreateArrangement();
        for (int i = 1; i <= 75; i++)
        {
            Console.WriteLine($"Blinking for the time {i}. {arrangement.Pebbles.Count} pebbles by now.");
            arrangement.Blink();
        }
        return arrangement.Pebbles.Count;
    }
}

class Arrangement(IList<Pebble> pebbles)
{
    public IList<Pebble> Pebbles = pebbles;

    public string PebblesString = "." + String.Join(".", pebbles.Select(p => p.Number.ToString())) + ".";

    public void Blink()
    {
        for (int i = 0; i < Pebbles.Count; i++)
        {
            var pebble = Pebbles[i];
            if (pebble.Number == 0)
            {
                pebble.Number = 1;
            }
            else
            {
                var numberString = pebble.Number.ToString();
                if (numberString.Length % 2 == 0)
                {
                    Pebbles.Remove(pebble);
                    var halfSize = numberString.Length / 2;
                    Pebbles.Insert(i, new Pebble(long.Parse(numberString.Substring(0, halfSize))));
                    Pebbles.Insert(i + 1, new Pebble(long.Parse(numberString.Substring(halfSize, halfSize))));
                    i++;
                }
                else
                {
                    pebble.Number *= 2024;
                }
            }
        }
    }

    public void Blink2()
    {
        var lastSeparatorIndex = 0;
        for (var i = 1; i < PebblesString.Length; i++)
        {
            var value = PebblesString[i];
            if (value == 0)
            {
                PebblesString = PebblesString.Remove(i);
                PebblesString = PebblesString.Insert(i, "1");
            }
            else if (value == '.')
            {
                var distanceSinceLastSeparator = i - lastSeparatorIndex - 1;

                if (distanceSinceLastSeparator % 2 == 0)
                {
                    var halfSize = distanceSinceLastSeparator / 2;
                    var newSeparatorIndex = i - halfSize;
                    PebblesString = PebblesString.Insert(newSeparatorIndex, ".");
                    i++;
                    var newNumberLeftString = PebblesString.Substring(lastSeparatorIndex + 1, halfSize);
                    var newNumberRightString = PebblesString.Substring(newSeparatorIndex + 1, halfSize);

                    var sizeBeforeReplace = PebblesString.Length;
                    var newNumberLeft = long.Parse(newNumberLeftString);
                    PebblesString = PebblesString.Remove(lastSeparatorIndex + 1, halfSize);
                    PebblesString = PebblesString.Insert(lastSeparatorIndex + 1, newNumberLeft.ToString());

                    var diff = sizeBeforeReplace - PebblesString.Length;
                    newSeparatorIndex = newSeparatorIndex - diff;
                    sizeBeforeReplace = PebblesString.Length;
                    var newNumberRight = long.Parse(newNumberRightString);

                    PebblesString = PebblesString.Remove(newSeparatorIndex + 1, halfSize);
                    PebblesString = PebblesString.Insert(newSeparatorIndex + 1, newNumberRight.ToString());

                    i -= sizeBeforeReplace - PebblesString.Length;
                }
                else
                {
                    var number = long.Parse(PebblesString.Substring(lastSeparatorIndex + 1, i - lastSeparatorIndex - 1));
                    if (number == 0)
                    {
                        PebblesString = PebblesString.Remove(lastSeparatorIndex + 1, 1);
                        PebblesString = PebblesString.Insert(lastSeparatorIndex + 1, 1.ToString());
                    }
                    else
                    {
                        var newNumber = number * 2024;
                        var differenceInSize = newNumber.ToString().Length - number.ToString().Length;
                        PebblesString = PebblesString.Remove(lastSeparatorIndex + 1, number.ToString().Length);
                        PebblesString = PebblesString.Insert(lastSeparatorIndex + 1, newNumber.ToString());
                        i += differenceInSize;
                    }
                }

                lastSeparatorIndex = i;
            }
        }

    }
}

class Pebble(long number)
{
    public long Number = number;
}
