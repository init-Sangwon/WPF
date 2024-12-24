using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GeminiClient
{
    public partial class MainWindow : Window
    {
        private TcpClient client;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string serverIP = ServerIPTextBox.Text; // 서버 IP 주소
                int port = int.Parse(PortTextBox.Text); // 포트 번호
                client = new TcpClient(serverIP, port);
                stream = client.GetStream();
                MessageBox.Show("서버에 연결되었습니다.");

                // 메시지 수신 스레드 시작
                new System.Threading.Thread(ReceiveMessage).Start();

                ConnectButton.IsEnabled = false;
                SendButton.IsEnabled = true;
                MessageTextBox.IsEnabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("연결 오류: " + ex.Message);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = MessageTextBox.Text;
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
                MessageTextBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("전송 오류: " + ex.Message);
            }
        }

        private void ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Dispatcher.Invoke(() => // UI 스레드에서 메시지 표시
                    {
                        ChatTextBox.AppendText(message + "\n");
                    });
                }
            }
            catch
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show("서버와의 연결이 끊어졌습니다.");
                    ConnectButton.IsEnabled = true;
                    SendButton.IsEnabled = false;
                    MessageTextBox.IsEnabled = false;
                });
            }
        }
    }
}