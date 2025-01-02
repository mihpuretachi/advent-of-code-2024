namespace advent_of_code_2024.challenges;

internal class Day7
{
    static IList<Equation> ReadEquations()
    {
        var source = Utils.ReadLinesFromChallengeSource(7);
        var equations = new List<Equation>();

        foreach (var line in source)
        {
            var split = line.Split(':');
            var result = long.Parse(split[0]);
            var values = split[1].Trim().Split(" ").Select(long.Parse).ToArray();
            equations.Add(new Equation(result, values));
        }

        return equations;
    }

    public static double ResolvePartOne()
    {
        return ReadEquations().Where(e => e.IsValid(["+", "*"])).Select(e => e.Result).Sum();
    }

    public static double ResolvePartTwo()
    {
        return ReadEquations().Where(e => e.IsValid(["+", "*", "|"])).Select(e => e.Result).Sum();
    }
}


class Equation(long result, long[] values)
{
    public long Result = result;
    public long[] Values = values;

    public bool IsValid(string[] validOperators)
    {
        var combinations = Utils.CombinationsWithRepetition(validOperators, Values.Length - 1);
        foreach (var combination in combinations)
        {
            var combinationResult = Run(combination);
            if (combinationResult == Result)
            {
                return true;
            }
        }
        return false;
    }

    private long Run(string[] operators)
    {
        var result = Values[0];
        var index = 1;

        foreach (var operatorValue in operators)
        {
            if (operatorValue == "+")
            {
                result += Values[index];
            }
            else if (operatorValue == "*")
            {
                result *= Values[index];
            }
            else if (operatorValue == "|")
            {
                var a = result.ToString();
                var b = Values[index].ToString();
                result = long.Parse(a + b);
            }
            else
            {
                throw new ArgumentException($"Invalid Operator: {operatorValue}");
            }

            index++;
        }

        return result;
    }
}
