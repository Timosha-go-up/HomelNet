using System.Windows;

namespace WpfHomeNet.SubWindows
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public MessageWindow(string message ="")
        {
            InitializeComponent();


        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
           
            Close();
        }

       


           
    }
}
