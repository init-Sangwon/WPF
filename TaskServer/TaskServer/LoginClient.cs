using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.ViewModel
{
    public class LoginClient
    {
        private readonly string _serverAddress;
        private readonly int _serverPort;

        public LoginClient(string serverAddress, int serverPort)
        {
            _serverAddress = serverAddress;
            _serverPort = serverPort;
        }

        public async Task<string> LoginAsync(string id, string password)
        {
            try
            {
                // 비밀번호 해싱
                string hashedPassword = HashPassword(password);

                // 로그인 데이터 준비
                string dataToSend = $"LOGIN|{id}|{hashedPassword}";

                // 서버와 연결
                using (TcpClient client = new TcpClient(_serverAddress, _serverPort))
                using (NetworkStream stream = client.GetStream())
                {
                    // 데이터 전송
                    byte[] data = Encoding.UTF8.GetBytes(dataToSend);
                    await stream.WriteAsync(data, 0, data.Length);

                    // 서버 응답 읽기
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"로그인 요청 중 오류 발생: {ex.Message}");
                return "서버와의 연결 실패";
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash); // Base64로 변환
            }
        }
    }
}
