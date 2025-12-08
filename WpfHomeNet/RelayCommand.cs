using System.Windows.Input;

namespace WpfHomeNet.ViewModels
{
    using System.Diagnostics;
    using System.Windows.Input;

    namespace WpfHomeNet
    {
        public class RelayCommand : ICommand
        {
            private readonly Action<object> _execute;
            private readonly Predicate<object> _canExecute;

            public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public bool CanExecute(object parameter)
            {
                Debug.WriteLine($"CanExecute проверка: {(_canExecute == null || _canExecute(parameter))}");
                return _canExecute == null || _canExecute(parameter);
            }


            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            public void RaiseCanExecuteChanged()
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }





}
