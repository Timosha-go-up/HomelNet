using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfHomeNet.UiHelpers
{

    // Интерфейс для работы с UI элементами авторизации
    public interface IInputHelper
    {
        // Проверка текущего этапа на этап ввода email
        bool IsEmailStep();

        // Проверка текущего этапа на этап ввода пароля
        bool IsPasswordStep();

        // Отображение сообщения об ошибке
        void ShowError(string message);

        // Переключение на этап ввода пароля
        void SwitchToPasswordStep();

        // Активация режима проверки пароля
        void SetPasswordCheckMode();

        // Отображение успешного завершения авторизации
        void ShowSuccess();
    }


   
}
