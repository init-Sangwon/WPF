using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace chatServer
{
    public partial class MainWindow : Window
    {
        private TcpListener _server;
        private bool _isRunning;
        private Thread _listenerThread;
        private List<TcpClient> _clients = new List<TcpClient>();
        private Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();
        private Dictionary<TcpClient, string> _clientNicknames = new Dictionary<TcpClient, string>();

        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (_server == null)
            {
                _server = new TcpListener(IPAddress.Any, 8080);
            }

            try
            {
                _server.Start();
                _isRunning = true;

                LogMessage("서버가 시작되었습니다.");

                _listenerThread = new Thread(ListenForClients);
                _listenerThread.Start();

                StartButton.IsEnabled = false;
                StopButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                LogMessage($"서버 시작 중 오류: {ex.Message}");
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _isRunning = false;

                foreach (var client in _clients)
                {
                    client.Close();
                }
                _clients.Clear();

                if (_server != null)
                {
                    _server.Stop();
                    _server = null;
                }

                if (_listenerThread != null && _listenerThread.IsAlive)
                {
                    _listenerThread.Join();
                    _listenerThread = null;
                }

                LogMessage("서버가 종료되었습니다.");

                StartButton.IsEnabled = true;
                StopButton.IsEnabled = false;
            }
            catch (Exception ex)
            {
                LogMessage($"서버 종료 중 오류: {ex.Message}");
            }
        }

        private void ListenForClients()
        {
            while (_isRunning)
            {
                try
                {
                    TcpClient client = _server.AcceptTcpClient();
                    _clients.Add(client);

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();

                    LogMessage("클라이언트가 연결되었습니다.");
                }
                catch (SocketException)
                {
                    // 서버가 중지될 때 발생하는 예외 무시
                }
                catch (Exception ex)
                {
                    LogMessage($"클라이언트 연결 중 오류: {ex.Message}");
                }
            }
        }

        private void HandleClient(TcpClient client)
        {
            try
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                string nickname = null;

                // 방 목록 전송
                SendRoomList(stream);

                while (_isRunning)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    if (message.StartsWith("NICK:"))
                    {
                        nickname = message.Substring(5).Trim();
                        lock (_clientNicknames)
                        {
                            _clientNicknames[client] = nickname;
                        }
                        LogMessage($"{nickname} 님이 연결되었습니다.");
                    }
                    else if (message.StartsWith("CREATE_ROOM:"))
                    {
                        string roomName = message.Substring(12).Trim();
                        CreateRoom(roomName, nickname);
                        BroadcastRoomList();
                    }
                    else if (message.StartsWith("JOIN_ROOM:"))
                    {
                        string roomName = message.Substring(10).Trim();
                        JoinRoom(roomName, nickname);
                    }
                    else if (message.StartsWith("MSG:"))
                    {
                        string[] parts = message.Split(':', 3);
                        if (parts.Length == 3)
                        {
                            string roomName = parts[1];
                            string chatMessage = parts[2];
                            BroadcastToRoom(roomName, $"{nickname}: {chatMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"클라이언트 처리 중 오류: {ex.Message}");
            }
            finally
            {
                client?.Close();
                lock (_clientNicknames)
                {
                    _clientNicknames.Remove(client);
                }
                _clients.Remove(client);
            }
        }

        private void CreateRoom(string roomName, string nickname)
        {
            lock (_rooms)
            {
                if (!_rooms.ContainsKey(roomName))
                {
                    _rooms[roomName] = new List<string> { nickname };
                    LogMessage($"방 '{roomName}'이(가) 생성되었습니다.");
                }
                else
                {
                    LogMessage($"방 '{roomName}'은 이미 존재합니다.");
                }
            }
        }

        private void JoinRoom(string roomName, string nickname)
        {
            lock (_rooms)
            {
                if (_rooms.ContainsKey(roomName))
                {
                    _rooms[roomName].Add(nickname);
                    LogMessage($"{nickname} 님이 방 '{roomName}'에 입장했습니다.");
                }
                else
                {
                    LogMessage($"방 '{roomName}'이(가) 존재하지 않습니다.");
                }
            }
        }

        private void BroadcastToRoom(string roomName, string message)
        {
            lock (_rooms)
            {
                // 방이 존재하는지 확인
                if (_rooms.ContainsKey(roomName))
                {
                    var targetClients = new HashSet<TcpClient>();

                    // 방에 속한 닉네임을 기준으로 클라이언트 필터링
                    foreach (var nickname in _rooms[roomName])
                    {
                        foreach (var kvp in _clientNicknames)
                        {
                            if (kvp.Value == nickname) // 닉네임이 일치하는 클라이언트를 찾음
                            {
                                targetClients.Add(kvp.Key); // HashSet은 중복을 자동 제거
                            }
                        }
                    }

                    // 메시지를 대상 클라이언트들에게 전송
                    foreach (var client in targetClients)
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            byte[] buffer = Encoding.UTF8.GetBytes($"[ROOM:{roomName}] {message}");
                            stream.Write(buffer, 0, buffer.Length);
                        }
                        catch (Exception ex)
                        {
                            LogMessage($"메시지 전송 실패: {ex.Message}");
                        }
                    }

                    // 서버 로그에 기록
                    LogMessage($"[To {roomName}] {message}");
                }
                else
                {
                    LogMessage($"방 '{roomName}'이(가) 존재하지 않습니다.");
                }
            }
        }

        private void SendRoomList(NetworkStream stream)
        {
            lock (_rooms)
            {
                string roomList = string.Join("|", _rooms.Keys);
                byte[] buffer = Encoding.UTF8.GetBytes($"[ROOM_LIST]{roomList}");
                stream.Write(buffer, 0, buffer.Length);
            }
        }

        private void BroadcastRoomList()
        {
            lock (_rooms)
            {
                string roomList = string.Join("|", _rooms.Keys);
                byte[] buffer = Encoding.UTF8.GetBytes($"[ROOM_LIST]{roomList}");
                foreach (var client in _clients)
                {
                    try
                    {
                        client.GetStream().Write(buffer, 0, buffer.Length);
                    }
                    catch
                    {
                        // 무시
                    }
                }
            }
        }

        private void LogMessage(string message)
        {
            Dispatcher.Invoke(() => LogBox.AppendText($"{message}\n"));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_isRunning)
            {
                StopButton_Click(null, null);
            }
        }
    }
}
