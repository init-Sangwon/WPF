using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfClient.View
{
    /// <summary>
    /// Login.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            DataContext = new ViewModel.LoginViewModel();
        }

        private void SignupButton_Click(object sender, RoutedEventArgs e)
        {
            Signup signupWindow = new Signup();
            signupWindow.Show();

            // 현재 창 닫기
            this.Close();
        }
    }
}
