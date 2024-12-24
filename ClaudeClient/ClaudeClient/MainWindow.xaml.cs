using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClaudeClient
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            btnSend.IsEnabled = false;
            txtMessage.IsEnabled = false;
        }

        private async void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("이름을 입력해주세요.");
                    return;
                }

                try
                {
                    client = new TcpClient();
                    await client.ConnectAsync("127.0.0.1", 13000);
                    stream = client.GetStream();

                    // 서버에 이름 전송
                    byte[] nameBytes = Encoding.UTF8.GetBytes(txtName.Text);
                    await stream.WriteAsync(nameBytes, 0, nameBytes.Length);

                    isConnected = true;
                    btnConnect.Content = "연결 끊기";
                    btnSend.IsEnabled = true;
                    txtMessage.IsEnabled = true;
                    txtName.IsEnabled = false;

                    // 메시지 수신 시작
                    _ = ReceiveMessagesAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"서버 연결 실패: {ex.Message}");
                }
            }
            else
            {
                DisconnectFromServer();
            }
        }

        private async Task ReceiveMessagesAsync()
        {
            byte[] buffer = new byte[1024];
            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // UI 스레드에서 메시지 표시
                    Dispatcher.Invoke(() =>
                    {
                        txtChat.AppendText($"{message}\n");
                        txtChat.ScrollToEnd();
                    });
                }
            }
            catch (Exception)
            {
                if (isConnected)
                {
                    Dispatcher.Invoke(() => DisconnectFromServer());
                }
            }
        }

        private async void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(txtMessage.Text) && isConnected)
            {
                try
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(txtMessage.Text);
                    await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
                    txtMessage.Clear();
                }
                catch (Exception)
                {
                    DisconnectFromServer();
                }
            }
        }

        private void DisconnectFromServer()
        {
            isConnected = false;
            stream?.Close();
            client?.Close();

            btnConnect.Content = "접속";
            btnSend.IsEnabled = false;
            txtMessage.IsEnabled = false;
            txtName.IsEnabled = true;

            txtChat.AppendText("서버와의 연결이 종료되었습니다.\n");
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void txtMessage_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            DisconnectFromServer();
            base.OnClosed(e);
        }
    }
}