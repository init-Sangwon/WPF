using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TaskServer
{
    internal class ClientConnect
    {
        private readonly TcpListener _listener;
        private readonly UserService _userService;

        public ClientConnect(TcpListener listener, UserService userService)
        {
            _listener = listener;
            _userService = userService;
        }

        public async Task AcceptClientsAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    TcpClient client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine("클라이언트가 연결되었습니다.");
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("클라이언트 연결 수락이 취소되었습니다.");
            }
            finally
            {
                Console.WriteLine("TCP Listener가 종료되었습니다.");
            }
        }
        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            try
            {
                using (client)
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                    string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine($"클라이언트로부터 데이터 수신: {receivedData}");

                    // UserService에 데이터 처리 요청
                    string response = await _userService.ProcessUserSignupAsync(receivedData);

                    // 응답 전송
                    byte[] responseData = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseData, 0, responseData.Length, token);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"클라이언트 처리 중 오류 발생: {ex.Message}");
            }
        }

    }
}
    

