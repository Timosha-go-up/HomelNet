using HomeNetCore.Data.Interfaces;
using HomeNetCore.Models;
using HomeNetCore.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using WpfHomeNet.Interfaces;

namespace WpfHomeNet.ViewModels
{
    public class MainViewModel(UserService userService, ILogger logger) : INotifyPropertyChanged, IStatusUpdater
    {
        
      

        public event PropertyChangedEventHandler? PropertyChanged;

        private ObservableCollection<UserEntity> _users = new();
        private string _statusText = string.Empty;

        public ObservableCollection<UserEntity> Users
        {
            get => _users;
            private set
            {
                if (_users == value) return;
                _users = value;
                OnPropertyChanged(nameof(Users));
            }
        }

        public string StatusText
        {
            get => _statusText;
            private set
            {
                if (_statusText == value) return;
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public void SetStatus(string message) => StatusText = message;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task LoadUsersAsync()
        {
            StatusText = "Загрузка...";

            try
            {
                var users = await userService.GetAllUsersAsync();

                if (users == null)
                {
                    HandleError("Получены пустые данные");
                    return;
                }

                Users.Clear();
                Users = new ObservableCollection<UserEntity>(users);

                var userCount = users.Count;

                if (userCount == 0)
                {
                    HandleSuccess($"Список пользователей пуст");
                }
                else
                {
                    HandleSuccess($"Загружено {userCount} пользователей");
                }
            }
            catch (Exception ex)
            {
                HandleError($"Ошибка загрузки: {ex.Message}");
            }
        }

        private void HandleSuccess(string message)
        {
            logger?.LogInformation(message);
            StatusText = message;
        }

        private void HandleError(string message)
        {
            logger?.LogError(message);
            StatusText = message;
        }
    }
}

