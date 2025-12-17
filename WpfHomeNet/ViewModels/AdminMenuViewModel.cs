using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using WpfHomeNet.UiHelpers;



namespace WpfHomeNet.ViewModels
{
    public class AdminMenuViewModel : INotifyPropertyChanged
    {
       
        private Window _mainWindow;
        private MainViewModel _mainViewModel;
        

        private readonly LogQueueManager _logQueueManager;
        private bool _isSubscribedToMainWindowEvents;
        // Команда-переключатель
        public ICommand ToggleLogWindowCommand { get; }

        // Свойство для текста кнопки
        private string _toggleButtonText = "Показать лог";
        public string ToggleButtonText
        {
            get => _toggleButtonText;
            set
            {
                _toggleButtonText = value;
                OnPropertyChanged(nameof(ToggleButtonText));
            }
        }

        public AdminMenuViewModel( Window mainWindow, LogQueueManager logQueueManager
            )
        {
           

            _logQueueManager = logQueueManager;

            _mainWindow = mainWindow;

            ToggleLogWindowCommand = new RelayCommand(ExecuteToggleLogWindow);

       
        }


        public void ConnectToMainViewModel(MainViewModel mainVm)
        {
            if (mainVm == null)
                throw new ArgumentNullException(nameof(mainVm));

            _mainViewModel = mainVm;
        }









        private void ExecuteToggleLogWindow(object? parameter)
        {
            if (_mainViewModel.LogWindow.Visibility == Visibility.Visible)
            {
                // Скрываем окно
                _mainViewModel.LogWindow.Hide();
                // Логика при скрытии
                ToggleButtonText = "Показать лог"; // Меняем текст кнопки

                if (_isSubscribedToMainWindowEvents)
                {
                    _mainWindow.LocationChanged -= OnMainWindowMoved;
                    _mainWindow.SizeChanged -= OnMainWindowResized;
                    _isSubscribedToMainWindowEvents = false;
                }
            }
            else
            {
                // При первом показе — подписываемся на события
                if (!_isSubscribedToMainWindowEvents)
                {
                    _mainWindow.LocationChanged += OnMainWindowMoved;
                    _mainWindow.SizeChanged += OnMainWindowResized;
                    _isSubscribedToMainWindowEvents = true;
                }

                PositionLogWindow();
                _mainViewModel.LogWindow.Show();
                _logQueueManager.SetReady();
                ToggleButtonText = "Скрыть лог";
            }
        }



        public void ConnectMainWindow(Window mainWindow)
        {
            _mainWindow = mainWindow;

            // Только теперь подписываемся на события
            _mainWindow.LocationChanged += OnMainWindowMoved;
            _mainWindow.SizeChanged += OnMainWindowResized;

            // Если лог-окно уже видно — сразу позиционируем
            if (_mainViewModel.LogWindow.Visibility == Visibility.Visible)
                PositionLogWindow();
        }




        // INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void PositionLogWindow()
        {
            _mainViewModel.LogWindow.Left = _mainWindow.Left + _mainWindow.Width +5;
            _mainViewModel.LogWindow.Top = _mainWindow.Top;
            _mainViewModel.LogWindow.Height = _mainWindow.Height;
            _mainViewModel.LogWindow.Width = 600;
        }

        private void OnMainWindowMoved(object sender, EventArgs e)
        {
            if (_mainViewModel.LogWindow.Visibility == Visibility.Visible) PositionLogWindow();
        }

        private void OnMainWindowResized(object sender, SizeChangedEventArgs e)
        {
            if (_mainViewModel.LogWindow.Visibility == Visibility.Visible) PositionLogWindow();
        }


        public void Dispose()
        {
            _mainWindow.LocationChanged -= OnMainWindowMoved;
            _mainWindow.SizeChanged -= OnMainWindowResized;
        }
    }
}
