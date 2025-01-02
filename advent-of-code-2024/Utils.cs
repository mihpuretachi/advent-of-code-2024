using System.Text;
using System.Text.RegularExpressions;

namespace advent_of_code_2024
{
    internal static class Utils
    {
        public static IList<string> ReadLinesFromChallengeSource(int day)
        {
            var result = new List<string>();
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead($"./challenges/day-{day.ToString("00")}/source.txt"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()!) != null)
                {
                    result.Add(line);
                }
            }
            return result;
        }

        public static string ReadChallengeSource(int day)
        {
            return File.ReadAllText($"./challenges/day-{day.ToString("00")}/source.txt");
        }

        public static int FindOcurrencesInString(string text, string target)
        {
            return new Regex(Regex.Escape(target)).Matches(text).Count;
        }

        public static T[] GetColumn<T>(T[,] matrix, int columnNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(0))
                    .Select(x => matrix[x, columnNumber])
                    .ToArray();
        }

        public static T[] GetRow<T>(T[,] matrix, int rowNumber)
        {
            return Enumerable.Range(0, matrix.GetLength(1))
                    .Select(x => matrix[rowNumber, x])
                    .ToArray();
        }

        public static IList<string[]> CombinationsWithRepetition(IEnumerable<string> input, int length)
        {
            return DoCombinationsWithRepetition(input, length)
                .Select(s => Regex.Split(s, string.Empty).Where(s => !string.IsNullOrEmpty(s)).ToArray()).ToList();
        }

        private static IEnumerable<String> DoCombinationsWithRepetition(IEnumerable<string> input, int length)
        {
            if (length <= 0)
                yield return "";
            else
            {
                foreach (var i in input)
                    foreach (var c in DoCombinationsWithRepetition(input, length - 1))
                        yield return i + c;
            }
        }
    }
}
