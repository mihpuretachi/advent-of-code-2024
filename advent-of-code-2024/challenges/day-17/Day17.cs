namespace advent_of_code_2024.challenges;

internal class Day17
{
    static ThreeBitComputer CreateComputer()
    {
        long a = 0;
        long b = 0;
        long c = 0;
        IList<Instruction> instructions = [];

        var lines = Utils.ReadLinesFromChallengeSource(17);

        foreach (var line in lines)
        {
            if (line.Contains("A"))
            {
                a = long.Parse(line.Split(": ")[1]);
            }
            else if (line.Contains("B"))
            {
                b = long.Parse(line.Split(": ")[1]);
            }
            else if (line.Contains("C"))
            {
                c = long.Parse(line.Split(": ")[1]);
            }
            else if (line.Contains("Program"))
            {
                var values = line.Split(": ")[1].Split(",");
                for (var index = 0; index < values.Length; index += 2)
                {
                    instructions.Add(new Instruction(long.Parse(values[index]), long.Parse(values[index + 1])));
                }
            }
        }

        return new ThreeBitComputer(a, b, c, instructions);
    }

    public static string ResolvePartOne()
    {
        var computer = CreateComputer();
        computer.Run();
        return string.Join(",", computer.Output.Select(v => v.ToString()));
    }

    public static long ResolvePartTwo()
    {
        var originalComputer = CreateComputer();
        var originalProgram = string.Join(",", originalComputer.Program.Select(i => string.Join(",", new List<string> { i.Opcode.ToString(), i.Operand.ToString() })));
        long possibleRegisterA = 20000046204963;
        var currentOutput = "";

        while (currentOutput != originalProgram)
        {
            possibleRegisterA += 8;
            Console.WriteLine($"Trying with value: {possibleRegisterA} / In octal: {DecToOctal(possibleRegisterA)}");
            var computer = CreateComputer();
            computer.RegisterA = possibleRegisterA;
            computer.Run();
            currentOutput = string.Join(",", computer.Output);
            Console.WriteLine($"Current Output: {currentOutput}");
        }

        return possibleRegisterA;
    }

    static long DecToOctal(long n)
    {
        long[] octalNum = new long[100];
        int i = 0;
        while (n != 0)
        {
            octalNum[i] = n % 8;
            n /= 8;
            i++;
        }

        string result = "";
        for (int j = i - 1; j >= 0; j--)
            result += octalNum[j];

        return long.Parse(result);
    }

    class ThreeBitComputer(long a, long b, long c, IList<Instruction> program)
    {
        public long RegisterA = a;
        long RegisterB = b;
        long RegisterC = c;
        public IList<Instruction> Program = program;

        int index = 0;
        public IList<long> Output = [];

        public void Run()
        {
            while (index < Program.Count)
            {
                var avoidIncreaseIndex = false;

                var command = Program[index];
                switch (command.Opcode)
                {
                    case 0:
                        // adv
                        RegisterA = (long)Math.Floor(RegisterA / Math.Pow(2, TranslateComboOperand(command.Operand)));
                        break;
                    case 1:
                        // bxl
                        RegisterB ^= command.Operand;
                        break;
                    case 2:
                        // bst
                        RegisterB = TranslateComboOperand(command.Operand) % 8;
                        break;
                    case 3:
                        // jnz
                        if (RegisterA == 0)
                        {
                            break;
                        }
                        index = (int)command.Operand / 2;
                        avoidIncreaseIndex = true;
                        break;
                    case 4:
                        // bxc
                        RegisterB ^= RegisterC;
                        break;
                    case 5:
                        // out
                        var result = TranslateComboOperand(command.Operand) % 8;
                        Output = Output
                            .Concat(result.ToString().Split().Select(v => long.Parse(v)))
                            .ToList();
                        break;
                    case 6:
                        // bdv
                        RegisterB = (long)Math.Floor(RegisterA / Math.Pow(2, TranslateComboOperand(command.Operand)));
                        break;
                    case 7:
                        // cdv
                        RegisterC = (long)Math.Floor(RegisterA / Math.Pow(2, TranslateComboOperand(command.Operand)));
                        break;
                    default: throw new Exception($"Invalid command: {command.Opcode}");
                }

                if (avoidIncreaseIndex)
                {
                    continue;
                }

                index++;
            }
        }

        private long TranslateComboOperand(long operand)
        {
            switch (operand)
            {
                case 0: return 0;
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
                case 4: return RegisterA;
                case 5: return RegisterB;
                case 6: return RegisterC;
                case 7: throw new Exception("Invalid program. Operand 7 received.");
                default: throw new Exception($"Invalid operand: {operand}");
            }


        }
    }

    record Instruction(long Opcode, long Operand) { }
}