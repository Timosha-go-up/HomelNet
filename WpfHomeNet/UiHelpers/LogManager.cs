using System.Collections.Concurrent;
using System.Diagnostics;

namespace WpfHomeNet.UiHelpers
{

    public class LogManager
    {
        private readonly ConcurrentQueue<(string level, string message)> _logQueue = new();
        private bool _isProcessing;
        private LogWindow _logWindow;
        private string _currentAnimation = "";

        public LogManager(LogWindow logWindow)
        {
            _logWindow = logWindow ?? throw new ArgumentNullException(nameof(logWindow));
        }

        public void WriteLog(string message)
        {
            Debug.WriteLine($"Получено сообщение для логирования: {message}");

            // Удаляем лишние переносы строк
            message = message
                .Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine)
                .Trim('\r', '\n');

            var level = ExtractLogLevel(message);
            _logQueue.Enqueue((level, message));

            if (!_isProcessing)
                ProcessLogQueue();
        }

        private string ExtractLogLevel(string message)
        {
            try
            {
                int[] indices = new int[]
                {
                message.IndexOf('['),
                message.IndexOf(']', message.IndexOf('[')),
                message.IndexOf('[', message.IndexOf(']')),
                message.IndexOf(']', message.IndexOf('[', message.IndexOf(']')) + 1)
                };

                if (indices[0] >= 0 && indices[1] > indices[0] &&
                    indices[2] > indices[1] && indices[3] > indices[2])
                {
                    return message.Substring(indices[2] + 1, indices[3] - indices[2] - 1)
                                 .Trim()
                                 .ToUpper();
                }
            }
            catch { }

            return "INFO";
        }

        // В LogManager добавим проверку и обработку первой строки
        private async void ProcessLogQueue()
        {
            if (_isProcessing) return;
            _isProcessing = true;

            try
            {
                bool isFirstMessage = true; // Флаг для первой строки

                while (_logQueue.TryDequeue(out var logEntry))
                {
                    // Проверяем первую строку на лишние переносы
                    if (isFirstMessage)
                    {
                        // Удаляем лишние переносы только для первой строки
                        logEntry.message = logEntry.message
                            .Replace(Environment.NewLine + Environment.NewLine, Environment.NewLine)
                            .TrimStart('\r', '\n');

                        isFirstMessage = false;
                    }

                    _currentAnimation = "";
                    foreach (char c in logEntry.message)
                    {
                        _currentAnimation += c;
                        await _logWindow.AddLog(_currentAnimation, logEntry.level, true);
                        await Task.Delay(25);
                    }

                    // Добавляем перенос строки только если сообщение не заканчивается на перенос
                    if (!logEntry.message.EndsWith(Environment.NewLine))
                    {
                        await _logWindow.AddLog(Environment.NewLine, logEntry.level, false);
                    }
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

    }







}
