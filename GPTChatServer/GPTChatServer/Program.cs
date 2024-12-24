using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GPTChatServer
{
    class Program
    {
        private static TcpListener listener;
        private static List<TcpClient> clients = new List<TcpClient>();

        static void Main(string[] args)
        {
            listener = new TcpListener(IPAddress.Any, 8081);
            listener.Start();
            Console.WriteLine("GPTChatServer가 시작되었습니다...");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                clients.Add(client);
                Console.WriteLine("클라이언트 연결됨!");

                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }

        private static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            try
            {
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int byteCount = stream.Read(buffer, 0, buffer.Length);
                    if (byteCount == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine($"수신된 메시지: {message}");
                    BroadcastMessage(message, client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"오류: {ex.Message}");
            }
            finally
            {
                clients.Remove(client);
                client.Close();
            }
        }

        private static void BroadcastMessage(string message, TcpClient sender)
        {
            foreach (var client in clients)
            {
                if (client != sender)
                {
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}
