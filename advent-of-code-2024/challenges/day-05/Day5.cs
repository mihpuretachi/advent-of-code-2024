namespace advent_of_code_2024.challenges;

internal class Day5
{

    static (Dictionary<int, int[]>, IList<IList<int>>) ReadPrintOrder()
    {
        var source = Utils.ReadLinesFromChallengeSource(5);

        var rules = new Dictionary<int, int[]>();
        var updates = new List<IList<int>>();
        var startedUpdates = false;

        for (var rowIndex = 0; rowIndex < source.Count; rowIndex++)
        {
            var row = source[rowIndex];

            if (startedUpdates)
            {
                updates.Add(row.Split(',').Select(s => int.Parse(s)).ToArray());
            }
            else if (string.IsNullOrWhiteSpace(row))
            {
                startedUpdates = true;
            }
            else
            {
                var values = row.Split("|").Select(s => int.Parse(s)).ToList();
                if (rules.ContainsKey(values[0]))
                {
                    rules[values[0]] = rules[values[0]].Append(values[1]).ToArray();
                }
                else
                {
                    rules.Add(values[0], [values[1]]);
                }
            }
        }

        return (rules, updates);
    }

    public static double ResolvePartOne()
    {
        var (rules, updates) = ReadPrintOrder();

        return updates
            .Select(u => ProcessUpdate(u, rules))
            .Where(r => r.Item1)
            .Sum(r => r.Item2);
    }

    public static double ResolvePartTwo()
    {
        var (rules, updates) = ReadPrintOrder();

        return updates
            .Select(u => ProcessUpdate(u, rules))
            .Where(r => !r.Item1)
            .Sum(r => r.Item2);
    }

    static (bool, int) ProcessUpdate(IList<int> update, Dictionary<int, int[]> rules)
    {
        var passRules = true;
        var middleNumber = 0;
        var numbersBefore = new Dictionary<int, int>();

        for (var index = 0; index < update.Count; index++)
        {
            var currentNumber = update[index];

            if (rules.TryGetValue(currentNumber, out int[]? numbersMandatoryToComeAfter))
            {
                var wrongNumbersMap = numbersBefore.Where(numberBefore => numbersMandatoryToComeAfter.Any(numberThatMustComeAfter => numberThatMustComeAfter == numberBefore.Value));
                if (wrongNumbersMap.Count() > 0)
                {
                    passRules = false;
                    var indexToInsertNumber = wrongNumbersMap.Select(item => item.Key).Min();

                    var copy = update.ToList();
                    copy.Remove(currentNumber);
                    copy.Insert(indexToInsertNumber, currentNumber);

                    update = copy;

                    numbersBefore = numbersBefore.Where(n => n.Key < indexToInsertNumber).ToDictionary();

                    index = indexToInsertNumber - 1; // Restart from where value was inserted
                    continue;
                }
            }

            numbersBefore[index] = currentNumber;
        }


        middleNumber = update[update.Count / 2];

        return (passRules, middleNumber);
    }
}
