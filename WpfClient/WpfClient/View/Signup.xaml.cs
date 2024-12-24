using System.Windows;
using WpfClient.ViewModel;

namespace WpfClient.View
{
    public partial class Signup : Window
    {
        public Signup()
        {
            InitializeComponent();

            DataContext = new SignupViewModel();
        }
    }
}
