namespace advent_of_code_2024;

public class Coordinate(int x, int y)
{
    public int X = x;
    public int Y = y;

    public override bool Equals(object? other)
    {
        return other != null &&
            other is Coordinate otherCoordinate &&
            X == otherCoordinate.X &&
            Y == otherCoordinate.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString() => $"({X},{Y})";
}