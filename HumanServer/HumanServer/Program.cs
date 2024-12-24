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
                    _ =ConnectSuccess(client);

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
            String message = "서버 연결 성공";
            byte[] messageByte = Encoding.UTF8.GetBytes(message);
            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(messageByte, 0, messageByte.Length);
        }
        private static async Task MessageHandle(TcpClient client, string message)
        {

        }
    }
}