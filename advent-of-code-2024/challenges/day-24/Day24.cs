namespace advent_of_code_2024.challenges;

internal class Day24
{
    private static (IDictionary<string, int>, IList<Gate>) ReadGatesSetup()
    {
        var lines = Utils.ReadLinesFromChallengeSource(24);

        var wires = new Dictionary<string, int>();
        IList<Gate> gates = [];
        bool startedReadingGates = false;

        foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                startedReadingGates = true;
                continue;
            }

            if (startedReadingGates)
            {
                var values = line.Split(' ');
                var operation = values[1] == "AND"
                    ? GateOperation.AND
                    : values[1] == "OR"
                        ? GateOperation.OR
                        : GateOperation.XOR;
                gates.Add(new Gate(values[0], values[2], operation, values[4]));
            }
            else
            {
                var values = line.Split(": ");
                wires.Add(values[0], int.Parse(values[1]));
            }
        }

        return (wires, gates);
    }

    public static long ResolvePartOne()
    {
        var (wires, gates) = ReadGatesSetup();

        var gatesMap = gates.ToDictionary(g => g, g => false);

        while (gatesMap.Any(g => !g.Value))
        {
            foreach (var gateEntry in gatesMap.Where(g => !g.Value))
            {
                var gate = gateEntry.Key;

                if (wires.TryGetValue(gate.LabelA, out var valueA)
                    && wires.TryGetValue(gate.LabelB, out var valueB))
                {
                    var result = gate.Operation == GateOperation.AND
                        ? valueA == 1 && valueB == 1
                        : gate.Operation == GateOperation.OR
                            ? valueA == 1 || valueB == 1
                            : valueA != valueB;

                    wires.Add(gate.ResultLabel, result ? 1 : 0);
                    gatesMap[gate] = true;
                }
            }
        }

        var relevantWires = wires
            .Where(w => w.Key.StartsWith("z"))
            .OrderByDescending(w => w.Key)
            .Select(w => w.Value)
            .ToList();

        return Convert.ToInt64(string.Join("", relevantWires), 2);
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }

    record Wire(string Label, int Value) { }

    class Gate(string labelA, string labelB, GateOperation operation, string resultLabel)
    {
        public string LabelA = labelA;
        public string LabelB = labelB;
        public GateOperation Operation = operation;
        public string ResultLabel = resultLabel;
    }

    enum GateOperation
    {
        OR,
        AND,
        XOR
    }
}