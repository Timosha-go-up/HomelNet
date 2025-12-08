
using HomeNetCore.Services.UsersServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfHomeNet.ViewModels.WpfHomeNet;
using static HomeNetCore.Services.UsersServices.RegisterService;

namespace WpfHomeNet.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private readonly RegisterService _registerService;
        private RelayCommand _registerCommand; 
        public ICommand RegisterCommand => _registerCommand;


       



        // Свойства для данных пользователя
        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                SetField(ref _email, value);
                _registerCommand?.RaiseCanExecuteChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                SetField(ref _password, value);
                _registerCommand?.RaiseCanExecuteChanged();
            }
        }

        private string _userName = string.Empty;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                SetField(ref _userName, value);
                _registerCommand?.RaiseCanExecuteChanged();
            }
        }

        private string _phone = string.Empty;
        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                SetField(ref _phone, value);
                _registerCommand?.RaiseCanExecuteChanged();
            }
        }

        // Свойства для отображения результата
        private ValidationState _validationState = ValidationState.None;
        public ValidationState ValidationState
        {
            get => _validationState;
            set => SetField(ref _validationState, value); // Однострочный сеттер
        }

        private string _errorMessage = string.Empty;
        private string _emailError;

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                SetField(ref _errorMessage, value);
            }
        }

        public RegistrationViewModel(RegisterService registerService)
        {
            if (registerService is null)
                throw new ArgumentNullException(nameof(registerService));

            _registerService = registerService;

            _registerCommand = new RelayCommand(
                execute: async (obj) => await ExecuteRegisterCommand(),
                canExecute: (obj) => true // Кнопка всегда активна
            );
        }



        
        

        private async Task ExecuteRegisterCommand()
        {
            try
            {
                ValidationState = ValidationState.Success;
                ErrorMessage = string.Empty;

                var result = await _registerService.RegisterUserAsync(
                    Email,
                    Password,
                    UserName,
                    Phone);

                if (result.State == ValidationState.Error)
                {
                    ValidationState = ValidationState.Error;
                    ErrorMessage = result.Message; 
                   
                   
                    return;
                }

                ValidationState = ValidationState.Success;

                ErrorMessage = result.Message;

                // Очистка полей после успешной регистрации
                Email = string.Empty;
                Password = string.Empty;
                UserName = string.Empty;
                Phone = string.Empty;
            }
            catch (Exception ex)
            {
                ValidationState = ValidationState.Error;
                ErrorMessage = $"Произошла ошибка: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


