using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfClient.ViewModel;

public class SignupViewModel
{
    public string Id { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }

    public ICommand SignupCommand { get; }

    public SignupViewModel()
    {
        SignupCommand = new RelayCommand(async () => await Signup());
    }

    private async Task Signup()
    {
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

                MessageBox.Show(response);
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
