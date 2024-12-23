using System.Windows;

namespace WpfClient.View
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.MainWindowViewModel();
        }
    }
}
