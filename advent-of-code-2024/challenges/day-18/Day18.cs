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
        //return ReadMemoryMaze(70).CalculateShortestPath(1024);
        return ReadMemoryMaze(7).CalculateShortestPath(12);
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }
}

internal class MemoryMaze(IList<Coordinate> corruptedSpaces, int size)
{
    IList<Coordinate> CorruptedSpaces = corruptedSpaces;
    public int Size = size;
    Coordinate Start = new Coordinate(0, 0);
    Coordinate End = new Coordinate(size - 1, size - 1);

    public long CalculateShortestPath(int corruptedMemoryQuantity)
    {
        var shortestPath = CalculatePaths(CorruptedSpaces.Take(corruptedMemoryQuantity).ToList())
            .MinBy(r => r.VisitedPoints.Count)!;

        return shortestPath.VisitedPoints.Count - 1;
    }

    private IList<MemoryRunner> CalculatePaths(IList<Coordinate> corruptedSpaces)
    {
        var runner = new MemoryRunner(Start, End, [new VisitedMemoryPoint(Start, 0)], corruptedSpaces, Size);
        IList<MemoryRunner> runners = [runner];

        while (runners.Any(r => r.CanWalk))
        {
            IList<MemoryRunner> runnersToBeDeleted = [];
            IList<MemoryRunner> runnersToBeAdded = [];

            foreach (var currentRunner in runners)
            {
                var newRunners = currentRunner.GoAhead();
                runnersToBeDeleted.Add(currentRunner);
                foreach (var newRunner in newRunners)
                {
                    Console.WriteLine($"Passing through point {newRunner.CurrentPosition}");

                    if (newRunner.FoundDeadEndAt != null)
                    {
                        continue;
                    }

                    // runnersToBeDeleted = runnersToBeDeleted.Concat(runners.Where(r => !r.IsBetterThan(newRunner))).ToList();

                    foreach (var existingRunner in runners)
                    {
                        if (newRunner.IsBetterThan(existingRunner))
                        {
                            runnersToBeDeleted.Add(existingRunner);
                        }
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

        return runners.Where(r => r.FoundTarget).ToList();
    }
}

internal class MemoryRunner(
    Coordinate currentPosition,
    Coordinate targetPosition,
    IList<VisitedMemoryPoint> visitedPoints,
    IList<Coordinate> corruptedSpaces,
    int mazeSize)
{
    public bool FoundTarget = false;
    public Coordinate? FoundDeadEndAt;
    public Coordinate CurrentPosition = currentPosition;
    public IList<VisitedMemoryPoint> VisitedPoints = visitedPoints;

    public bool CanWalk => !FoundTarget && FoundDeadEndAt == null;

    public IList<MemoryRunner> GoAhead()
    {
        var nextCoordinates = new List<Coordinate> {
                new Coordinate(CurrentPosition.X + 1, CurrentPosition.Y),
                new Coordinate(CurrentPosition.X - 1, CurrentPosition.Y),
                new Coordinate(CurrentPosition.X, CurrentPosition.Y + 1),
                new Coordinate(CurrentPosition.X, CurrentPosition.Y - 1),
            };

        nextCoordinates = nextCoordinates
            .Where(coordinate =>
            {
                return coordinate.X >= 0 &&
                    coordinate.Y >= 0 &&
                    coordinate.X < mazeSize &&
                    coordinate.Y < mazeSize &&
                    !corruptedSpaces.Any(space => space.Equals(coordinate)) &&
                    !VisitedPoints.Any(visitedPoint => visitedPoint.Coordinate.Equals(coordinate));
            }).ToList();

        if (nextCoordinates.Count == 0)
        {
            FoundDeadEndAt = CurrentPosition;
            return [this];
        }

        return nextCoordinates.Select(coordinate =>
        {
            var newRunner = new MemoryRunner(
                coordinate,
                targetPosition,
                VisitedPoints.Append(new VisitedMemoryPoint(coordinate, VisitedPoints.Count)).ToList(),
                corruptedSpaces,
                mazeSize);


            if (coordinate.Equals(targetPosition))
            {
                newRunner.FoundTarget = true;
            }

            return newRunner;

        }).ToList();
    }

    public bool IsBetterThan(MemoryRunner other)
    {
        var foundAnyIntersection = false;

        foreach (var visitedPointInOther in other.VisitedPoints)
        {
            var visitedPoint = VisitedPoints.FirstOrDefault(p => p.Coordinate.Equals(visitedPointInOther.Coordinate));
            if (visitedPoint != null)
            {
                foundAnyIntersection = true;
                if (visitedPointInOther.VisitedPointsTillThere < visitedPoint.VisitedPointsTillThere)
                {
                    return false;
                }
            }
        }

        return foundAnyIntersection;
    }
}
internal record VisitedMemoryPoint(Coordinate Coordinate, int VisitedPointsTillThere) { }