using System;
using System.ComponentModel;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfClient.ViewModel
{
    public class SignupViewModel : INotifyPropertyChanged
    {
        private string _id = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private string _name = string.Empty;

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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ICommand SignupCommand { get; }

        public SignupViewModel()
        {
            SignupCommand = new RelayCommand(async () => await Signup());
        }

        private async Task Signup()
        {
            if (string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(Name))
            {
                MessageBox.Show("모든 필드를 입력해주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("비밀번호가 일치하지 않습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 비밀번호 해싱
            string hashedPassword = HashPassword(Password);

            // 전송 데이터 구성
            string dataToSend = $"{Id}|{hashedPassword}|{Name}";

            try
            {
                using (TcpClient client = new TcpClient("127.0.0.1", 8081))
                using (NetworkStream stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes(dataToSend);
                    await stream.WriteAsync(data, 0, data.Length);

                    // 서버 응답 읽기 (옵션)
                    byte[] buffer = new byte[1024];
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string serverResponse = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    MessageBox.Show($"서버 응답: {serverResponse}", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터 전송 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 비밀번호 해싱 메서드
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash); // Base64로 변환
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
