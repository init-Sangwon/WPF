using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfClient.ViewModel
{
    public class ChatRoomViewModel : ViewModelBase
    {
        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();
        private readonly string _roomId;

        private string _messageText = string.Empty;
        public string MessageText
        {
            get => _messageText;
            set
            {
                _messageText = value;
                OnPropertyChanged(nameof(MessageText));
            }
        }

        public ICommand SendMessageCommand { get; }

        public ChatRoomViewModel(string roomId)
        {
            _roomId = roomId;
            SendMessageCommand = new RelayCommand(async () => await SendMessage());
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(MessageText)) return;

            string messageToSend = $"MESSAGE|{_roomId}|{MessageText}";
            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 8081))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(messageToSend);
                    await stream.WriteAsync(data, 0, data.Length);

                    Messages.Add($"나: {MessageText}");
                    MessageText = string.Empty;
                }
            }
            catch
            {
                Messages.Add("메시지 전송 실패");
            }
        }
    }
}
