using HomeNetCore.Helpers;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;

namespace HomeNetCore.Helpers
{
    public class Logger : ILogger
    {
        private readonly Action<string> _output;
        

        public Logger(Action<string> output)
        {
            _output = output;
        }

        public virtual void Log(
            LogLevel level,
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {            
            string formattedMessage = string.Format(message, args);

            string className = string.IsNullOrEmpty(filePath)
                ? "UnknownClass"
                : Path.GetFileNameWithoutExtension(filePath)
                      .Split('.').Last() ?? "Unknown";

            className = CleanName(className);
            memberName = CleanName(memberName);
            
            var timestamp = DateTime.UtcNow.ToString("MM/dd HH:mm:ss.fff");
            var levelStr = level.ToString().ToUpper();
           
            var logEntry = $"[{timestamp}] [{levelStr}] [{className}.{memberName}:{lineNumber}] | {formattedMessage}";
            _output(logEntry);
        }

        public  virtual void LogDebug(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Debug, message, memberName, filePath, lineNumber, args);
        }

        public virtual void LogInformation(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Information, message, memberName, filePath, lineNumber, args);
        }

        public virtual void LogWarning(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Warning, message, memberName, filePath, lineNumber, args);
        }

        public virtual void LogError(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0,
            params object[] args)
        {
            Log(LogLevel.Error, message, memberName, filePath, lineNumber, args);
        }

        public virtual void LogCritical(
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
            return name.Replace('_', ' ')
                      .Trim()
                      .Replace(".", " ")
                      .Replace("`", "");
        }
    }
}
