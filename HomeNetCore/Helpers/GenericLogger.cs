using HomeNetCore.Helpers;
using System.Runtime.CompilerServices;

namespace HomeNetCore.Helpers
{
    public class GenericLogger : ILogger
    {
        private readonly Action<string> _output;

        public GenericLogger(Action<string> output)
        {
            _output = output ?? throw new ArgumentNullException(nameof(output));
        }

        public void Log(
            LogLevel level,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            // Форматируем сообщение с параметрами
            string formattedMessage = string.Format(message, args);

            // Извлекаем имя класса из пути к файлу
            string className = string.IsNullOrEmpty(filePath)
                ? "UnknownClass"
                : Path.GetFileNameWithoutExtension(filePath)
                      .Split('.').Last() ?? "Unknown";

            className = CleanName(className);
            memberName = CleanName(memberName);

            var timestamp = DateTime.UtcNow.ToString("MM/dd HH:mm:ss.fff");
            var levelStr = level.ToString().ToUpper();
            var logEntry = $"[{timestamp}] [{levelStr}] {className}.{memberName}:{lineNumber} | {formattedMessage}";

            _output(logEntry);
        }

        public void LogDebug(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Debug, message, memberName, filePath, lineNumber, args);
        }

        public void LogInformation(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Information, message, memberName, filePath, lineNumber, args);
        }

        public void LogWarning(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Warning, message, memberName, filePath, lineNumber, args);
        }

        public void LogError(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Error, message, memberName, filePath, lineNumber, args);
        }

        public void LogCritical(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Critical, message, memberName, filePath, lineNumber, args);
        }

        private string CleanName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "Unknown";

            int startIndex = name.IndexOf('<');
            if (startIndex >= 0)
            {
                int endIndex = name.IndexOf('>');
                if (endIndex > startIndex)
                {
                    name = name.Substring(endIndex + 1);
                }
            }

            name = name.Replace("<", "").Replace(">", "").Trim();
            return string.IsNullOrEmpty(name) ? "Unknown" : name;
        }
    }

}
