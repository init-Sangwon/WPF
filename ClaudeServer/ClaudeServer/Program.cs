using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClaudeServer
{
    class Program
    {
        private static readonly List<TcpClient> clients = new List<TcpClient>();
        private static readonly Dictionary<TcpClient, string> clientNames = new Dictionary<TcpClient, string>();

        static async Task Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                // 서버 시작
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                server = new TcpListener(localAddr, port);
                server.Start();

                Console.WriteLine("채팅 서버가 시작되었습니다...");

                while (true)
                {
                    TcpClient client = await server.AcceptTcpClientAsync();
                    clients.Add(client);

                    // 클라이언트별로 메시지 수신 처리
                    _ = HandleClientAsync(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
            finally
            {
                server?.Stop();
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            string clientName = "";

            try
            {
                // 첫 메시지로 클라이언트 이름 받기
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                clientName = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                clientNames[client] = clientName;

                // 새 사용자 입장 알림
                string welcomeMessage = $"{clientName}님이 입장하셨습니다.";
                await BroadcastMessage(welcomeMessage, client);

                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    string fullMessage = $"{clientName}: {message}";

                    await BroadcastMessage(fullMessage, client);
                }
            }
            catch (Exception)
            {
                // 클라이언트 연결 끊김
            }
            finally
            {
                string disconnectMessage = $"{clientName}님이 퇴장하셨습니다.";
                clients.Remove(client);
                clientNames.Remove(client);
                await BroadcastMessage(disconnectMessage, null);
                client.Close();
            }
        }

        private static async Task BroadcastMessage(string message, TcpClient excludeClient)
        {
            byte[] broadcastBytes = Encoding.UTF8.GetBytes(message);

            foreach (TcpClient client in clients.ToArray())
            {
                if (client == excludeClient) continue;

                try
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(broadcastBytes, 0, broadcastBytes.Length);
                }
                catch (Exception)
                {
                    // 오류가 발생한 클라이언트는 무시
                }
            }
            Console.WriteLine(message); // 서버 콘솔에도 메시지 출력
        }
    }
}