using advent_of_code_2024.common;

namespace advent_of_code_2024.challenges;

internal class Day18
{
    static MemoryMaze ReadMemoryMaze(int size)
    {
        var lines = Utils.ReadLinesFromChallengeSource(18);
        IList<Coordinate> corruptedSpaces = [];

        foreach (var line in lines)
        {
            var values = line.Split(',');
            corruptedSpaces.Add(new Coordinate(int.Parse(values[0]), int.Parse(values[1])));
        }

        return new MemoryMaze(corruptedSpaces, size);
    }

    public static long ResolvePartOne()
    {
        return ReadMemoryMaze(70).CalculateShortestPath(1024);
        // return ReadMemoryMaze(6).CalculateShortestPath(12);
    }

    public static string ResolvePartTwo()
    {
        return ReadMemoryMaze(70).CalculateMinimalCorruptedMemoryToBlockExit(1024).ToString();
        // return ReadMemoryMaze(6).CalculateMinimalCorruptedMemoryToBlockExit(12).ToString();
    }
}

internal class MemoryMaze : Maze
{
    public MemoryMaze(IList<Coordinate> corruptedSpaces, int size)
        : base(corruptedSpaces, size, new Coordinate(0, 0), new(size, size))
    {
    }

    public long CalculateShortestPath(int corruptedMemoryQuantity)
    {
        var corruptedSpaces = Walls.Take(corruptedMemoryQuantity).ToList();
        var startingRunner = new BasicRunner(Start, End, 0, corruptedSpaces, Size);

        var runners = RunTillFindExit(startingRunner, corruptedSpaces);

        var shortestPath = runners.FirstOrDefault(r => r.FoundTarget);

        return shortestPath!.VisitedPoints;
    }

    public Coordinate CalculateMinimalCorruptedMemoryToBlockExit(int initialCorruptedMemoryQuantity)
    {
        var corruptedMemoryQuantity = initialCorruptedMemoryQuantity;
        IList<Runner> runners = [];

        do
        {
            corruptedMemoryQuantity++;

            var corruptedSpaces = Walls.Take(corruptedMemoryQuantity).ToList();
            var startingRunner = new BasicRunner(Start, End, 0, corruptedSpaces, Size);

            runners = RunTillFindExit(startingRunner, corruptedSpaces);
        } while (runners.Count > 0);

        return Walls.Skip(corruptedMemoryQuantity - 1).Take(1).First();
    }
}

