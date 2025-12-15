using System.Windows;
using System.Windows.Input;

namespace WpfHomeNet
{
    public partial class MainWindow
    {
       

      
        #region управление оконами

        /// <summary>
        /// таскальщик главного с окном логов с позиционированием справа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>

        private void WindowDrag_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove(); // Перемещаем главное окно
            }
        }


        /// <summary>
        /// закрывает главное окно одно временно с окном логов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="InvalidOperationException"></exception>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();

            LogWindow.Close();
        }


        #endregion


       


        private async void RefreshStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await RefreshDataAsync();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }


      private void SyncLogWindowPosition(object? sender, EventArgs e)
{
    if (!LogWindow.IsLoaded || !LogWindow.IsVisible) return;

    LogWindow.WindowStartupLocation = WindowStartupLocation.Manual;

    const double margin = 2;
    double targetLeft = this.Left + this.ActualWidth + margin;
    double targetTop = this.Top;

    var workArea = SystemParameters.WorkArea;

    LogWindow.Left = targetLeft;
    LogWindow.Top = targetTop;
}


    }
}





