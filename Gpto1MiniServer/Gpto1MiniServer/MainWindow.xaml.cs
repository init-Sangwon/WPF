using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace Gpto1MiniClient
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;
        private Thread listenThread;
        private bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                client = new TcpClient();
                client.Connect("127.0.0.1", 5000); // 서버 IP와 포트
                stream = client.GetStream();
                isConnected = true;
                listenThread = new Thread(ListenForMessages);
                listenThread.Start();
                AppendMessage("서버에 연결되었습니다.");
            }
            catch (Exception ex)
            {
                AppendMessage($"서버 연결 실패: {ex.Message}");
            }
        }

        private void ListenForMessages()
        {
            byte[] buffer = new byte[1024];
            int byteCount;
            try
            {
                while (isConnected && (byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Dispatcher.Invoke(() => AppendMessage($"서버: {message}"));
                }
            }
            catch (Exception)
            {
                Dispatcher.Invoke(() => AppendMessage("서버와의 연결이 끊어졌습니다."));
            }
            finally
            {
                isConnected = false;
                stream.Close();
                client.Close();
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (isConnected && !string.IsNullOrWhiteSpace(MessageBox.Text))
            {
                string message = MessageBox.Text;
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                try
                {
                    stream.Write(buffer, 0, buffer.Length);
                    AppendMessage($"나: {message}");
                    MessageBox.Clear();
                }
                catch (Exception ex)
                {
                    AppendMessage($"메시지 전송 실패: {ex.Message}");
                }
            }
        }

        private void AppendMessage(string message)
        {
            ChatBox.AppendText($"{message}\n");
            ChatBox.ScrollToEnd();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            isConnected = false;
            try
            {
                stream.Close();
                client.Close();
            }
            catch { }
        }
    }
}
