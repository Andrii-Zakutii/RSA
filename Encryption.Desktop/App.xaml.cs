using Encryption.Desktop.ViewModels;
using Encryption.Desktop.Views;
using System.Windows;

namespace Encryption.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MainWindow = new MainWindow();
            MainWindow.DataContext = new MainViewModel();
            MainWindow.Show();
        }
    }
}
