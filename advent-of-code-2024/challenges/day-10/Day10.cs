namespace advent_of_code_2024.challenges;

internal class Day10
{
    static int[,] ReadMap()
    {
        var source = Utils.ReadLinesFromChallengeSource(10);

        var map = new int[source.Count, source.Count];

        for (var rowIndex = 0; rowIndex < source.Count; rowIndex++)
        {
            var row = source[rowIndex];
            for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                var value = row[columnIndex];
                map.SetValue(int.Parse(value.ToString()), rowIndex, columnIndex);
            }
        }
        return map;
    }

    public static double ResolvePartOne()
    {
        return new TopographicMap(ReadMap()).FindTrailHeads().Sum(h => h.Score);
    }

    public static double ResolvePartTwo()
    {
        return new TopographicMap(ReadMap()).FindTrailHeads().Sum(h => h.Rating);
    }
}

class TopographicMap(int[,] source)
{
    public IList<TrailHead> FindTrailHeads()
    {
        var list = new List<TrailHead>();

        for (var y = 0; y < source.GetLength(0); y++)
        {
            for (var x = 0; x < source.GetLength(1); x++)
            {
                var height = GetHeight((x, y));
                if (height == 0)
                {
                    var (score, rating) = CalculateStats((x, y));
                    list.Add(new TrailHead(score, rating));
                }
            }
        }

        return list;
    }

    (int, int) CalculateStats((int, int) position)
    {
        var score = 0;
        var trails = new List<List<(int, int)>>();

        void Hike(int targetHeight, (int, int) position, IList<(int, int)> previousPositions)
        {
            var left = (position.Item1 - 1, position.Item2);
            var right = (position.Item1 + 1, position.Item2);
            var up = (position.Item1, position.Item2 - 1);
            var down = (position.Item1, position.Item2 + 1);

            IList<(int, int)> nextSteps = [left, right, up, down];

            foreach (var nextStep in nextSteps)
            {
                if (previousPositions.LastOrDefault() == nextStep)
                {
                    continue;
                }

                if (IsOutOfBounds(nextStep))
                {
                    continue;
                }

                var height = GetHeight(nextStep);
                if (targetHeight != height)
                {
                    continue;
                }


                var trail = previousPositions.Concat([nextStep]).ToList();

                if (height == 9)
                {
                    trails.Add(trail);
                    score++;
                    continue;
                }

                if (targetHeight < 9)
                {
                    Hike(targetHeight + 1, nextStep, trail);
                }
            }

        }

        Hike(1, position, []);

        return (trails.Select(t => t.Last()).Distinct().Count(), trails.Count);
    }

    int GetHeight((int, int) position)
    {
        return (int)source.GetValue(position.Item2, position.Item1)!;
    }

    bool IsOutOfBounds((int, int) position)
    {
        return position.Item1 < 0 ||
            position.Item2 < 0 ||
            position.Item1 >= source.GetLength(1) ||
            position.Item2 >= source.GetLength(0);
    }
}

class TrailHead(int score, int rating)
{
    public int Score = score;
    public int Rating = rating;
}
