using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace ChatServer
{
    public class ChatMessage
    {
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
    }

    public class Server
    {
        private TcpListener _server;
        private Dictionary<int, TcpClient> _clients;
        private string _connectionString;

        public Server(string connectionString)
        {
            _connectionString = connectionString;
            _clients = new Dictionary<int, TcpClient>();
        }

        public async Task StartServer()
        {
            _server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
            _server.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = await _server.AcceptTcpClientAsync();
                _ = HandleClientAsync(client);
            }
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            while (true)
            {
                try
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    ChatMessage chatMessage = JsonConvert.DeserializeObject<ChatMessage>(message);

                    // Save message to database
                    await SaveMessageToDatabase(chatMessage);

                    // Broadcast message to all clients
                    await BroadcastMessage(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    break;
                }
            }

            client.Close();
        }

        private async Task SaveMessageToDatabase(ChatMessage message)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                string query = "INSERT INTO Messages (SenderID, Content, SentAt) VALUES (@SenderId, @Content, @SentAt)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SenderId", message.SenderId);
                    command.Parameters.AddWithValue("@Content", message.Content);
                    command.Parameters.AddWithValue("@SentAt", message.SentAt);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task BroadcastMessage(string message)
        {
            byte[] broadcastBuffer = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients.Values)
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    await stream.WriteAsync(broadcastBuffer, 0, broadcastBuffer.Length);
                }
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = "Server=localhost;Port=3306;Database=chatapp;User=root;Password=root;";
            Server server = new Server(connectionString);
            await server.StartServer();
        }
    }
}