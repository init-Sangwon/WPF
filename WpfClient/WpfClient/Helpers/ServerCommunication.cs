using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient
{
    public static class ServerCommunication
    {
        private const string ServerAddress = "127.0.0.1";
        private const int ServerPort = 8081;

        public static async Task<string> CreateRoomAsync(string roomName)
        {
            return await SendRequestAsync($"CREATE_ROOM|{roomName}");
        }

        public static async Task<List<string>> GetChatRoomsAsync()
        {
            string response = await SendRequestAsync("GET_ROOMS");
            return new List<string>(response.Split('\n', StringSplitOptions.RemoveEmptyEntries));
        }

        public static async Task SendMessageAsync(string message)
        {
            await SendRequestAsync($"MESSAGE|{message}");
        }

        private static async Task<string> SendRequestAsync(string request)
        {
            using var client = new TcpClient(ServerAddress, ServerPort);
            using var stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(request);
            await stream.WriteAsync(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
    }
}
