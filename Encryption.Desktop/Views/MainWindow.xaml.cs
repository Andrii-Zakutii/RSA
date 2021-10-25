using Encryption.Desktop.ViewModels;
using Microsoft.Win32;
using System.Windows;

namespace Encryption.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ChooseFile(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;
                var viewModel = (MainViewModel)DataContext;
                viewModel.FilePath = filePath;
            }
        }
    }
}
