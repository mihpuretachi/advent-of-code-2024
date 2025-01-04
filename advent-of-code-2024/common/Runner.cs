namespace advent_of_code_2024.common;

abstract class Runner
{
    protected int MazeSize;
    protected IList<Coordinate> Walls;
    protected Coordinate TargetPosition;

    public Coordinate? FoundDeadEndAt;
    public Coordinate CurrentPosition;
    public int VisitedPoints;

    public Runner(
        Coordinate currentPosition,
        Coordinate targetPosition,
        int visitedPoints,
        IList<Coordinate> walls,
        int mazeSize)
    {
        CurrentPosition = currentPosition;
        TargetPosition = targetPosition;
        VisitedPoints = visitedPoints;
        Walls = walls;
        MazeSize = mazeSize;
    }

    public bool CanWalk => !FoundTarget && FoundDeadEndAt == null;
    public bool FoundTarget => CurrentPosition.Equals(TargetPosition);

    public IList<Runner> Split(IList<Coordinate> allVisitedPoints)
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
                    !Walls.Any(space => space.Equals(coordinate)) &&
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
            var newRunner = CreateRunner(
               finalCoordinate,
               TargetPosition,
               VisitedPoints + 1,
               Walls,
               MazeSize);

            return [newRunner];
        }

        return nextCoordinates.Select(coordinate =>
            CreateRunner(
                coordinate,
                TargetPosition,
                VisitedPoints + 1,
                Walls,
                MazeSize))
            .ToList();
    }

    protected abstract Runner CreateRunner(
        Coordinate currentCoordinate, 
        Coordinate targetCoordinate,
        int visitedPoints,
        IList<Coordinate> walls,
        int mazeSize);
}

internal class BasicRunner : Runner
{
    public BasicRunner(
        Coordinate currentPosition,
        Coordinate targetPosition,
        int visitedPoints,
        IList<Coordinate> walls,
        int mazeSize)
    : base(currentPosition, targetPosition, visitedPoints, walls, mazeSize)
    { }

    protected override Runner CreateRunner(Coordinate currentCoordinate, Coordinate targetCoordinate, int visitedPoints, IList<Coordinate> walls, int mazeSize)
    {
        return new BasicRunner(
            currentCoordinate,
            targetCoordinate,
            visitedPoints,
            walls,
            mazeSize);
    }
}