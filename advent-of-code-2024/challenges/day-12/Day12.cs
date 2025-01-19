namespace advent_of_code_2024.challenges;

internal class Day12
{
    private static IList<Garden> ReadGardens()
    {
        var lines = Utils.ReadLinesFromChallengeSource(12);
        IList<Garden> gardens = [];

        for (var y = 0; y < lines.Count; y++)
        {
            var line = lines[y];
            for (var x = 0; x < line.Length; x++)
            {
                var plant = new Plant(new Coordinate(x, y));
                var symbol = line[x].ToString();
                var handled = false;
                foreach (var garden in gardens.Where(g => g.Symbol == symbol))
                {
                    if (garden.TryAddPlant(plant, symbol, lines))
                    {
                        handled = true;
                        break;
                    }
                }
                if (!handled)
                {
                    gardens.Add(Garden.StartGardenWith(plant, symbol));
                }
            }
        }

        return gardens;
    }

    public static long ResolvePartOne()
    {
        var gardens = ReadGardens();
        return gardens.Sum(g => g.CalculatePrice());
    }

    public static long ResolvePartTwo()
    {
        return 0;
    }

    class Garden(string symbol)
    {
        public List<Plant> Plants = [];
        public string Symbol = symbol;

        public static Garden StartGardenWith(Plant plant, string symbol)
        {
            var garden = new Garden(symbol);
            garden.AddPlant(plant);
            return garden;
        }

        public bool HasPlant(Plant plant)
        {
            return Plants.Any(p => p.Coordinate.Equals(plant.Coordinate));
        }

        public void AddPlant(Plant plant)
        {
            Plants.Add(plant);
        }

        public bool HasDirectNeighborHere(Plant plant, string symbol)
        {
            if (symbol != Symbol)
            {
                return false;
            }

            var possibleNeighbors = plant.CalculateNeighbors();

            return Plants.Any(p => possibleNeighbors.Any(n => p.Coordinate.Equals(n)));
        }

        public bool TryAddPlant(Plant plant, string symbol, IList<string> map)
        {
            if (CheckNeighborsTillDifferentSymbol(map, plant, symbol, []))
            {
                AddPlant(plant);
                return true;
            }

            return false;
        }

        private bool CheckNeighborsTillDifferentSymbol(IList<string> map, Plant plant, string symbol, List<Coordinate> alreadyVisited)
        {
            var neighborsWithSameSymbol = plant.CalculateNeighbors()
                .Where(p => !alreadyVisited.Contains(p))
                .Where(p => p.Y < map.Count
                    && p.Y >= 0)
                .Select(p => (position: p, line: map[p.Y]))
                .Where(pair => pair.position.X < pair.line.Length
                    && pair.position.X >= 0
                    && pair.line[pair.position.X].ToString() == symbol)
                .Select(pair => new Plant(pair.position))
                .ToList();

            if (neighborsWithSameSymbol.Count == 0)
            {
                return false;
            }

            if (neighborsWithSameSymbol.Any(plantWithSameSymbol => HasPlant(plantWithSameSymbol)))
            {
                return true;
            }

            alreadyVisited.AddRange(neighborsWithSameSymbol.Select(p => p.Coordinate));
            return neighborsWithSameSymbol.Any(n => CheckNeighborsTillDifferentSymbol(map, n, symbol, alreadyVisited));
        }

        public long CalculatePrice()
        {
            long CalculateFences(Plant plant)
            {
                return plant.CalculateNeighbors()
                    .Count(position => !Plants.Any(plant => plant.Coordinate.Equals(position)));
            }

            return Plants.Count * Plants.Sum(CalculateFences);
        }

        public override string ToString()
        {
            return $"{Symbol}-{Plants.Count}";
        }
    }

    class Plant(Coordinate coordinate)
    {
        public Coordinate Coordinate = coordinate;

        public bool IsNeighborOf(Plant otherPlant)
        {
            return CalculateNeighbors().Any(n => n.Equals(otherPlant.Coordinate));
        }

        public IList<Coordinate> CalculateNeighbors()
        {
            return [
                new Coordinate(Coordinate.X - 1, Coordinate.Y),
                new Coordinate(Coordinate.X, Coordinate.Y - 1),
                new Coordinate(Coordinate.X + 1, Coordinate.Y),
                new Coordinate (Coordinate.X, Coordinate.Y +1)
            ];
        }
    }
}
