using HomeNetCore.Data.Enums;
using HomeNetCore.Data.Interfaces;
using HomeNetCore.Models;
using HomeNetCore.Services.UsersServices;

namespace HomeNetCore.Services
{
   

    public partial class RegisterService(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ValidationFormat _validateField = new();


        /// <summary>
        /// Регистрирует пользователя: валидирует данные и сохраняет в БД.
        /// </summary>
        /// <param name="userInput">Данные регистрации.</param>
        /// <returns>
        /// Tuple: (пользователь, список ошибок).  
        /// Если ошибки есть — пользователь null.
        /// </returns>
        public async Task<(bool IsSuccess, List<ValidationResult> Errors)> RegisterUserAsync(CreateUserInput userInput)
        {
            var validationResults = new List<ValidationResult>();
            
            validationResults.Add(ValidatePassword(userInput.Password));
            validationResults.Add(ValidateUserName(userInput.UserName));
            validationResults.Add(ValidatePhone(userInput.PhoneNumber));
            var emailResult = await ValidateEmailAsync(userInput.Email);
            validationResults.Add(emailResult);

            // 2. Если есть ошибки — возвращаем их
            if (validationResults.Any(r => r.State == ValidationState.Error))
            {
                return (false, validationResults);
            }


            var user = new UserEntity
            {
                FirstName = userInput.UserName,
                Email = userInput.Email,
                PhoneNumber = userInput.PhoneNumber,
                Password = userInput.Password
            };

            try
            {
                //await _userRepository.InsertUserAsync(user);

                return (true, new List<ValidationResult>());  
            }
            catch (Exception ex)
            {
                
                var errorResult = new ValidationResult
                {
                    State = ValidationState.Error,
                    Message = $"Ошибка сохранения пользователя: {ex.Message}"
                };
                return (false, new List<ValidationResult> { errorResult });
            }
        }
       

        private ValidationResult ValidatePassword(string password)
        {
            var result = new ValidationResult { Field = TypeField.PasswordType };

            if (string.IsNullOrWhiteSpace(password))
            {
                result.State = ValidationState.Error;
                result.Message = "Пароль не может быть пустым";
                return result;
            }

            if (!_validateField.ValidatePasswordFormat(password))
            {
                result.State = ValidationState.Error;
                result.Message = "Пароль должен содержать минимум 8 символов, буквы и цифры";
                return result;
            }

            result.State = ValidationState.Success;
            result.Message = "Пароль принят";
            return result;
        }


        private ValidationResult ValidateUserName(string userName)
        {
            var result = new ValidationResult { Field = TypeField.NameType };

            if (string.IsNullOrWhiteSpace(userName))
            {
                result.State = ValidationState.Error;
                result.Message = "Имя пользователя не может быть пустым";
                return result;
            }

            if (!_validateField.ValidateUserNameFormat(userName))
            {
                result.State = ValidationState.Error;
                result.Message = "Допустимо минимум 3 буквы подряд без пробелов";
                return result;
            }

            result.State = ValidationState.Success;
            result.Message = "Имя пользователя принято";
            return result;
        }

        private ValidationResult ValidatePhone(string phone)
        {
            var result = new ValidationResult { Field = TypeField.PhoneType };

            if (string.IsNullOrWhiteSpace(phone))
            {
                result.State = ValidationState.Error;
                result.Message = "Номер телефона не может быть пустым";
                return result;
            }

            if (!_validateField.ValidatePhoneFormat(phone))
            {
                result.State = ValidationState.Error;
                result.Message = "Некорректный формат телефона (допустимо: +79991234567)";
                return result;
            }

            result.State = ValidationState.Success;
            result.Message = "Номер телефона принят";
            return result;
        }

        private async Task<ValidationResult> ValidateEmailAsync(string email)
        {
            var result = new ValidationResult { Field = TypeField.EmailType };

            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    result.State = ValidationState.Error;
                    result.Message = "Email не может быть пустым";
                    return result;
                }

                if (!_validateField.IsValidEmailFormat(email))
                {
                    result.State = ValidationState.Error;
                    result.Message = "Некорректный формат email";
                    return result;
                }

                if (await _userRepository.EmailExistsAsync(email))
                {
                    result.State = ValidationState.Error;
                    result.Message = "Email уже зарегистрирован";
                    return result;
                }

                result.State = ValidationState.Success;
                result.Message = "Email принят";
                return result;
            }
            catch (Exception ex)
            {
                result.State = ValidationState.Error;
                result.Message = $"Ошибка проверки email: {ex.Message}";
                return result;
            }
        }
    }

}
