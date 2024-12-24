using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfClient.View;

namespace WpfClient.ViewModel
{
    public class SignupViewModel
    {
        public string Id { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }

        public ICommand SignupCommand { get; }

        public SignupViewModel()
        {
            SignupCommand = new RelayCommand(async () => await Signup());
        }

        private async Task Signup()
        {
            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("모든 필드를 채워주세요.");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.");
                return;
            }

            string hashedPassword = HashPassword(Password);
            string dataToSend = $"SIGNUP|{Id}|{hashedPassword}|{Name}";

            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 8081))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(dataToSend);
                    await stream.WriteAsync(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (response.Contains("SUCCESS"))
                    {
                        MessageBox.Show("회원가입 성공! 로그인 페이지로 이동합니다.");

                        // 로그인 페이지로 이동
                        var loginPage = new Login(); // 로그인 창 생성
                        loginPage.Show();
                        Application.Current.MainWindow.Close(); // 현재 창 닫기
                        Application.Current.MainWindow = loginPage; // 새 창 설정
                    }
                    else
                    {
                        MessageBox.Show(response); // 서버 실패 응답 처리
                    }
                }
            }
            catch
            {
                MessageBox.Show("서버 연결 실패");
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            return Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
        }
    }
}
