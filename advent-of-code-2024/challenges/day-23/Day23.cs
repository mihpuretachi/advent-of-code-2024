using System.Linq;

namespace advent_of_code_2024.challenges;

internal class Day23
{
    static IDictionary<string, IList<string>> ReadConnectedComputers()
    {
        var lines = Utils.ReadLinesFromChallengeSource(23);

        var map = new Dictionary<string, IList<string>>();

        foreach (var line in lines)
        {
            var values = line.Split('-');
            if (!map.ContainsKey(values[0]))
            {
                map.Add(values[0], new List<string>());
            }
            map[values[0]].Add(values[1]);

            if (!map.ContainsKey(values[1]))
            {
                map.Add(values[1], new List<string>());
            }
            map[values[1]].Add(values[0]);
        }

        return map;
    }

    public static long ResolvePartOne()
    {
        var map = ReadConnectedComputers();
        IList<string> linkedComputers = [];

        foreach (var (currentComputer, otherComputers) in map)
        {
            if (otherComputers.Count < 2)
            {
                continue;
            }

            foreach (var otherComputer in otherComputers)
            {
                foreach (var thirdComputer in map[otherComputer])
                {
                    if (map[thirdComputer].Contains(currentComputer))
                    {
                        var trio = new List<string> { currentComputer, otherComputer, thirdComputer };
                        if (trio.Any(c => c.StartsWith("t")))
                        {
                            trio.Sort();
                            linkedComputers = linkedComputers.Append(string.Join(",", trio)).ToList();
                        }
                    }
                }
            }
        }

        return linkedComputers.Distinct().Count();
    }

    public static string ResolvePartTwo()
    {
        var map = ReadConnectedComputers();
        IList<IList<string>> biggestParties = [];

        foreach (var (currentComputer, otherComputers) in map.Where(pair => pair.Key.StartsWith("t")))
        {
            biggestParties.Add(FindBiggestParty(map, [currentComputer]));
        }

        var biggestPartyWithLeader = biggestParties
           .Where(party => party.Any(c => c.StartsWith("t")))
           .Select(party => (party.Count, Party: party))
           .ToList()
           .OrderBy((v) => v.Count)
           .Last()
           .Party;

        return string.Join(",", biggestPartyWithLeader.Order());
    }

    static IList<string> FindBiggestParty(IDictionary<string, IList<string>> map, IList<string> currentParty)
    {
        var currentComputer = currentParty.Last();
        var otherComputers = map[currentComputer];

        IList<IList<string>> parties = [];

        foreach (var otherComputer in otherComputers)
        {
            var isInThePartyAlready = currentParty.Contains(otherComputer);
            var possibleNewPartyFormation = currentParty.Concat([otherComputer]).ToList();
            var willTheyAllBeConnected = possibleNewPartyFormation
                .Select(c => (computer: c, linkedComputers: map[c]))
                .All(pair => possibleNewPartyFormation.All(c => pair.computer == c || pair.linkedComputers.Contains(c)));

            if (!isInThePartyAlready && willTheyAllBeConnected)
            {
                parties.Add(FindBiggestParty(map, possibleNewPartyFormation));
            }
        }

        if (parties.Count == 0)
        {
            // Could not find a new computer to be added
            return currentParty;
        }

        return parties
            .Select(party => (party.Count, Party: party))
            .ToList()
            .OrderBy((v) => v.Count)
            .Last()
            .Party;
    }
}

