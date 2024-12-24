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
using Microsoft.VisualBasic;

namespace HumanClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("이름을 입력해주세요.");
                return;
            }

            try
            {
                // 연결이 이미 되어 있으면 새로 연결하지 않음
                if (client == null || !client.Connected)
                {
                    client = new TcpClient();
                    await client.ConnectAsync("127.0.0.1", 8080);
                    stream = client.GetStream();

                    // 이름 전송
                    byte[] name = Encoding.UTF8.GetBytes(txtName.Text);
                    await stream.WriteAsync(name, 0, name.Length);

                    // 서버의 환영 메시지 수신
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string serverMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    MessageBox.Show($"{serverMessage}");
                }
                else
                {
                    MessageBox.Show("이미 연결되어 있습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"연결 오류: {ex.Message}");
            }
        }


        private async void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if (client == null || !client.Connected)
            {
                MessageBox.Show("먼저 서버에 연결하세요.");
                return;
            }

            try
            {

                string message = txtMessage.Text.Trim();
                string combinedMessage = $"{message}";

                byte[] messageBytes = Encoding.UTF8.GetBytes(combinedMessage);
                await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                txtMessage.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"메시지 전송 오류: {ex.Message}");
            }
        }
    }
}