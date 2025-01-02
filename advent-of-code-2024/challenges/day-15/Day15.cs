namespace advent_of_code_2024.challenges
{
    internal class Day15
    {
        private static Warehouse ReadWarehouseMap()
        {
            var lines = Utils.ReadLinesFromChallengeSource(15);

            IList<WarehouseWall> walls = [];
            IList<WarehouseBox> boxes = [];
            Coordinate? robotPosition = null;
            IList<Direction> directions = [];
            int height = lines.Count;
            int width = 0;

            for (int y = 0; y < lines.Count; y++)
            {
                var line = lines[y];

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                for (var x = 0; x < line.Length; x++)
                {
                    var content = line[x];
                    width = x;

                    switch (content)
                    {
                        case '#':
                            walls.Add(new WarehouseWall(new Coordinate(x, y)));
                            break;
                        case '@':
                            robotPosition = new Coordinate(x, y);
                            break;
                        case '.':
                            break;
                        case 'O':
                            boxes.Add(new WarehouseBox(new Coordinate(x, y)));
                            break;
                        case '^':
                            directions.Add(Direction.Up);
                            break;
                        case 'v':
                            directions.Add(Direction.Down);
                            break;
                        case '<':
                            directions.Add(Direction.Left);
                            break;
                        case '>':
                            directions.Add(Direction.Right);
                            break;
                        default:
                            throw new Exception($"Invalid content in map: {content}");
                    }
                }
            }

            return new Warehouse(boxes, walls, new WarehouseRobot(robotPosition!, directions), height, width);
        }

        private static Warehouse ReadWiderWarehouseMap()
        {
            var lines = Utils.ReadLinesFromChallengeSource(15);

            IList<WarehouseWall> walls = [];
            IList<WarehouseBox> boxes = [];
            Coordinate? robotPosition = null;
            IList<Direction> directions = [];
            int height = lines.Count;
            int width = 0;

            for (int y = 0; y < lines.Count; y++)
            {
                var line = lines[y];

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                for (var x = 0; x < line.Length; x++)
                {
                    var content = line[x];
                    width = x;

                    switch (content)
                    {
                        case '#':
                            walls.Add(new WarehouseWall(new Coordinate(x * 2, y), 2));
                            break;
                        case '@':
                            robotPosition = new Coordinate(x * 2, y);
                            break;
                        case '.':
                            break;
                        case 'O':
                            boxes.Add(new WarehouseBox(new Coordinate(x * 2, y), 2));
                            break;
                        case '^':
                            directions.Add(Direction.Up);
                            break;
                        case 'v':
                            directions.Add(Direction.Down);
                            break;
                        case '<':
                            directions.Add(Direction.Left);
                            break;
                        case '>':
                            directions.Add(Direction.Right);
                            break;
                        default:
                            throw new Exception($"Invalid content in map: {content}");
                    }
                }
            }

            return new Warehouse(boxes, walls, new WarehouseRobot(robotPosition!, directions), height, width);
        }

        public static long ResolvePartOne()
        {
            var warehouse = ReadWarehouseMap();
            warehouse.TurnOnRobot();
            return warehouse.CalculateGPSSum();
        }

        public static long ResolvePartTwo()
        {
            var warehouse = ReadWiderWarehouseMap();
            warehouse.TurnOnRobot();
            return warehouse.CalculateGPSSum();
        }
    }

    internal class Warehouse(IList<WarehouseBox> boxes, IList<WarehouseWall> walls, WarehouseRobot robot, int height, int width)
    {
        int Height = height;
        int Width = width;
        IList<WarehouseBox> Boxes = boxes;
        IList<WarehouseWall> Walls = walls;
        WarehouseRobot Robot = robot;

        public void TurnOnRobot()
        {
            //Console.WriteLine("Initial state");
            //Console.WriteLine();
            //Print();

            foreach (var command in Robot.Commands)
            {
                var robotAsItem = Robot as WarehouseItem;
                var itemsToMove = DoTryToMove(robotAsItem, command, []);
                foreach (var item in itemsToMove)
                {
                    if (command == Direction.Up)
                    {
                        item.Position.Y--;
                    }
                    else if (command == Direction.Down)
                    {
                        item.Position.Y++;
                    }
                    else if ((command == Direction.Left))
                    {
                        item.Position.X--;
                    }
                    else
                    {
                        item.Position.X++;
                    }
                }

                //Console.WriteLine($"After command: {command}");
                //Console.WriteLine();
                //Print();
            }
        }

        private IList<WarehouseItem> DoTryToMove(WarehouseItem item, Direction direction, IList<WarehouseItem> boxesAlreadyMoved)
        {
            var nextPossibleCoordinate = CalculateNextPosition(item.Position, direction);

            if (Walls.Any(wall => wall.IntersectsWith(nextPossibleCoordinate, item.Size)))
            {
                return []; // Found wall, no item can be moved
            }

            var boxes = Boxes.Where(box => box.IntersectsWith(nextPossibleCoordinate, item.Size) && !boxesAlreadyMoved.Contains(box)).ToList();
            if (boxes.Count > 0)
            {
                IList<WarehouseItem> itemsToBeMoved = [];
                foreach (var box in boxes)
                {
                    boxesAlreadyMoved.Add(box);
                    var result = DoTryToMove(box, direction, boxesAlreadyMoved);
                    if (result.Count == 0)
                    {
                        return []; // Some item can't be moved
                    }
                }
                return boxesAlreadyMoved.Append(Robot).ToList();
            }

            return [Robot]; // No item besides robot to be moved
        }

        private Coordinate CalculateNextPosition(Coordinate currentPosition, Direction direction)
        {
            return direction == Direction.Up
               ? new Coordinate(currentPosition.X, currentPosition.Y - 1)
               : direction == Direction.Down
               ? new Coordinate(currentPosition.X, currentPosition.Y + 1)
               : direction == Direction.Left
               ? new Coordinate(currentPosition.X - 1, currentPosition.Y)
               : new Coordinate(currentPosition.X + 1, currentPosition.Y);
        }

        public long CalculateGPSSum()
        {
            return Boxes.Sum(box => box.Position.Y * 100 + box.Position.X);
        }

        private void Print()
        {
            for (var y = 0; y < Height; y++)
            {
                var line = "";
                for (var x = 0; x < Width; x++)
                {
                    var box = Boxes.FirstOrDefault(b => b.Position.X == x && b.Position.Y == y);
                    if (box != null)
                    {
                        if (box.Size == 1)
                        {
                            line += "O";
                        }
                        else
                        {
                            line += "[]";
                            x++;
                        }
                        continue;
                    }

                    var wall = Walls.FirstOrDefault(w => w.Position.X == x && w.Position.Y == y);
                    if (wall != null)
                    {
                        if (wall.Size == 1)
                        {
                            line += "#";
                        }
                        else
                        {
                            line += "##";
                            x++;
                        }
                        continue;
                    }

                    if (Robot.Position.X == x && Robot.Position.Y == y)
                    {
                        line += "@";
                        continue;
                    }

                    line += ".";
                }
                Console.WriteLine(line);
            }
        }
    }

    class WarehouseRobot : WarehouseItem
    {
        public IList<Direction> Commands { get; }

        public WarehouseRobot(Coordinate initialPosition, IList<Direction> commands) : base(initialPosition)
        {
            Commands = commands;
        }
    }

    class WarehouseBox : WarehouseItem
    {
        public WarehouseBox(Coordinate position, int size = 1) : base(position, size) { }
    }

    class WarehouseWall : WarehouseItem
    {
        public WarehouseWall(Coordinate position, int size = 1) : base(position, size) { }
    }

    class WarehouseItem
    {
        public Coordinate Position { get; set; }
        public int Size { get; private set; }

        public WarehouseItem(Coordinate position, int size = 1)
        {
            Position = position;
            Size = size;
        }

        public bool ExistsIn(Coordinate position)
        {
            for (var i = 0; i < Size; i++)
            {
                if (Position.X + i == position.X && Position.Y == position.Y)
                {
                    return true;
                }
            }
            return false;
        }


        public bool IntersectsWith(Coordinate anotherItemCoordinate, int anotherItemSize)
        {
            IList<Coordinate> positions = [];
            IList<Coordinate> otherPositions = [];

            for (var i = 0; i < Size; i++)
            {
                positions.Add(new Coordinate(Position.X + i, Position.Y));
            }

            for (var i = 0; i < anotherItemSize; i++)
            {
                otherPositions.Add(new Coordinate(anotherItemCoordinate.X + i, anotherItemCoordinate.Y));
            }

            return positions.Intersect(otherPositions).ToList().Count > 0;
        }
    }
}
