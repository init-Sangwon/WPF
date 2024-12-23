using System;
using System.Net.Sockets;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace ChatClient2
{
    public partial class MainWindow : Window
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private readonly int _userId; // In a real app, this would come from login

        public MainWindow()
        {
            InitializeComponent();
            _userId = 1; // Hardcoded for demonstration
            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync("127.0.0.1", 8888);
                _stream = _client.GetStream();

                // Start listening for messages
                _ = ReceiveMessages();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server: {ex.Message}");
            }
        }

        private async Task ReceiveMessages()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                try
                {
                    int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var chatMessage = JsonConvert.DeserializeObject<ChatMessage>(message);

                    // Update UI on main thread
                    Dispatcher.Invoke(() =>
                    {
                        MessageList.Items.Add($"{chatMessage.SentAt:HH:mm:ss} - User{chatMessage.SenderId}: {chatMessage.Content}");
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error receiving message: {ex.Message}");
                    break;
                }
            }
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MessageInput.Text))
                return;

            try
            {
                var message = new ChatMessage
                {
                    SenderId = _userId,
                    Content = MessageInput.Text,
                    SentAt = DateTime.Now
                };

                string jsonMessage = JsonConvert.SerializeObject(message);
                byte[] buffer = Encoding.UTF8.GetBytes(jsonMessage);
                await _stream.WriteAsync(buffer, 0, buffer.Length);

                MessageInput.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error sending message: {ex.Message}");
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            _client?.Close();
            base.OnClosing(e);
        }
    }

    public class ChatMessage
    {
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }
}