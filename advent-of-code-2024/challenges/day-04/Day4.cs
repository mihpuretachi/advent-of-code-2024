using System.Data;

namespace advent_of_code_2024.challenges;

internal class Day4
{
    static char[,] ReadMatrix()
    {
        var matrix = new char[140, 140];
        var source = Utils.ReadLinesFromChallengeSource(4);

        for (var rowIndex = 0; rowIndex < source.Count; rowIndex++)
        {
            var row = source[rowIndex];
            for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
            {
                var charValue = row[columnIndex];
                matrix.SetValue(charValue, rowIndex, columnIndex);
            }
        }

        return matrix;
    }

    public static double ResolvePartOne()
    {
        var target = "XMAS";
        var matrix = ReadMatrix();
        var ocurrences = 0;

        // Find occurrences in rows
        for (var i = 0; i < matrix.GetLength(0); i++)
        {
            var row = Utils.GetRow<char>(matrix, i);

            var rowString = new string(row);
            ocurrences += Utils.FindOcurrencesInString(rowString, target);

            var reversedRowString = new string(row.Reverse().ToArray());
            ocurrences += Utils.FindOcurrencesInString(reversedRowString, target);
        }

        // Find occurrences in columns
        for (var i = 0; i < matrix.GetLength(1); i++)
        {
            var column = Utils.GetColumn<char>(matrix, i);

            var columnString = new String(column);
            ocurrences += Utils.FindOcurrencesInString(columnString, target);

            var reversedColumnString = new string(column.Reverse().ToArray());
            ocurrences += Utils.FindOcurrencesInString(reversedColumnString, target);
        }

        // Find ocurrences diagonally
        var diagonals = GetDiagonals(matrix);
        foreach (var diagonal in diagonals)
        {
            ocurrences += Utils.FindOcurrencesInString(new string(diagonal), target);
            ocurrences += Utils.FindOcurrencesInString(new string(diagonal.Reverse().ToArray()), target);
        }

        return ocurrences;
    }

    public static double ResolvePartTwo()
    {
        var matrix = ReadMatrix();
        var count = 0;

        for (var y = 0; y < matrix.GetLength(0); y++)
        {
            var row = Utils.GetRow<char>(matrix, y);
            for (var x = 0; x < matrix.GetLength(1); x++)
            {
                var positionContent = GetContent(matrix, (x, y));
                if (positionContent == 'A')
                {
                    var diag1 = (x - 1, y - 1);
                    if (IsOutOfBounds(matrix, diag1))
                    {
                        continue;
                    }
                    var letterDiag1 = GetContent(matrix, diag1);

                    var diag2 = (x + 1, y - 1);
                    if (IsOutOfBounds(matrix, diag2))
                    {
                        continue;
                    }
                    var letterDiag2 = GetContent(matrix, diag2);

                    var diag3 = (x + 1, y + 1);
                    if (IsOutOfBounds(matrix, diag3))
                    {
                        continue;
                    }
                    var letterDiag3 = GetContent(matrix, diag3);

                    var diag4 = (x - 1, y + 1);
                    if (IsOutOfBounds(matrix, diag4))
                    {
                        continue;
                    }
                    var letterDiag4 = GetContent(matrix, diag4);

                    char[] letters = [
                        letterDiag1,
                        letterDiag2,
                        letterDiag3,
                        letterDiag4,
                    ];

                    if (letters.Count(l => l == 'M') == 2 &&
                        letters.Count(l => l == 'S') == 2 &&
                        letterDiag1 != letterDiag3 &&
                        letterDiag2 != letterDiag4)
                    {
                        count++;
                    }
                }
            }
        }

        return count;
    }

    static IList<string> GetDiagonals(char[,] matrix)
    {
        // Source: https://stackoverflow.com/questions/6313308/get-all-the-diagonals-in-a-matrix-list-of-lists-in-python
        var maxCol = matrix.GetLength(0);
        var maxRow = matrix.GetLength(1);
        var diagonalsQuantity = maxRow + maxCol - 1;
        var fdiagonals = new char[diagonalsQuantity][];
        var bdiagonals = new char[diagonalsQuantity][];
        var min_bdiag = -maxRow + 1;
        for (var x = 0; x < maxCol; x++)
        {
            for (var y = 0; y < maxRow; y++)
            {
                var fdiagonalsIndex = x + y;
                if (fdiagonals[fdiagonalsIndex] == null)
                {
                    fdiagonals[fdiagonalsIndex] = [];
                }
                fdiagonals[fdiagonalsIndex] = fdiagonals[fdiagonalsIndex].Append(matrix[y, x]).ToArray();


                var bdiagonalsIndex = x - y - min_bdiag;
                if (bdiagonals[bdiagonalsIndex] == null)
                {
                    bdiagonals[bdiagonalsIndex] = [];
                }
                bdiagonals[bdiagonalsIndex] = bdiagonals[bdiagonalsIndex].Append(matrix[y, x]).ToArray();
            }
        }

        return fdiagonals.Select(charArray => new string(charArray))
            .Concat(bdiagonals.Select(charArray => new string(charArray)))
            .ToArray();
    }

    static bool IsOutOfBounds(char[,] matrix, (int, int) coordinate)
    {
        return coordinate.Item1 < 0 ||
               coordinate.Item1 >= matrix.GetLength(0) ||
               coordinate.Item2 < 0 ||
               coordinate.Item2 >= matrix.GetLength(1);
    }

    static char GetContent(char[,] matrix, (int, int) coordinate)
    {
        return (char)matrix.GetValue(coordinate.Item2, coordinate.Item1)!;
    }
}
