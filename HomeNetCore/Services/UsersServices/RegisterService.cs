using HomeNetCore.Data.Interfaces;
using HomeNetCore.Models;
using System.Text.RegularExpressions;
namespace HomeNetCore.Services.UsersServices
{
    public class RegisterService(IUserRepository userRepository)
    {
        private readonly IUserRepository _userRepository = userRepository;

        public enum ValidationState
        {
            None,
            Success,
            Error
        }

        // Расширенный результат: теперь включает поле и код ошибки
        public record ValidationResult
        (
            ValidationState State,
            string Message,
            string Field,           // Поле, в котором ошибка (например, "email")
            string ErrorCode         // Код ошибки для локализации/логирования
        );

        public async Task<ValidationResult> RegisterUserAsync(
            string email,
            string password,
            string userName,
            string phone)
        {
            // 1. Проверка на пустые/null
            if (string.IsNullOrWhiteSpace(email))
                return new ValidationResult
                (
                    ValidationState.Error,
                    "Email не может быть пустым",
                    "email",
                    "empty"
                );

            if (string.IsNullOrWhiteSpace(password))
                return new ValidationResult
                (
                    ValidationState.Error,
                    "Пароль не может быть пустым",
                    "password",
                    "empty"
                );

            if (string.IsNullOrWhiteSpace(userName))
                return new ValidationResult
                (
                    ValidationState.Error,
                    "Имя пользователя не может быть пустым",
                    "userName",
                    "empty"
                );

            if (string.IsNullOrWhiteSpace(phone))
                return new ValidationResult
                (
                    ValidationState.Error,
                    "Номер телефона не может быть пустым",
                    "phone",
                    "empty"
                );

            // 2. Валидация email (разделяем ошибки)
            var emailValidation = await ValidateEmailAsync(email);
            if (!emailValidation.IsValid)
                return new ValidationResult
                (
                    ValidationState.Error,
                    emailValidation.Message,
                    "email",
                    emailValidation.ErrorCode
                );

            // 3. Валидация пароля
            if (!ValidatePassword(password))
                return new ValidationResult
                (
                    ValidationState.Error,
                    "Пароль должен содержать минимум 8 символов, буквы и цифры",
                    "password",
                    "invalid_format"
                );

            // 4. Валидация имени
            if (!ValidateUserName(userName))
                return new ValidationResult
                (
                    ValidationState.Error,
                    "Имя может содержать только буквы и пробелы",
                    "userName",
                    "invalid_format"
                );

            // 5. Валидация телефона
            if (!ValidatePhone(phone))
                return new ValidationResult
                (
                    ValidationState.Error,
                    "Некорректный формат телефона (допустимо: +79991234567)",
                    "phone",
                    "invalid_format"
                );

            // 6. Сохраняем пользователя
            await _userRepository.InsertUserAsync(new UserEntity
            {
                Email = email,
                Password = password,
                FirstName = userName,
                PhoneNumber = phone
            });

            return new ValidationResult
            (
                ValidationState.Success,
                "Регистрация успешно завершена",
                "",
                ""
            );
        }




        // Возвращает детальный результат валидации email
        private async Task<(bool IsValid, string Message, string ErrorCode)> ValidateEmailAsync(string email)
        {
            if (!IsValidEmailFormat(email))
                return (false, "Некорректный формат email", "invalid_format");

            if (await _userRepository.EmailExistsAsync(email))
                return (false, "Email уже зарегистрирован", "already_exists");

            return (true, "", "");
        }

        private bool IsValidEmailFormat(string email) =>
            new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$").IsMatch(email);

        private bool ValidatePassword(string password) =>
            new Regex(@"^(?=.*[a-zA-Z])(?=.*\d)[A-Za-z\d]{8,}$").IsMatch(password);

        private bool ValidateUserName(string userName) =>
            !string.IsNullOrEmpty(userName) &&
            new Regex(@"^[a-zA-Z\s]+$").IsMatch(userName);

        private bool ValidatePhone(string phone) =>
            new Regex(@"^\+?\d{10,15}$").IsMatch(phone);
    }







}
