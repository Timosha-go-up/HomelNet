using HomeNetCore.Data.Enums;
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





        public ValidationState EmailValidationState
        {
            get => _emailValidationState;
            set
            {

                _emailValidationState = value;
                OnPropertyChanged(nameof(EmailValidationState));

            }


        }

        public ValidationState PasswordValidationState
        {
            get => _passwordValidationState;
            set
            {

                _passwordValidationState = value;
                OnPropertyChanged(nameof(PasswordValidationState));

            }


        }

        public ValidationState NameValidationState
        {
            get => _nameValidationState;
            set
            {

                _nameValidationState = value;
                OnPropertyChanged(nameof(NameValidationState));

            }

        }

        public ValidationState PhoneValidationState
        {
            get => _phoneValidationState;
            set
            {

                _phoneValidationState = value;
                OnPropertyChanged(nameof(PhoneValidationState));

            }
        }

        // Приватные поля
        private ValidationState _emailValidationState = ValidationState.None;
        private ValidationState _passwordValidationState = ValidationState.None;
        private ValidationState _nameValidationState = ValidationState.None;
        private ValidationState _phoneValidationState = ValidationState.None;


      

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
                                EmailValidationState = result.State;
                                break;

                            case TypeField.PasswordType:
                                PasswordError = result.Message;
                                PasswordValidationState = result.State;
                                break;

                            case TypeField.NameType:
                                NameError = result.Message;
                                NameValidationState = result.State;
                                break;

                            case TypeField.PhoneType:
                                PhoneError = result.Message;
                                PhoneValidationState = result.State;
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



