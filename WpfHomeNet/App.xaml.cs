using System.Diagnostics;
using System.Windows;
public partial class App : Application
{
    public App()
    {
        // Добавляем обработчики исключений
        this.DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        // Показываем сообщение об ошибке
        MessageBox.Show($"Произошла ошибка: {e.Exception.Message}\n\nStackTrace: {e.Exception.StackTrace}");

        // Помечаем ошибку как обработанную
        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        // Обрабатываем критические ошибки
        var exception = e.ExceptionObject as Exception;
        MessageBox.Show($"Критическая ошибка: {exception?.Message}\n\nStackTrace: {exception?.StackTrace}");
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);

           
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка при запуске: {ex.Message}");
            Environment.Exit(1);
        }
    }
}
