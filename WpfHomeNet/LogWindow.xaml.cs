using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;



namespace WpfHomeNet
{

    public partial class LogWindow : Window
    {
        private Dictionary<string, Brush> _colorMap = new()
    {
        {"CRITICAL", Brushes.Red},
        {"ERROR", Brushes.Orange},
        {"WARNING", Brushes.Yellow},
        {"INFO", Brushes.Black},
        {"DEBUG", Brushes.Blue},
        {"TRACE", Brushes.Gray}
    };

        private bool _isFirstMessage = true; // Флаг для первого сообщения

        public LogWindow()
        {
            InitializeComponent();
        }

        public async Task AddLog(string text, string level, bool isAnimating)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                try
                {
                    if (LogTextBox.Document == null)
                    {
                        LogTextBox.Document = new FlowDocument();
                    }

                   

                    if (isAnimating)
                    {
                        var lastParagraph = GetLastParagraph(LogTextBox.Document);

                        if (lastParagraph == null)
                        {
                            lastParagraph = new Paragraph
                            {
                               
                            };
                            LogTextBox.Document.Blocks.Add(lastParagraph);
                        }

                        lastParagraph.Inlines.Clear();
                        var run = new Run(text)
                        {
                            Foreground = _colorMap.ContainsKey(level) ? _colorMap[level] : Brushes.White
                        };
                        lastParagraph.Inlines.Add(run);
                    }
                    else
                    {
                        AddNewLine(text, level);
                    }

                    LogTextBox.ScrollToEnd();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка при обновлении лога: {ex.Message}");
                }
            }, DispatcherPriority.Normal);
        }

        private Paragraph? GetLastParagraph(FlowDocument document)
        {
            if (document?.Blocks == null || document.Blocks.Count == 0)
                return null;
            return document.Blocks.LastBlock as Paragraph;
        }

        private void AddNewLine(string text, string level)
        {
            if (LogTextBox.Document == null)
            {
                LogTextBox.Document = new FlowDocument();
            }

            var paragraph = new Paragraph
            {
               
            };

            var run = new Run(text)
            {
                Foreground = _colorMap.ContainsKey(level) ? _colorMap[level] : Brushes.White
            };

            paragraph.Inlines.Add(run);
            LogTextBox.Document.Blocks.Add(paragraph);
        }
    }







}