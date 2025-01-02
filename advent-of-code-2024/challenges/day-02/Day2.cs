namespace advent_of_code_2024.challenges;

internal class Day2
{
    public static List<List<int>> ReadReports()
    {
        var source = Utils.ReadLinesFromChallengeSource(2);

        var reports = new List<List<int>>();

        foreach (var line in source)
        {
            var report = line
                .Split(" ")
                .Select(int.Parse)
                .ToList();
            reports.Add(report);
        }

        return reports;
    }

    private static bool ValidateSafeness(List<int> report)
    {
        var sortedListAsc = report.OrderBy(x => x).ToList();
        if (!sortedListAsc.SequenceEqual(report))
        {
            var sortedListDesc = report.OrderByDescending(x => x).ToList();
            if (!sortedListDesc.SequenceEqual(report))
            {
                return false;
            }
        }

        for (var i = 1; i < report.Count; i++)
        {
            var previousNumber = report[i - 1];
            var currentNumber = report[i];

            var differenceBetweenNumbers = Math.Abs(previousNumber - currentNumber);

            var failed = differenceBetweenNumbers < 1 || differenceBetweenNumbers > 3;

            if (failed)
            {
                return false;
            }
        }

        return true;
    }

    private static List<List<int>> FindVariants(List<int> report)
    {
        var variants = new List<List<int>>
        {
            report
        };

        for (var i = 0; i < report.Count; i++)
        {
            var copy = report.ToList();
            copy.RemoveAt(i);
            variants.Add(copy);
        }

        return variants;
    }

    public static double ResolvePartOne()
    {
        return ReadReports().Where(r => ValidateSafeness(r)).Count();
    }

    public static double ResolvePartTwo()
    {
        return ReadReports().Where(r => FindVariants(r).Any(v => ValidateSafeness(v))).Count();
    }
}
