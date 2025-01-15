using advent_of_code_2024.common;

namespace advent_of_code_2024.challenges;

internal class Day20
{
    static RaceTrack ReadRaceTrack()
    {
        var lines = Utils.ReadLinesFromChallengeSource(20);

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

        return new RaceTrack(walls, lines.Count, start!, end!);
    }

    public static long ResolvePartOne()
    {
        return ReadRaceTrack().CalculateHowManyCheatsSaveAtLeast100ps();
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }
}

class RaceTrack : Maze
{
    public RaceTrack(IList<Coordinate> walls, int size, Coordinate start, Coordinate end)
        : base(walls, size, start, end) { }

    public long CalculateHowManyCheatsSaveAtLeast100ps()
    {
        var originalTime = CalulateTimeExcludingWallIn(null);
        var count = 0;

        foreach (var wallPosition in Walls)
        {
            if (wallPosition.X == 0 || wallPosition.Y == 0 || wallPosition.X == Size - 1 || wallPosition.Y == Size - 1)
            {
                continue;
            }

            Console.WriteLine($"Testing removing wall in {wallPosition}");
            var newTime = CalulateTimeExcludingWallIn(wallPosition);
            if (originalTime - newTime >= 100)
            {
                count++;
            }
        }

        return count;
    }

    private long CalulateTimeExcludingWallIn(Coordinate? position)
    {
        var walls = Walls;
        if (position != null)
        {
            walls = walls.Except([position]).ToList();
        }

        var startingRunner = new BasicRunner(Start, End, 0, walls, Size);

        var runners = RunTillFindExit(startingRunner, walls);

        var shortestPath = runners.FirstOrDefault(r => r.FoundTarget);

        return shortestPath == null ? long.MaxValue : shortestPath!.VisitedPoints;
    }
}