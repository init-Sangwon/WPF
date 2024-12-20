using System.Net;
using System.Net.Sockets;
using TaskServer;


class Program
{
    private const string ConnectionString = "Server=localhost;Port=3306;Database=chatapp;User=root;Password=root;";


    static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        TcpListener listener = new TcpListener(IPAddress.Any, 8081);
        listener.Start();

        UserDB userDB = new UserDB(ConnectionString);
        UserService userService = new UserService(userDB);

        ClientConnect clientConnect = new ClientConnect(listener, userService);
        Task serverTask = clientConnect.AcceptClientsAsync(cts.Token);


        Console.WriteLine("Enter 키를 눌러 서버를 종료합니다.");
        Console.ReadLine();

        cts.Cancel();

        await serverTask;

        listener.Stop();

    }
}