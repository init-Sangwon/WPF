using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

namespace GeminiServer
{
    class Program
    {
        private static TcpListener server;
        private static List<TcpClient> clients = new List<TcpClient>();

        static void Main(string[] args)
        {
            try
            {
                int port = 8888; // 포트 번호
                server = new TcpListener(IPAddress.Any, port);
                server.Start();
                Console.WriteLine("GeminiServer 시작. 포트: " + port);

                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    clients.Add(client);
                    Console.WriteLine("클라이언트 연결됨.");

                    Thread clientThread = new Thread(HandleClient);
                    clientThread.Start(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("오류: " + e.Message);
            }
        }

        static void HandleClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try
            {
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine("수신: " + message);

                    // 모든 클라이언트에게 메시지 전송
                    foreach (TcpClient c in clients)
                    {
                        if (c != client && c.Connected) // 메시지 보낸 클라이언트는 제외, 연결된 클라이언트만 전송
                        {
                            NetworkStream cStream = c.GetStream();
                            cStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("클라이언트 연결 끊김.");
            }
            finally
            {
                clients.Remove(client);
                client.Close();
            }
        }
    }
}