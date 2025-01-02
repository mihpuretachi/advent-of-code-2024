namespace advent_of_code_2024.challenges;

internal class Day1
{
    public static (List<int>, List<int>) ReadLists()
    {
        var source = Utils.ReadLinesFromChallengeSource(1);

        var leftList = new List<int>();
        var rightList = new List<int>();

        foreach (var line in source)
        {
            var numbers = line.Split("   ");
            leftList.Add(int.Parse(numbers[0]));
            rightList.Add(int.Parse(numbers[1]));
        }

        return (leftList, rightList);
    }

    public static double ResolvePartOne()
    {
        var (leftList, rightList) = ReadLists();

        leftList.Sort();
        rightList.Sort();

        var result = 0;
        for (var i = 0; i < leftList.ToArray().Length; i++)
        {
            var leftNumber = leftList[i];
            var rightNumber = rightList[i];

            result += Math.Abs(leftNumber - rightNumber);
        }

        return result;
    }

    public static double ResolvePartTwo()
    {
        var (leftList, rightList) = ReadLists();

        var map = new Dictionary<int, (int, int)>();

        for (var i = 0; i < leftList.ToArray().Length; i++)
        {
            var leftNumber = leftList[i];

            if (!map.ContainsKey(leftNumber))
            {
                map.Add(leftNumber, (1, 0));
            }
            else
            {
                var (countLeft, countRight) = map[leftNumber];
                map[leftNumber] = (countLeft + 1, countRight);
            }


            var rightNumber = rightList[i];
            if (!map.ContainsKey(rightNumber))
            {
                map.Add(rightNumber, (0, 1));
            }
            else
            {
                var (countLeft, countRight) = map[rightNumber];
                map[rightNumber] = (countLeft, countRight + 1);
            }
        }

        return map.Select((item) => item.Key * item.Value.Item1 * item.Value.Item2).Sum();

    }
}
