using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace ClaudeServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            TcpListener listener = null;


            try
            {
                listener = new TcpListener(IPAddress.Any, 8080);
                listener.Start();

                while (true)
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    _ = ConnectSuccess(client);

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                listener?.Stop();
            }
        }
        private static async Task ConnectSuccess(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            string clientName = "알 수 없음";

            try
            {
                // 클라이언트 이름 읽기 (최초 한 번)
                byte[] buffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                Console.WriteLine($"클라이언트 연결됨: {clientName}");

                // 환영 메시지 전송
                string connectMessage = $"{clientName} 님 환영합니다.";
                byte[] messageBytes = Encoding.UTF8.GetBytes(connectMessage);
                await stream.WriteAsync(messageBytes, 0, messageBytes.Length);

                // 메시지 처리 시작
                await MessageHandle(client, clientName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ConnectSuccess 오류: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }

        private static async Task MessageHandle(TcpClient client, string clientName)
        {
            NetworkStream stream = client.GetStream();

            try
            {
                while (true) // 메시지 수신 루프
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0) // 클라이언트 연결 종료
                    {
                        Console.WriteLine($"{clientName} 연결 종료");
                        break;
                    }

                    string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();

                    Console.WriteLine($"{clientName}: {receivedMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MessageHandle 오류: {ex.Message}");
            }
            finally
            {
                client.Close();
            }
        }
    }
}