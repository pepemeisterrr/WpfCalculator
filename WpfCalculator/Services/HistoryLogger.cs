using System;
using System.Globalization;
using System.IO;

namespace WpfCalculator.Services
{
    public static class HistoryLogger
    {
        private static readonly string HistoryFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "calculator_history.txt");

        public static void Log(string expression, double result, DateTime time)
        {
            try
            {
                string directory = Path.GetDirectoryName(HistoryFilePath)!;
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                string normalizedExpression = expression.Replace(",", ".");
                string resultStr = result.ToString(CultureInfo.InvariantCulture);

                string line = $"[{time:dd.MM.yyyy HH:mm:ss}] {normalizedExpression} = {resultStr}";
                File.AppendAllText(HistoryFilePath, line + Environment.NewLine);
            }
            catch
            {
               
            }
        }
    }
}