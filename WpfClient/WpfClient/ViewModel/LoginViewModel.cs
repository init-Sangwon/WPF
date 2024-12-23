using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfClient.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        private string _id = string.Empty;
        private string _password = string.Empty;

        public string Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(async () => await Login());
        }

        private async Task Login()
        {
            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("ID와 비밀번호를 입력해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string hashedPassword = HashPassword(Password);
            string dataToSend = $"LOGIN|{Id}|{hashedPassword}";

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

                    if (response == "로그인 성공")
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var mainWindow = new View.MainWindow();
                            mainWindow.Show();
                            Application.Current.Windows[0]?.Close();
                        });
                    }
                    else
                    {
                        MessageBox.Show(response, "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"서버와의 연결 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string HashPassword(string password)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
