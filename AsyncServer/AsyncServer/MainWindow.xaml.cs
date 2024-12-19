using System.Diagnostics.Eventing.Reader;
using System.Net;
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

namespace AsyncServer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpListener AsyncServer;
        private bool isRunning;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            AsyncServer = new TcpListener(IPAddress.Any, 8081);
            AsyncServer.Start();

            Log.Text = "서버 실행중";
            Console.WriteLine("서버실행중");

            while (true)
            {
                TcpClient client = await AsyncServer.AcceptTcpClientAsync();
                Console.WriteLine("클라이언트 연결 성공");

                _ = Task.Run(() => MssageHandleAsync(client));
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            AsyncServer.Stop();
            Console.WriteLine("서버중단");
        }

        private async Task MssageHandleAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            var pool = System.Buffers.ArrayPool<byte>.Shared;

            while (true)
            {
                byte[] lengthBuffer = pool.Rent(4);
                byte[] messageBuffer = null;

                try
                {
                    // 메시지 길이 읽기
                    int bytesRead = await stream.ReadAsync(lengthBuffer, 0, 4);
                    if (bytesRead == 0)
                    {
                        // 클라이언트가 연결을 끊은 경우
                        Console.WriteLine("클라이언트 연결 종료");
                        await UpdateLogAsync("클라이언트 연결 종료");
                        break;
                    }

                    int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

                    if (messageLength > 1024)
                    {
                        messageBuffer = pool.Rent(messageLength);
                    }
                    else
                    {
                        messageBuffer = new byte[messageLength];
                    }

                    // 메시지 데이터 읽기
                    int totalBytesRead = 0;
                    while (totalBytesRead < messageLength)
                    {
                        bytesRead = await stream.ReadAsync(messageBuffer, totalBytesRead, messageLength - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("클라이언트 연결 종료 중단");
                            await UpdateLogAsync("클라이언트 연결 종료 중단");
                            break;
                        }
                        totalBytesRead += bytesRead;
                    }

                    string message = Encoding.UTF8.GetString(messageBuffer, 0, totalBytesRead);
                    Console.WriteLine($"클라이언트 메시지: {message}");
                    await UpdateLogAsync($"클라이언트 메시지: {message}");

                    // 응답 보내기
                    string response = "서버가 메시지를 처리했습니다.";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"에러 발생: {ex.Message}");
                    await UpdateLogAsync($"에러 발생: {ex.Message}");
                    break;
                }
                finally
                {
                    pool.Return(lengthBuffer);
                    if (messageBuffer != null && messageBuffer.Length > 1024)
                    {
                        pool.Return(messageBuffer);
                    }
                }
            }

            client.Close();
        }
        private Task UpdateLogAsync(string message)
        {
            return Dispatcher.InvokeAsync(() =>
            {
                Log.Text = message;
            }).Task;
        }
    }
}
