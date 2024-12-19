using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace chatClient
{
    public partial class MainWindow : Window
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private Thread _receiveThread;
        private bool _isConnected = false;
        private string _currentRoom = null;
        private string _nickname = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ReceiveMessages()
        {
            try
            {
                byte[] buffer = new byte[1024];
                _stream.ReadTimeout = 500; // 500ms 대기 후 타임아웃

                while (_isConnected)
                {
                    try
                    {
                        if (_stream.DataAvailable)
                        {
                            int bytesRead = _stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break; // 스트림이 닫혔을 때 종료

                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Dispatcher.Invoke(() =>
                            {
                                if (message.StartsWith("[ROOM_LIST]"))
                                {
                                    UpdateRoomList(message.Substring(11));
                                }
                                else
                                {
                                    LogMessage(message);
                                    ShowPopup(message); // 팝업 표시
                                }
                            });
                        }
                    }
                    catch (IOException)
                    {
                        // ReadTimeout 발생 시 무시
                    }
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => LogMessage($"메시지 수신 중 오류: {ex.Message}"));
            }
            finally
            {
                // 수신 스레드 종료 후 UI 상태 업데이트
                Dispatcher.Invoke(() =>
                {
                    LogMessage("서버 연결이 종료되었습니다.");
                    UpdateUIState(connected: false);
                });
            }
        }

        private void ShowPopup(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                PopupWindow popup = new PopupWindow(message)
                {
                    Left = SystemParameters.WorkArea.Width - 300,
                    Top = SystemParameters.WorkArea.Height - 100
                };
                popup.Show();

                // 3초 후 팝업 닫기
                Task.Delay(3000).ContinueWith(_ =>
                {
                    Dispatcher.Invoke(() => popup.Close());
                });
            });
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (textBox.Text == "Enter Nickname" || textBox.Text == "Enter Room Name")
                {
                    textBox.Text = string.Empty;
                    textBox.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    if (textBox.Name == "NicknameBox")
                        textBox.Text = "Enter Nickname";
                    else if (textBox.Name == "NewRoomBox")
                        textBox.Text = "Enter Room Name";

                    textBox.Foreground = new SolidColorBrush(Colors.Gray);
                }
            }
        }

        private void ConnectToServerButton_Click(object sender, RoutedEventArgs e)
        {
            _nickname = NicknameBox.Text.Trim();

            if (string.IsNullOrEmpty(_nickname))
            {
                LogMessage("닉네임을 입력하세요.");
                return;
            }

            if (_nickname.Any(c => !char.IsLetterOrDigit(c)))
            {
                LogMessage("닉네임은 공백이나 특수문자를 포함할 수 없습니다.");
                return;
            }

            ConnectToServer();
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => DisconnectFromServer());
        }

        private void ConnectToServer()
        {
            try
            {
                _client = new TcpClient("127.0.0.1", 8080);
                _stream = _client.GetStream();
                _isConnected = true;

                _receiveThread = new Thread(ReceiveMessages)
                {
                    IsBackground = true
                };
                _receiveThread.Start();

                SendMessage($"NICK:{_nickname}");
                LogMessage($"서버에 연결되었습니다. 닉네임: {_nickname}");

                UpdateUIState(connected: true);
            }
            catch (Exception ex)
            {
                LogMessage($"서버 연결 실패: {ex.Message}");
            }
        }

        private void DisconnectFromServer()
        {
            try
            {
                // 연결 종료 신호 설정
                _isConnected = false;

                // 네트워크 리소스 해제
                _stream?.Close();
                _client?.Close();

                // 수신 스레드 안전 종료
                if (_receiveThread != null && _receiveThread.IsAlive)
                {
                    _receiveThread.Join(500); // 최대 500ms 대기 후 반환
                }

                LogMessage("서버 연결이 종료되었습니다.");
                UpdateUIState(connected: false);
            }
            catch (Exception ex)
            {
                LogMessage($"서버 연결 종료 중 오류: {ex.Message}");
            }
        }

        private void UpdateRoomList(string roomList)
        {
            RoomListBox.Items.Clear();
            foreach (var room in roomList.Split('|'))
            {
                if (!string.IsNullOrWhiteSpace(room))
                {
                    RoomListBox.Items.Add(room);
                }
            }
        }

        private void CreateRoomButton_Click(object sender, RoutedEventArgs e)
        {
            string newRoomName = NewRoomBox.Text.Trim();
            if (string.IsNullOrEmpty(newRoomName))
            {
                LogMessage("방 이름을 입력하세요.");
                return;
            }

            if (newRoomName.Contains(" ") || newRoomName.Any(c => !char.IsLetterOrDigit(c)))
            {
                LogMessage("방 이름은 공백이나 특수문자를 포함할 수 없습니다.");
                return;
            }

            SendMessage($"CREATE_ROOM:{newRoomName}");
            NewRoomBox.Clear();
            LogMessage($"방 생성 요청: {newRoomName}");
        }

        private void RoomListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RoomListBox.SelectedItem != null)
            {
                string roomName = RoomListBox.SelectedItem.ToString();
                SendMessage($"JOIN_ROOM:{roomName}");
                _currentRoom = roomName;

                MessageBox.IsEnabled = true;
                SendButton.IsEnabled = true;

                LogMessage($"'{roomName}' 방에 입장했습니다.");
            }
        }

        private void SendMessage(string message)
        {
            if (_isConnected)
            {
                try
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    _stream.Write(buffer, 0, buffer.Length);
                }
                catch (Exception ex)
                {
                    LogMessage($"메시지 전송 실패: {ex.Message}");
                }
            }
        }

        private void LogMessage(string message)
        {
            ChatBox.AppendText($"{message}\n");
            ChatBox.ScrollToEnd();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageBox.Text.Trim();
            if (!string.IsNullOrEmpty(message) && _currentRoom != null)
            {
                SendMessage($"MSG:{_currentRoom}:{message}");
                MessageBox.Clear();
            }
        }

        private void UpdateUIState(bool connected)
        {
            ConnectToServerButton.IsEnabled = !connected;
            DisconnectButton.IsEnabled = connected;
            CreateRoomButton.IsEnabled = connected;
            NewRoomBox.IsEnabled = connected;
            RoomListBox.IsEnabled = connected;
            NicknameBox.IsEnabled = !connected;
            MessageBox.IsEnabled = false;
            SendButton.IsEnabled = false;
        }
    }
}
