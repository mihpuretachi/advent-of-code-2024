namespace advent_of_code_2024.challenges;

internal class Day8
{
    static AntennaMap ReadMap()
    {
        var source = Utils.ReadLinesFromChallengeSource(8);

        var map = new char[source.Count, source.Count];

        for (var rowIndex = 0; rowIndex < source.Count; rowIndex++)
        {
            var row = source[rowIndex];
            for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                var charValue = row[columnIndex];
                map.SetValue(charValue, columnIndex, rowIndex);
            }
        }
        return new AntennaMap(map);
    }

    public static double ResolvePartOne()
    {
        var map = ReadMap();
        map.IdentifyAntinodes();
        return map.Antinodes.Distinct().Count();
    }

    public static double ResolvePartTwo()
    {
        return 0;
    }
}

class AntennaMap(char[,] source)
{
    public IList<Antinode> Antinodes = new List<Antinode>();

    public void IdentifyAntinodes()
    {
        var map = new Dictionary<char, IList<(int, int)>>();

        for (int col = 0; col < source.GetLength(0); col++)
        {
            for (int row = 0; row < source.GetLength(1); row++)
            {
                var value = source[col, row];
                if (value != '.')
                {
                    if (map.ContainsKey(value))
                    {
                        map[value].Add((col, row));
                    }
                    else
                    {
                        map.Add(value, [(col, row)]);
                    }
                }
            }
        }

        foreach (var key in map.Keys)
        {
            var antennaList = map[key];
            foreach (var antenna in antennaList)
            {
                foreach (var otherAntenna in antennaList)
                {
                    if (antenna != otherAntenna)
                    {
                        Antinodes = Antinodes.Concat(CreateAntinodesBetween(antenna, otherAntenna)).ToList();
                    }
                }
            }
        }
    }

    private IList<Antinode> CreateAntinodesBetween((int, int) positionA, (int, int) positionB)
    {
        var antinodes = new List<Antinode>();

        var distanceX = Math.Abs(positionA.Item1 - positionB.Item1);
        var distanceY = Math.Abs(positionA.Item2 - positionB.Item2);

        if (positionA.Item1 < positionB.Item1 && positionA.Item2 < positionB.Item2)
        {
            // A . .  
            // . . . 
            // . . B
            var antinode1 = (positionA.Item1 - distanceX, positionA.Item2 - distanceY);
            antinodes.Add(new Antinode(antinode1));

            var antinode2 = (positionB.Item1 + distanceX, positionB.Item2 + distanceY);
            antinodes.Add(new Antinode(antinode2));
        }
        else if (positionA.Item1 > positionB.Item1 && positionA.Item2 < positionB.Item2)
        {
            // . . A
            // . . .
            // B . .
            var antinode1 = (positionA.Item1 + distanceX, positionA.Item2 - distanceY);
            antinodes.Add(new Antinode(antinode1));

            var antinode2 = (positionB.Item1 - distanceX, positionB.Item2 + distanceY);
            antinodes.Add(new Antinode(antinode2));
        }
        else if (positionA.Item1 < positionB.Item1 && positionA.Item2 > positionB.Item2)
        {
            // . . B
            // . . .
            // A . . 
            var antinode1 = (positionA.Item1 - distanceX, positionA.Item2 + distanceY);
            antinodes.Add(new Antinode(antinode1));

            var antinode2 = (positionB.Item1 + distanceX, positionB.Item2 - distanceY);
            antinodes.Add(new Antinode(antinode2));
        }
        else if (positionA.Item1 > positionB.Item1 && positionA.Item2 > positionB.Item2)
        {
            // B . .
            // . . .
            // . . A
            var antinode1 = (positionB.Item1 - distanceX, positionB.Item2 - distanceY);
            antinodes.Add(new Antinode(antinode1));

            var antinode2 = (positionA.Item1 + distanceX, positionA.Item2 + distanceY);
            antinodes.Add(new Antinode(antinode2));
        }
        else if (positionA.Item1 == positionB.Item1)
        {
            // . A .
            // . . .
            // . B .

            var antinode1 = (positionA.Item1, Math.Min(positionA.Item2, positionB.Item2) - distanceY);
            antinodes.Add(new Antinode(antinode1));

            var antinode2 = (positionA.Item1, Math.Max(positionA.Item2, positionB.Item2) + distanceY);
            antinodes.Add(new Antinode(antinode2));
        }
        else
        {
            // . . .
            // A . B
            // . . .

            var antinode1 = (Math.Min(positionA.Item1, positionB.Item1) - distanceX, positionA.Item2);
            antinodes.Add(new Antinode(antinode1));

            var antinode2 = (Math.Max(positionA.Item2, positionB.Item2) + distanceX, positionA.Item2);
            antinodes.Add(new Antinode(antinode2));
        }

        return antinodes.Where(a => !IsOutOfBounds(a.Position)).ToList();
    }

    public void PrintAntinodes()
    {
        foreach (var antinode in Antinodes)
        {
            Console.WriteLine(antinode);
        }
    }

    bool IsOutOfBounds((int, int) position)
    {
        return position.Item1 < 0 ||
            position.Item1 >= source.GetLength(0) ||
            position.Item2 < 0 ||
            position.Item2 >= source.GetLength(1);
    }
}

class Antinode((int, int) position)
{
    public (int, int) Position = position;

    public override bool Equals(object? other)
    {
        return other != null &&
            other is Antinode otherAntinode &&
            Position.Item1 == otherAntinode.Position.Item1 &&
            Position.Item2 == otherAntinode.Position.Item2;
    }

    public override int GetHashCode()
    {
        return Position.Item1.GetHashCode() * Position.Item2.GetHashCode() * 13;
    }

    public override string ToString()
    {
        return $"({Position.Item1}, {Position.Item2})";
    }
}
