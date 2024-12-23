using System.Windows;

namespace WpfClient.View
{
    public partial class ChatRoom : Window
    {
        public ChatRoom(string roomId)
        {
            InitializeComponent();
            DataContext = new ViewModel.ChatRoomViewModel(roomId);
        }
    }
}
