using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using static WpfHomeNet.ViewModels.LogViewModel;



namespace WpfHomeNet.ViewModels
{
    public class AdminMenuViewModel : INotifyPropertyChanged
    {       
        private MainViewModel? _mainVm;

        public MainViewModel MainVm
        {
            get => _mainVm ?? throw new InvalidOperationException($"{nameof(_mainVm)} не инициализирован");
            set => _mainVm = value;
        }
       
        public ICommand ToggleLogWindowCommand { get; }

        public ICommand UserTableViewCommand { get; set; }

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

        public AdminMenuViewModel()
        {
            ToggleLogWindowCommand = new RelayCommand(ExecuteToggleLogWindow);

            UserTableViewCommand = new RelayCommand(parameter => ExecuteUserTableViewVisible());


        }

        public void ConnectToMainViewModel(MainViewModel mainVm) => MainVm = mainVm;


        // В AdminMenuViewModel
        private void ExecuteToggleLogWindow(object? parameter)
        {
            if (MainVm.LogVm.ShowLogWindowDelegate == null)
                return;

            var state = MainVm.LogVm.ShowLogWindowDelegate();

            ToggleButtonText = state == LogWindowState.Visible ? "Скрыть лог" : "Показать лог";
        }


        private void ExecuteUserTableViewVisible()
        {
           
            MainVm.PanelVisibility = MainVm.PanelVisibility == Visibility.Collapsed
                    ? Visibility.Visible 
                    : Visibility.Collapsed;


        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
 
    }
}
