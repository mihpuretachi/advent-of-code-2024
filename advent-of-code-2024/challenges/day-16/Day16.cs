namespace advent_of_code_2024.challenges;

internal class Day16
{
    private static ReindeerMaze ReadMaze()
    {
        var lines = Utils.ReadLinesFromChallengeSource(16);

        IList<Coordinate> walls = [];
        Coordinate? start = null;
        Coordinate? end = null;

        for (int y = 0; y < lines.Count; y++)
        {
            var line = lines[y];

            for (var x = 0; x < line.Length; x++)
            {
                var content = line[x];

                switch (content)
                {
                    case '#':
                        walls.Add(new Coordinate(x, y));
                        break;
                    case '.':
                        break;
                    case 'S':
                        start = new Coordinate(x, y);
                        break;
                    case 'E':
                        end = new Coordinate(x, y);
                        break;
                    default:
                        throw new Exception($"Invalid content in maze: {content}");
                }

            }
        }

        return new ReindeerMaze(walls, start!, end!);
    }

    public static long ResolvePartOne()
    {
        return ReadMaze().CalculateShortestPathScore();
    }

    public static long ResolvePartTwo()
    {
        return ReadMaze().CalculateBestPathTiles();
    }
}

internal class ReindeerMaze
{
    public IList<Coordinate> Walls;
    Coordinate Start;
    Coordinate End;

    public ReindeerMaze(IList<Coordinate> walls, Coordinate start, Coordinate end)
    {
        Walls = walls;
        Start = start;
        End = end;
    }

    public long CalculateShortestPathScore()
    {
        // return CalculatePaths().MinBy(r => r.Score)!.Score;
        return 0;
    }

    public long CalculateBestPathTiles()
    {
        var paths = CalculatePaths();
        var bestPathScore = paths.Select(p => p.Score).Min();
        var bestPaths = paths.Where(p => p.Score == bestPathScore);
        return bestPaths.SelectMany(p => p.VisitedPoints).DistinctBy(p => p.Coordinate.GetHashCode()).Count();
    }

    private IList<MazeRunner> CalculatePaths()
    {
        var runner = new MazeRunner(Start, End, Direction.Right, 0, [new VisitedPoint(Start, 0, Direction.Right)], this);
        IList<MazeRunner> runners = [runner];
        IList<long> scores = [];

        while (runners.Any(r => r.CanWalk))
        {
            foreach (var currentRunner in runners.Where(r => r.CanWalk).ToList())
            {
                var newRunners = currentRunner.Split();
                runners.Remove(currentRunner);
                foreach (var newRunner in newRunners)
                {
                    if (newRunner.FoundDeadEndAt != null)
                    {
                        // Console.WriteLine($"A path found dead end at {newRunner.FoundDeadEndAt}");
                        continue;
                    }

                    var existingRunnerPassingInTheSamePath = runners.FirstOrDefault(r =>
                         r.VisitedPoints.Any(visitedPoint =>
                             visitedPoint.Coordinate.X == newRunner.CurrentPosition.X &&
                             visitedPoint.Coordinate.Y == newRunner.CurrentPosition.Y
                         )
                     );

                    if (existingRunnerPassingInTheSamePath != null)
                    {
                        var existingScore = existingRunnerPassingInTheSamePath.VisitedPoints
                            .Single(visitedPoint =>
                                visitedPoint.Coordinate.X == newRunner.CurrentPosition.X &&
                                visitedPoint.Coordinate.Y == newRunner.CurrentPosition.Y
                            ).Score;
                        if (existingScore >= newRunner.Score)
                        {
                            newRunner.AlternativePaths = newRunner.AlternativePaths
                                .Concat(ExtractAlternativePath(existingRunnerPassingInTheSamePath, newRunner.CurrentPosition))
                                .ToList();
                            runners.Remove(existingRunnerPassingInTheSamePath);
                            runners.Add(newRunner);
                        }
                    }
                    else
                    {
                        runners.Add(newRunner);
                    }
                }
            }
        }

        return runners.Where(r => r.FoundTarget).ToList();
    }

    private IEnumerable<VisitedPoint> ExtractAlternativePath(MazeRunner runner, Coordinate target)
    {
        IList<VisitedPoint> alternativePath = [];
        foreach (var visitedPoint in runner.VisitedPoints)
        {
            alternativePath.Add(visitedPoint);
            if (visitedPoint.Coordinate.X == target.X && visitedPoint.Coordinate.Y == target.Y)
            {
                break;
            }
        }
        return alternativePath;
    }
}

record VisitedPoint(Coordinate Coordinate, long Score, Direction Direction) { }

internal class MazeRunner(
    Coordinate currentPosition,
    Coordinate targetPosition,
    Direction currentDirection,
    long score,
    IList<VisitedPoint> visitedPoints,
    ReindeerMaze maze)
{
    public long Score = score;
    public bool FoundTarget = false;
    public Coordinate? FoundDeadEndAt;
    public Coordinate CurrentPosition = currentPosition;
    public Direction CurrentDirection = currentDirection;
    public IList<VisitedPoint> VisitedPoints = visitedPoints;
    public IList<VisitedPoint> AlternativePaths = [];

    public bool CanWalk => !FoundTarget && FoundDeadEndAt == null;

    public IList<MazeRunner> Split()
    {
        var nextCoordinates = new List<(Coordinate, long, Direction)> {
                (new Coordinate(CurrentPosition.X + 1, CurrentPosition.Y), CurrentDirection == Direction.Right ? 1 : 1001, Direction.Right),
                (new Coordinate(CurrentPosition.X - 1, CurrentPosition.Y), CurrentDirection == Direction.Left ? 1 : 1001, Direction.Left),
                (new Coordinate(CurrentPosition.X, CurrentPosition.Y + 1), CurrentDirection == Direction.Down ? 1 : 1001, Direction.Down),
                (new Coordinate(CurrentPosition.X, CurrentPosition.Y - 1), CurrentDirection == Direction.Up ? 1 : 1001, Direction.Up),
            };

        nextCoordinates = nextCoordinates
            .Where(pair =>
            {
                var coordinate = pair.Item1;
                return !maze.Walls.Any(wall => wall.X == coordinate.X && wall.Y == coordinate.Y) &&
                    !VisitedPoints.Any(visitedPoint => visitedPoint.Coordinate.X == coordinate.X && visitedPoint.Coordinate.Y == coordinate.Y);
            }).ToList();

        if (nextCoordinates.Count == 0)
        {
            // Dead end
            FoundDeadEndAt = CurrentPosition;
            return [this];
        }

        return nextCoordinates.Select(pair =>
        {
            var coordinate = pair.Item1;
            var newScore = Score + pair.Item2;
            var direction = pair.Item3;

            var newRunner = new MazeRunner(
                coordinate,
                targetPosition,
                direction,
                newScore,
                VisitedPoints.Append(new VisitedPoint(coordinate, newScore, direction)).ToList(),
                maze);

            newRunner.AlternativePaths = AlternativePaths.ToList();

            if (coordinate.X == targetPosition.X && coordinate.Y == targetPosition.Y)
            {
                newRunner.FoundTarget = true;
            }

            return newRunner;

        }).ToList();
    }
}
