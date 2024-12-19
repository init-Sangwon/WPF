using System.Diagnostics.Eventing.Reader;
using System.Net;
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

namespace AsyncServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpListener AsyncServer;
        private bool isRunning;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            AsyncServer = new TcpListener(IPAddress.Any, 8081);
            AsyncServer.Start();
            isRunning = true;

            Log.Text = "서버 실행중";

        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            AsyncServer.Stop();
            Log.Text = "서버 중단";
        }
    }
}