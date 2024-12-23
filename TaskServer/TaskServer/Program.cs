using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace TaskServer
{
    class Program
    {
        private const string ConnectionString = "Server=localhost;Port=3306;Database=chatapp;User=root;Password=root;";

        static async Task Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            TcpListener listener = new TcpListener(IPAddress.Any, 8081);
            listener.Start();

            UserDB userDB = new UserDB(ConnectionString);
            RoomService roomService = new RoomService();
            ClientManager clientManager = new ClientManager();
            UserService userService = new UserService(userDB, roomService, clientManager);
            ClientConnect clientConnect = new ClientConnect(listener, userService);

            Console.WriteLine("서버가 실행 중입니다. Enter 키를 눌러 종료합니다.");
            Task serverTask = clientConnect.AcceptClientsAsync(cts.Token);
            Console.ReadLine();

            cts.Cancel();
            listener.Stop();
            await serverTask;

            Console.WriteLine("서버가 종료되었습니다.");
        }
    }
}
