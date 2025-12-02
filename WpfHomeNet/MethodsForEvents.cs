using HomeNetCore.Helpers.Exceptions;
using HomeNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using WpfHomeNet.SubWindows;

namespace WpfHomeNet
{
    public partial class MainWindow
    {
        private async Task ExecuteAddUserOperation(UserEntity newUser, Button button)
        {
            ArgumentNullException.ThrowIfNull(newUser.Email);

            try
            {
                if (await UserService.EmailExistsAsync(newUser.Email))
                {
                    throw new DuplicateEmailException(newUser.Email);
                }

                await UserService.AddUserAsync(newUser);

                Logger.LogInformation($"Пользователь {newUser?.FirstName} добавлен");

                Status.SetStatus($"Пользователь {newUser?.FirstName} добавлен"); await Task.Delay(2000);

                await RefreshDataAsync();

                Status.SetStatus($"Загружено {MainVm.Users.Count} пользователей");
            }
            catch (DuplicateEmailException ex)
            {
                Logger?.LogWarning("Существующий имейл ");

                MessageBox.Show(ex.GetUserMessage(), string.Empty,
                MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }


        private async Task RefreshDataAsync()
        {
            try
            {
                Status.SetStatus("Обновление...");
                await Task.Delay(1000);
                await MainVm.LoadUsersAsync();
                Status.SetStatus("Список обновлён");
                await Task.Delay(1000);
                Status.SetStatus($"Загружено {MainVm.Users.Count} пользователей");
            }
            catch (Exception ex)
            {
                Status.SetStatus($"Ошибка обновления: {ex.Message}");

                _logger?.LogError($"Ошибка обновления: {ex.Message}");
            }
        }


        private void HandleException(Exception ex)
        {
            if (ex is ArgumentNullException)
            {
                MessageBox.Show(
                    $"Критическая ошибка: {ex.Message}",
                    "Ошибка инициализации",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);


                var message = new MessageWindow($"Критическая ошибка: {ex.Message}")
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                message.Show();

            }
        }





        private void ShowWindowLogs(LogWindow logWindow)
        {
            double targetLeft = this.Left +20; // offsetX может быть отрицательным (влево)

           

            var animation = new DoubleAnimation(0, TimeSpan.FromSeconds(2))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            this.BeginAnimation(Window.LeftProperty, animation);

            logWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            logWindow.Left = this.Left + 855;
            logWindow.Top = this.Top;


            logWindow.Show();

            btnLogs.Content = "Скрыть логи";
        }

        private void CenterMainAndHideLogs()
        {
            this.Left = 150;
            this.Top = 200;
            btnLogs.Content = "Показать логи";
        }
    }
}
