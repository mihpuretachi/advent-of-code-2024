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
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }
}

internal class MemoryMaze(IList<Coordinate> corruptedSpaces, int size)
{
    private IList<Coordinate> CorruptedSpaces = corruptedSpaces;
    private Coordinate Start = new(0, 0);
    private Coordinate End = new(size, size);

    public int Size = size;

    public long CalculateShortestPath(int corruptedMemoryQuantity)
    {
        var corruptedSpaces = CorruptedSpaces.Take(corruptedMemoryQuantity).ToList();

        // Print(corruptedSpaces);

        var runner = new MemoryRunner(Start, End, 0, corruptedSpaces, Size);
        IList<MemoryRunner> runners = [runner];
        IList<Coordinate> allVisitedPoints = [new Coordinate(0, 0)];

        while (!runners.Any(r => r.FoundTarget) && runners.Any(r => r.CanWalk))
        {
            IList<MemoryRunner> runnersToBeDeleted = [];
            IList<MemoryRunner> runnersToBeAdded = [];

            foreach (var currentRunner in runners)
            {
                var newRunners = currentRunner.GoAhead(allVisitedPoints);

                if (!currentRunner.FoundTarget)
                {
                    runnersToBeDeleted.Add(currentRunner);
                }

                foreach (var newRunner in newRunners)
                {
                    if (allVisitedPoints.Contains(newRunner.CurrentPosition))
                    {
                        continue;
                    }

                    Console.WriteLine($"Passing through point {newRunner.CurrentPosition}");

                    allVisitedPoints.Add(newRunner.CurrentPosition);

                    if (newRunner.FoundDeadEndAt != null)
                    {
                        continue;
                    }

                    runnersToBeAdded.Add(newRunner);
                }
            }

            foreach (var runnerToBeDeleted in runnersToBeDeleted)
            {
                runners.Remove(runnerToBeDeleted);
            }

            foreach (var runnerToBeAdded in runnersToBeAdded)
            {
                runners.Add(runnerToBeAdded);
            }
        }

        var shortestPath = runners.FirstOrDefault(r => r.FoundTarget);

        return shortestPath!.VisitedPoints;
    }

    private void Print(IList<Coordinate> corruptedSpaces)
    {
        for (var y = 0; y < Size; y++)
        {
            var line = "";
            for (var x = 0; x < Size; x++)
            {
                var corruptedSpace = corruptedSpaces.FirstOrDefault(s => s.X == x && s.Y == y);
                if (corruptedSpace != null)
                {
                    line += "#";
                    continue;
                }

                line += ".";
            }
            Console.WriteLine(line);
        }
    }
}

internal class MemoryRunner
{
    private int MazeSize;
    private IList<Coordinate> CorruptedSpaces;
    private Coordinate TargetPosition;

    public Coordinate? FoundDeadEndAt;
    public Coordinate CurrentPosition;
    public int VisitedPoints;

    public MemoryRunner(
        Coordinate currentPosition,
        Coordinate targetPosition,
        int visitedPoints,
        IList<Coordinate> corruptedSpaces,
        int mazeSize)
    {
        CurrentPosition = currentPosition;
        TargetPosition = targetPosition;
        VisitedPoints = visitedPoints;
        CorruptedSpaces = corruptedSpaces;
        MazeSize = mazeSize;
    }

    public bool CanWalk => !FoundTarget && FoundDeadEndAt == null;
    public bool FoundTarget => CurrentPosition.Equals(TargetPosition);

    public IList<MemoryRunner> GoAhead(IList<Coordinate> allVisitedPoints)
    {
        var nextCoordinates = new List<Coordinate> {
                new Coordinate(CurrentPosition.X + 1, CurrentPosition.Y),
                new Coordinate(CurrentPosition.X, CurrentPosition.Y + 1),
                new Coordinate(CurrentPosition.X - 1, CurrentPosition.Y),
                new Coordinate(CurrentPosition.X, CurrentPosition.Y - 1),
            };

        nextCoordinates = nextCoordinates
            .Where(coordinate =>
            {
                return coordinate.X >= 0 &&
                    coordinate.Y >= 0 &&
                    coordinate.X <= MazeSize &&
                    coordinate.Y <= MazeSize &&
                    !CorruptedSpaces.Any(space => space.Equals(coordinate)) &&
                    !allVisitedPoints.Any(visitedPoint => visitedPoint.Equals(coordinate));
            }).ToList();

        if (nextCoordinates.Count == 0)
        {
            FoundDeadEndAt = CurrentPosition;
            return [this];
        }

        var finalCoordinate = nextCoordinates.FirstOrDefault(c => c.Equals(TargetPosition));

        if (finalCoordinate != null)
        {
            var newRunner = new MemoryRunner(
               finalCoordinate,
               TargetPosition,
               VisitedPoints + 1,
               CorruptedSpaces,
               MazeSize);

            return [newRunner];
        }

        return nextCoordinates.Select(coordinate =>
            new MemoryRunner(
                coordinate,
                TargetPosition,
                VisitedPoints + 1,
                CorruptedSpaces,
                MazeSize))
            .ToList();
    }
}

internal record VisitedMemoryPoint(Coordinate Coordinate, int VisitedPointsTillThere) { }