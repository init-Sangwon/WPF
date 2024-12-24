using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Gpto1MiniServer
{
    class Program
    {
        static TcpListener listener;
        static List<TcpClient> clients = new List<TcpClient>();
        static readonly object lockObj = new object();
        static bool isRunning = true;

        static void Main(string[] args)
        {
            int port = 5000; // 원하는 포트 번호
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            Console.WriteLine($"서버가 포트 {port}에서 시작되었습니다.");

            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.Start();

            Console.WriteLine("서버를 종료하려면 'exit'을 입력하세요.");
            while (isRunning)
            {
                string input = Console.ReadLine();
                if (input.ToLower() == "exit")
                {
                    isRunning = false;
                    listener.Stop();
                    lock (lockObj)
                    {
                        foreach (var client in clients)
                        {
                            client.Close();
                        }
                        clients.Clear();
                    }
                }
            }
        }

        static void AcceptClients()
        {
            while (isRunning)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    lock (lockObj)
                    {
                        clients.Add(client);
                    }
                    Console.WriteLine("새 클라이언트가 연결되었습니다.");
                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
                catch (SocketException)
                {
                    // 서버가 중지될 때 발생할 수 있는 예외를 무시합니다.
                }
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int byteCount;

            try
            {
                while ((byteCount = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, byteCount);
                    Console.WriteLine($"받은 메시지: {message}");
                    BroadcastMessage(message, client);
                }
            }
            catch (Exception)
            {
                // 클라이언트 연결이 끊겼을 때 예외를 무시합니다.
            }
            finally
            {
                lock (lockObj)
                {
                    clients.Remove(client);
                }
                client.Close();
                Console.WriteLine("클라이언트 연결이 종료되었습니다.");
            }
        }

        static void BroadcastMessage(string message, TcpClient excludeClient)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            lock (lockObj)
            {
                foreach (var client in clients)
                {
                    if (client != excludeClient)
                    {
                        try
                        {
                            NetworkStream stream = client.GetStream();
                            stream.Write(buffer, 0, buffer.Length);
                        }
                        catch (Exception)
                        {
                            // 메시지를 보낼 수 없는 클라이언트를 무시합니다.
                        }
                    }
                }
            }
        }
    }
}
