using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfHomeNet.UiHelpers
{
    // Конкретная реализация интерфейса для окна авторизации
    public class LoginInputHelper : IInputHelper
    {
        // UI элементы, с которыми будет работать помощник
        private readonly Label _inputInfoTitle;
        private readonly Label _inputHelper;
        private readonly TextBox _inputDataAuthenficate;
        private readonly StackPanel _menu;
        private readonly ContentControl _loginIn;
        private readonly StackPanel _upperMenu;

        // Конструктор с привязкой к UI элементам
        public LoginInputHelper(
            Label inputInfoTitle,
            Label inputHelper,
            TextBox inputDataAuthenficate,
            StackPanel menu,
            ContentControl loginIn,
            StackPanel upperMenu)
        {
            _inputInfoTitle = inputInfoTitle;
            _inputHelper = inputHelper;
            _inputDataAuthenficate = inputDataAuthenficate;
            _menu = menu;
            _loginIn = loginIn;
            _upperMenu = upperMenu;
        }

        // Реализация методов интерфейса

        public bool IsEmailStep() =>
            _inputInfoTitle.Content.ToString() == "Введите ваш email";

        public bool IsPasswordStep() =>
            _inputInfoTitle.Content.ToString() == "Введите пароль от аккаунта" ||
            _inputInfoTitle.Content.ToString() == "Проверьте правильность введённого пароля";

        public void ShowError(string message)
        {
            _inputHelper.Content = message;
            _inputHelper.Foreground = Brushes.Red;
        }

        public void SwitchToPasswordStep()
        {
            _inputInfoTitle.Content = "Введите пароль от аккаунта";
            _inputDataAuthenficate.Text = "";
            _inputHelper.Content = "Email найден. Введите пароль";
            _inputHelper.Foreground = Brushes.DarkGreen;
        }

        public void SetPasswordCheckMode()
        {
            _inputInfoTitle.Content = "Проверьте правильность введённого пароля";
            _inputDataAuthenficate.Text = "";
        }

        public void ShowSuccess()
        {
            _inputHelper.Content = "Успешная авторизация!";
            _inputHelper.Foreground = Brushes.Green;
            _menu.Visibility = Visibility.Visible;
            _loginIn.Visibility = Visibility.Collapsed;
            _menu.IsEnabled = true;
            _upperMenu.IsEnabled = true;
        }


        // Добавляем метод ShowError
       
    }


   
}
