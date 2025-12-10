using HomeNetCore.Data.Interfaces;
using HomeNetCore.Services;
using HomeNetCore.Services.UsersServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using WpfHomeNet.ViewModels.WpfHomeNet;

namespace WpfHomeNet.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private readonly RegisterService _registerService;
        private readonly IUserRepository _userRepository;

        // Свойство с полной моделью (как обсуждали)
        public CreateUserInput UserData { get; } = new CreateUserInput();

        private RelayCommand _registerCommand;
        public ICommand RegisterCommand => _registerCommand;

        // Свойства для отображения ошибок в UI
        private string _emailError;
        public string EmailError
        {
            get => _emailError;
             set => SetField(ref _emailError, value);
        }

        private string _passwordError;
        public string PasswordError
        {
            get => _passwordError;
             set => SetField(ref _passwordError, value);
        }

        private string _nameError;
        public string NameError
        {
            get => _nameError;
            set => SetField(ref _nameError, value);
        }

        private string _phoneError;
        public string PhoneError
        {
            get => _phoneError;
             set => SetField(ref _phoneError, value);
        }

        public RegistrationViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _registerService = new RegisterService(_userRepository);

            _registerCommand = new RelayCommand(
                execute: async (obj) => await ExecuteRegisterCommand(),
                canExecute: (obj) => true
            );
        }

        private async Task ExecuteRegisterCommand()
        {
            try
            {
                // Очищаем предыдущие ошибки
                EmailError = string.Empty;
                PasswordError = string.Empty;
                NameError = string.Empty;
                PhoneError = string.Empty;

                var (isSuccess, errors) = await _registerService.RegisterUserAsync(UserData);

                if (!isSuccess)
                {
                    foreach (var result in errors)
                    {
                        switch (result.Field)
                        {
                            case TypeField.EmailType:
                                EmailError = result.Message;
                                break;

                            case TypeField.PasswordType:
                                PasswordError = result.Message;
                                break;

                            case TypeField.NameType:
                                NameError = result.Message;
                                break;

                            case TypeField.PhoneType:
                                PhoneError = result.Message;
                                break;
                        }
                    }
                }
                else
                {
                    // Успешная регистрация — можно закрыть форму или показать сообщение
                }
            }
            catch (Exception ex)
            {
                // Обработка критических ошибок (например, потеря соединения)
                EmailError = "Произошла ошибка при регистрации. Попробуйте ещё раз.";
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



