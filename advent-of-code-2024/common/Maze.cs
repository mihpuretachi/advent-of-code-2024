using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using advent_of_code_2024.challenges;

namespace advent_of_code_2024.common;

abstract class Maze (IList<Coordinate> walls, int size, Coordinate start, Coordinate end)
{
    protected IList<Coordinate> Walls = walls;
    protected Coordinate Start = start;
    protected Coordinate End = end;

    public int Size = size;
    
    protected IList<Runner> RunTillFindExit(Runner startingRunner, IList<Coordinate> walls)
    {
        IList<Runner> runners = [startingRunner];
        IList<Coordinate> allVisitedPoints = [new Coordinate(0, 0)];

        while (!runners.Any(r => r.FoundTarget) && runners.Any(r => r.CanWalk))
        {
            IList<Runner> runnersToBeDeleted = [];
            IList<Runner> runnersToBeAdded = [];

            foreach (var currentRunner in runners)
            {
                var newRunners = currentRunner.Split(allVisitedPoints);

                if (!currentRunner.FoundTarget)
                {
                    runnersToBeDeleted.Add(currentRunner);
                }

                foreach (var newRunner in newRunners)
                {
                    if (allVisitedPoints.Contains(newRunner.CurrentPosition))
                    {
                        continue;
                    }

                    // Console.WriteLine($"Passing through point {newRunner.CurrentPosition}");

                    allVisitedPoints.Add(newRunner.CurrentPosition);

                    if (newRunner.FoundDeadEndAt != null)
                    {
                        continue;
                    }

                    runnersToBeAdded.Add(newRunner);
                }
            }

            foreach (var runnerToBeDeleted in runnersToBeDeleted)
            {
                runners.Remove(runnerToBeDeleted);
            }

            foreach (var runnerToBeAdded in runnersToBeAdded)
            {
                runners.Add(runnerToBeAdded);
            }
        }

        return runners;
    }
}
