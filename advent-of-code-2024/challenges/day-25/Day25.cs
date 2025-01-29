using System.Reflection.Metadata.Ecma335;

namespace advent_of_code_2024.challenges;

internal class Day25
{
    private static IList<Schematic> ReadSchematics()
    {
        var lines = Utils.ReadLinesFromChallengeSource(25);
        var schematics = new List<Schematic>();

        var currentPin = StartNewPin();
        PinDirection currentPinDirection = PinDirection.Undefined;

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                schematics.Add(new Schematic(currentPin, currentPinDirection));
                currentPin = StartNewPin();
                currentPinDirection = PinDirection.Undefined;
                continue;
            }

            if (currentPinDirection == PinDirection.Undefined)
            {
                if (line.Contains("."))
                {
                    currentPinDirection = PinDirection.FromBottom;
                }
                else
                {
                    currentPinDirection = PinDirection.FromTop;
                }
            }

            for (var i = 0; i < 5; i++)
            {
                if (line[i] == '#')
                {
                    currentPin[i] += 1;
                }
            }
        }

        schematics.Add(new Schematic(currentPin, currentPinDirection));

        return schematics;
    }

    private static IList<int> StartNewPin()
    {
        return [-1, -1, -1, -1, -1];
    }

    public static long ResolvePartOne()
    {
        var schematics = ReadSchematics();

        var schematicsFromTop = schematics.Where(s => s.PinDirection == PinDirection.FromTop);
        var schematicsFromBottom = schematics.Where(s => s.PinDirection == PinDirection.FromBottom);

        var count = 0;

        foreach (var schematicFromTop in schematicsFromTop)
        {
            foreach (var schematicFromBottom in schematicsFromBottom)
            {
                if (schematicFromTop.Matches(schematicFromBottom))
                {
                    count++;
                }
            }
        }

        return count;
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }

    class Schematic(IList<int> pin, PinDirection pinDirection)
    {
        public IList<int> Pin = pin;
        public PinDirection PinDirection = pinDirection;

        public bool Matches(Schematic anotherSchematic)
        {
            if (PinDirection == anotherSchematic.PinDirection)
            {
                return false;
            }

            for (var i = 0; i < 5; i++)
            {
                var number = Pin[i];
                var numberInAnotherSchematic = anotherSchematic.Pin[i];
                if (number + numberInAnotherSchematic <= 5)
                {
                    continue;
                }
                return false;
            }

            return true;
        }
    }

    enum PinDirection
    {
        FromTop,
        FromBottom,
        Undefined
    }
}