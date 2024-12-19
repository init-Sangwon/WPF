using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient AsyncClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton(object sender, RoutedEventArgs e)
        {
            AsyncClient = new TcpClient();
            AsyncClient.Connect("127.0.0.1", 8081);

            if (AsyncClient.Connected)
            {
                Log.Text = "서버와 연결 되었습니다";
            }
        }

        private void DisConnectButton(object sender, RoutedEventArgs e)
        {
            AsyncClient.Close();

            if (AsyncClient.Connected != true)
            {
                Log.Text = "서버와 연결이 끊어졌습니다";
            }
        }
    }
}