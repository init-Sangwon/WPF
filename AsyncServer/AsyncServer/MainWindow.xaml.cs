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
        private CancellationTokenSource cts;
        private List<Task> clientTasks = new List<Task>();



        public MainWindow()
        {
            InitializeComponent();
            Console.WriteLine($"Main 스레드 ID: {Thread.CurrentThread.ManagedThreadId}");
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {


            if (AsyncServer == null)
            {
                AsyncServer = new TcpListener(IPAddress.Any, 8081);
                AsyncServer.Start();
            }
            else { Console.WriteLine("서버가 이미 실행중입니다."); }

            cts = new CancellationTokenSource();
            Log.Text = "서버 실행중";
            Console.WriteLine("서버실행중");

            try
            {
                await AcceptClientsAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("서버가 중단되었습니다.");
            }
            finally
            {
                StopServer();
            }
        }

        private async Task AcceptClientsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"[AcceptClientsAsync] 스레드 ID: {Thread.CurrentThread.ManagedThreadId}");
                try
                {
                    // CancellationToken을 사용한 작업 종료 감지
                    var acceptTask = AsyncServer.AcceptTcpClientAsync();
                    var completedTask = await Task.WhenAny(acceptTask, Task.Delay(Timeout.Infinite, token));

                    if (completedTask == acceptTask)
                    {
                        TcpClient client = await acceptTask;
                        Console.WriteLine("클라이언트 연결 성공");

                        var clientTask = Task.Run(() => MssageHandleAsync(client, token), token);
                        clientTasks.Add(clientTask);

                        // 완료된 작업 제거
                        _ = clientTask.ContinueWith(t => clientTasks.Remove(clientTask), TaskScheduler.Default);
                    }
                    else
                    {
                        // CancellationToken이 취소됨
                        throw new OperationCanceledException();
                    }
                }
                catch (OperationCanceledException)
                {
                    // 서버가 중단되어 더 이상 클라이언트를 수락하지 않도록 합니다.
                    Console.WriteLine("AcceptTcpClientAsync 작업 취소됨.");
                    break;
                }
                catch (SocketException ex) when (ex.SocketErrorCode == SocketError.Interrupted)
                {
                    Console.WriteLine("서버 중단 중...");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"예외 발생: {ex.Message}");
                }
            }
        }

        private async void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (AsyncServer != null)
            {
                Console.WriteLine("서버 중지 중...");
                cts.Cancel(); // 먼저 취소를 요청합니다.

                await Task.WhenAll(clientTasks); // 클라이언트 관련 작업들이 완료될 때까지 대기

                StopServer(); // 서버 정리
                Console.WriteLine("서버가 중지되었습니다.");
                Log.Text = "서버가 중지되었습니다.";
            }
            else
            {
                Console.WriteLine("서버가 이미 중단된 상태입니다.");
            }
        }

        private async Task MssageHandleAsync(TcpClient client, CancellationToken token)
        {
            Console.WriteLine($"[MssageHandleAsync - Start] 스레드 ID: {Thread.CurrentThread.ManagedThreadId}");
            NetworkStream stream = client.GetStream();
            var pool = System.Buffers.ArrayPool<byte>.Shared;

            // 서버 중지 요청이 있을 경우 즉시 종료
            if (token.IsCancellationRequested)
            {
                Console.WriteLine("서버 중지로 인해 작업이 취소되었습니다.");
                return; // 바로 종료
            }

            while (!token.IsCancellationRequested) // 매 반복마다 확인
            {
                Console.WriteLine($"[MssageHandleAsync - While Loop] 스레드 ID: {Thread.CurrentThread.ManagedThreadId}");
                byte[] lengthBuffer = pool.Rent(4);
                byte[] messageBuffer = null;

                try
                {
                    // 서버 중지 요청이 있을 경우 즉시 종료
                    if (token.IsCancellationRequested)
                    {
                        Console.WriteLine("서버 중지로 인해 작업이 취소되었습니다.");
                        break;
                    }

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
                        if (token.IsCancellationRequested)
                        {
                            Console.WriteLine("서버 중지로 인해 작업이 취소되었습니다.");
                            break;
                        }

                        bytesRead = await stream.ReadAsync(messageBuffer, totalBytesRead, messageLength - totalBytesRead);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("클라이언트 연결 종료 중단");
                            await UpdateLogAsync("클라이언트 연결 종료 중단");
                            break;
                        }
                        totalBytesRead += bytesRead;
                    }

                    if (!token.IsCancellationRequested)
                    {
                        string message = Encoding.UTF8.GetString(messageBuffer, 0, totalBytesRead);
                        Console.WriteLine($"클라이언트 메시지: {message}");
                        await UpdateLogAsync($"클라이언트 메시지: {message}");

                        // 응답 보내기
                        string response = "서버가 메시지를 처리했습니다.";
                        byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                        await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                    }
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
        private void StopServer()
        {
            AsyncServer?.Stop();
            AsyncServer = null;
            cts?.Dispose();
            cts = null;
            clientTasks.Clear();
        }

    }
}
