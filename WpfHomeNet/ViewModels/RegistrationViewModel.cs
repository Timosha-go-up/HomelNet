using HomeNetCore.Data.Enums;
using HomeNetCore.Data.Interfaces;
using HomeNetCore.Services;
using HomeNetCore.Services.UsersServices;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfHomeNet.ViewModels.WpfHomeNet;

namespace WpfHomeNet.ViewModels
{
    public class RegistrationViewModel : INotifyPropertyChanged
    {
        private readonly RegisterService _registerService;
        private readonly IUserRepository _userRepository;

        // Свойство с полной моделью (как обсуждали)
        public CreateUserInput UserData { get; set; } = new CreateUserInput();

        private RelayCommand _registerCommand;
        public ICommand RegisterCommand => _registerCommand;

        public ICommand ToggleRegistrationCommand { get; private set; }


        private RelayCommand _cancelCommand;
        public ICommand CancelCommand => _cancelCommand;




        private bool _isRegistrationComplete;
        public bool IsRegistrationComplete
        {
            get => _isRegistrationComplete;
            private set => SetField(ref _isRegistrationComplete, value);
        }


        private Visibility _controlVisibility = Visibility.Collapsed;
        public Visibility ControlVisibility
        {
            get => _controlVisibility;
            set => SetField(ref _controlVisibility, value); 
        }


        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }


        private string _registerButtonText = "Зарегистрироваться";
        public string RegisterButtonText
        {
            get => _registerButtonText;
            private set => SetField(ref _registerButtonText, value);
        }

        private void ResetRegistrationForm()
        {
            // Очищаем поля ввода
            UserData = new CreateUserInput(); // или обнуляем свойства вручную

            // Сбрасываем сообщения и состояния валидации
            StatusMessage = string.Empty;
            EmailMessage = string.Empty;
            PasswordMessage = string.Empty;
            NameMessage = string.Empty;
            PhoneMessage = string.Empty;

            EmailValidationState = ValidationState.None;
            PasswordValidationState = ValidationState.None;
            NameValidationState = ValidationState.None;
            PhoneValidationState = ValidationState.None;

            // Включаем поля
            AreFieldsEnabled = true;

            // Возвращаем текст кнопки
            RegisterButtonText = "Зарегистрироваться";

            // Снимаем флаг завершения
            IsRegistrationComplete = false;
        }


        public RegistrationViewModel(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _registerService = new RegisterService(_userRepository);

            // Внутренняя команда (как было)
            _registerCommand = new RelayCommand(
                execute: async (obj) => await ExecuteRegisterCommand(),
                canExecute: (obj) => true
            );



            // В конструкторе:
            _cancelCommand = new RelayCommand(
                execute: (obj) =>
                {
                    // Сбрасываем все состояния
                    ResetRegistrationForm();

                    // Скрываем контрол (если нужно)
                    ControlVisibility = Visibility.Collapsed;
                },
                canExecute: (obj) => true
            );

            ToggleRegistrationCommand = new RelayCommand(
      execute: async (parameter) =>
      {
          if (!IsRegistrationComplete)
          {
              // Первый режим: выполняем регистрацию
              await ExecuteRegisterCommand();
          }
          else
          {
              // Второй режим: кнопка "Выйти"
              ResetRegistrationForm(); // Сброс формы
              ControlVisibility = Visibility.Collapsed; // Скрываем контрол
          }
      },
      canExecute: (parameter) =>
      {
          if (!IsRegistrationComplete)
              return RegisterCommand.CanExecute(null);
          return true; // После регистрации всегда можно выйти
      }
  );
        }


        private async Task ExecuteRegisterCommand()
        {
            
            try
            {
                StatusMessage = string.Empty;
                EmailMessage = string.Empty;
                PasswordMessage = string.Empty;
                NameMessage = string.Empty;
                PhoneMessage = string.Empty;

                var (isSuccess, messages) = await _registerService.RegisterUserAsync(UserData);

                    foreach (var result in messages)
                    {
                        switch (result.Field)
                        {
                            case TypeField.EmailType:
                                EmailMessage = result.Message;
                                EmailValidationState = result.State;
                                break;

                            case TypeField.PasswordType:
                                PasswordMessage = result.Message;
                                PasswordValidationState = result.State;
                                break;

                            case TypeField.NameType:
                                NameMessage = result.Message;
                                NameValidationState = result.State;
                                break;

                            case TypeField.PhoneType:
                                PhoneMessage = result.Message;
                                PhoneValidationState = result.State;
                                break;
                        }
                    }


                if (isSuccess)
                { 
                    StatusMessage = "Вы успешно зарегистрированы";

                    AreFieldsEnabled = false;

                    RegisterButtonText = "Выйти";

                    IsRegistrationComplete = true;

                }

                else StatusMessage = "есть ошибки в полях"; 
               
            }
            catch (Exception ex)
            {
                // Обработка критических ошибок (например, потеря соединения)
                StatusMessage = $"При регистрации произошла ошибка {ex.Message} ";

                AreFieldsEnabled = true;
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


        #region свойства результов валидации


        private bool _areFieldsEnabled = true;
        public bool AreFieldsEnabled
        {
            get => _areFieldsEnabled;
            private set => SetField(ref _areFieldsEnabled, value);
        }


        // Свойства для отображения ошибок в UI
        private string _emailMessage;
        public string EmailMessage
        {
            get => _emailMessage;
            set => SetField(ref _emailMessage, value);
        }

        private string _passwordMessage;
        public string PasswordMessage
        {
            get => _passwordMessage;
            set => SetField(ref _passwordMessage, value);
        }

        private string _nameMessage;
        public string NameMessage
        {
            get => _nameMessage;
            set => SetField(ref _nameMessage, value);
        }

        private string _phoneMessage;
        public string PhoneMessage
        {
            get => _phoneMessage;
            set => SetField(ref _phoneMessage, value);
        }


        private ValidationState _emailValidationState;
        public ValidationState EmailValidationState
        {
            get => _emailValidationState;
            set => SetField(ref _emailValidationState, value);
        }


        private ValidationState _passwordValidationState;
        public ValidationState PasswordValidationState
        {
            get => _passwordValidationState;
            set => SetField(ref _passwordValidationState, value);
        }

        private ValidationState _nameValidationState;
        public ValidationState NameValidationState
        {
            get => _nameValidationState;
            set => SetField(ref _nameValidationState, value);
        }

        private ValidationState _phoneValidationState;
        public ValidationState PhoneValidationState
        {
            get => _phoneValidationState;
            set => SetField(ref _phoneValidationState, value);
        }

        #endregion

    }
}



