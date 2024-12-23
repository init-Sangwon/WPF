using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace WpfClient.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<ChatRoom> ChatRooms { get; } = new ObservableCollection<ChatRoom>();
        private ChatRoom? _selectedChatRoom;

        public ChatRoom? SelectedChatRoom
        {
            get => _selectedChatRoom;
            set
            {
                _selectedChatRoom = value;
                OnPropertyChanged(nameof(SelectedChatRoom));
                if (_selectedChatRoom != null)
                {
                    OpenChatRoom(_selectedChatRoom.Id);
                }
            }
        }

        public ICommand CreateRoomCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainWindowViewModel()
        {
            CreateRoomCommand = new RelayCommand(CreateChatRoom);
            LogoutCommand = new RelayCommand(Logout);

            // Load initial chat rooms
            ChatRooms.Add(new ChatRoom { Id = "1", Name = "Room 1" });
            ChatRooms.Add(new ChatRoom { Id = "2", Name = "Room 2" });
        }

        private void CreateChatRoom()
        {
            string newRoomId = (ChatRooms.Count + 1).ToString();
            ChatRooms.Add(new ChatRoom { Id = newRoomId, Name = $"Room {newRoomId}" });
        }

        private void OpenChatRoom(string roomId)
        {
            var chatRoomWindow = new View.ChatRoom(roomId);
            chatRoomWindow.Show();
        }

        private void Logout()
        {
            Application.Current.Shutdown();
        }
    }

    public class ChatRoom
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
