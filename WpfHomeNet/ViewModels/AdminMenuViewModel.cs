using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfHomeNet.ViewModels
{
    class AdminMenuViewModel
    {
        LogWindow _logWindow;

        RegistrationViewModel _registrationViewModel;


        public event PropertyChangedEventHandler? PropertyChanged;

        public AdminMenuViewModel(LogWindow logWindow, RegistrationViewModel registrationViewModel  )
        {
            _logWindow = logWindow;
            _registrationViewModel = registrationViewModel;
        }

        public ICommand ShowRegistrationCommand => new RelayCommand(_ =>
        {
            if (_registrationViewModel != null)
            {
                _registrationViewModel.ControlVisibility =
                    _registrationViewModel.ControlVisibility == Visibility.Collapsed
                        ? Visibility.Visible
                        : Visibility.Collapsed;

                OnPropertyChanged(nameof(IsButtonsPanelEnabled));
            }
        });

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public bool IsButtonsPanelEnabled =>
             !(_registrationViewModel?.ControlVisibility == Visibility.Visible); 
    }
}
