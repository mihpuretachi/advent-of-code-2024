namespace advent_of_code_2024.challenges;

internal class Day6
{
    static (char[,], Coordinate) ReadSetup()
    {
        var source = Utils.ReadLinesFromChallengeSource(6);

        Coordinate? initialPosition = null;
        var map = new char[source.Count, source.Count];

        for (var rowIndex = 0; rowIndex < source.Count; rowIndex++)
        {
            var row = source[rowIndex];
            for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                var charValue = row[columnIndex];
                map.SetValue(charValue, rowIndex, columnIndex);
                if (charValue == '^')
                {
                    initialPosition = new Coordinate(columnIndex, rowIndex);
                }
            }
        }
        return (map, initialPosition!);
    }

    public static double ResolvePartOne()
    {
        var (source, initialCoordinate) = ReadSetup();
        var map = new Map(source, initialCoordinate);
        map.WalkTillPossible();
        map.Print();
        return map.CountVisitedPositions();
    }

    public static double ResolvePartTwo()
    {
        IList<Coordinate> obstaclesAddedStartingLoop = [];

        var (source, initialCoordinate) = ReadSetup();
        var originalMap = new Map(source, initialCoordinate);

        originalMap.WalkTillPossible();

        IList<Coordinate> possibleAttemptsCoordinates = [];

        for (var rowIndex = 0; rowIndex < source.GetLength(0); rowIndex++)
        {
            var row = Utils.GetRow(source, rowIndex);
            for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                var charValue = row[columnIndex];
                var coordinate = new Coordinate(columnIndex, rowIndex);
                if (charValue != '#')
                {
                    possibleAttemptsCoordinates.Add(coordinate);
                }
            }
        }

        var count = 0;

        foreach (var chunk in possibleAttemptsCoordinates.Chunk(130))
        {
            chunk.AsParallel().ForAll(attempt =>
            {
                if (TestAttempt(originalMap, attempt) == EndWalkReason.Loop)
                {
                    count++;
                }
            });
        }

        return count;
    }

    static EndWalkReason TestAttempt(Map originalMap, Coordinate newObstaclePosition)
    {
        Console.WriteLine($"Testing adding obstacle in coordinate {newObstaclePosition}");
        var copy = originalMap.CloneInitialState();
        copy.PlaceObstacleAt(newObstaclePosition);
        return copy.WalkTillPossible();
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

enum EndWalkReason
{
    Outbound,
    Loop,
}

class Map
{
    private readonly char[,] source;
    private readonly IList<Step> steps = new List<Step>();
    private readonly Coordinate initialPosition;
    private readonly Direction initialDirection = Direction.Up;

    private Coordinate? nextCoordinate = null;
    private Direction nextDirection;

    public Map(char[,] source, Coordinate initialPosition)
    {
        this.source = source;
        this.initialPosition = initialPosition;
    }

    public EndWalkReason WalkTillPossible()
    {
        return GoAheadTillOuboundOrLoop(initialPosition, initialDirection);
    }

    private void CalculateNextStep(Coordinate currentCoordinate, Direction currentDirection)
    {
        var isPossibleMovement = false;

        Coordinate? possibleNextCoordinate = null;

        while (!isPossibleMovement)
        {
            switch (currentDirection)
            {
                case Direction.Up:
                    possibleNextCoordinate = new Coordinate(currentCoordinate.X, currentCoordinate.Y - 1);
                    break;
                case Direction.Down:
                    possibleNextCoordinate = new Coordinate(currentCoordinate.X, currentCoordinate.Y + 1);
                    break;
                case Direction.Left:
                    possibleNextCoordinate = new Coordinate(currentCoordinate.X - 1, currentCoordinate.Y);
                    break;
                case Direction.Right:
                    possibleNextCoordinate = new Coordinate(currentCoordinate.X + 1, currentCoordinate.Y);
                    break;
                default: throw new ArgumentException("Invalid Direction");
            }

            if (IsOutOfBounds(possibleNextCoordinate!))
            {
                nextCoordinate = possibleNextCoordinate;
                isPossibleMovement = true;
            }
            else
            {
                var contentAhead = GetContent(possibleNextCoordinate!);
                if (contentAhead == '#')
                {
                    // Found obstacle, must turn right
                    switch (currentDirection)
                    {
                        case Direction.Up:
                            currentDirection = Direction.Right;
                            break;
                        case Direction.Down:
                            currentDirection = Direction.Left;
                            break;
                        case Direction.Left:
                            currentDirection = Direction.Up;
                            break;
                        case Direction.Right:
                            currentDirection = Direction.Down;
                            break;
                        default: throw new ArgumentException("Invalid Direction");
                    }
                }
                else
                {
                    nextCoordinate = possibleNextCoordinate;
                    isPossibleMovement = true;
                }
            }
        }

        nextDirection = currentDirection;
    }

    EndWalkReason GoAheadTillOuboundOrLoop(Coordinate currentCoordinate, Direction currentDirection)
    {
        source.SetValue('X', currentCoordinate.Y, currentCoordinate.X);

        CalculateNextStep(currentCoordinate, currentDirection);

        if (IsOutOfBounds(nextCoordinate!))
        {
            return EndWalkReason.Outbound;
        }

        var newStep = new Step(currentCoordinate, nextCoordinate!);
        if (steps.Any(s => s.Equals(newStep)))
        {
            return EndWalkReason.Loop;
        }
        steps.Add(newStep);

        return GoAheadTillOuboundOrLoop(nextCoordinate!, nextDirection);
    }

    public void PlaceObstacleAt(Coordinate coordinate)
    {
        source[coordinate.Y, coordinate.X] = '#';
    }

    char GetContent(Coordinate position)
    {
        return source[position.Y, position.X];
    }

    bool IsOutOfBounds(Coordinate position)
    {
        return position.X < 0 ||
            position.X >= source.GetLength(0) ||
            position.Y < 0 ||
            position.Y >= source.GetLength(1);
    }

    public int CountVisitedPositions()
    {
        var count = 0;

        for (var rowIndex = 0; rowIndex < source.GetLength(0); rowIndex++)
        {
            var row = Utils.GetRow(source, rowIndex);
            for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                var charValue = row[columnIndex];
                if (charValue == 'X')
                {
                    count++;
                }
            }
        }

        return count;
    }

    public void Print()
    {
        for (var rowIndex = 0; rowIndex < source.GetLength(0); rowIndex++)
        {
            var row = Utils.GetRow(source, rowIndex);
            var rowString = new string(row);
            Console.WriteLine(rowString);
        }
    }

    public Map CloneInitialState()
    {
        return new Map((char[,])source.Clone(), initialPosition);
    }
}

class Step(Coordinate start, Coordinate end)
{
    public Coordinate Start = start;
    public Coordinate End = end;

    public override bool Equals(object? other)
    {
        return other != null &&
            other is Step otherStep &&
            GetHashCode() == otherStep.GetHashCode();
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End);
    }

    public override string ToString() => $"({Start.X}, {Start.Y}) -> ({End.X}, {End.Y})";
}
