using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfClient.ViewModel
{
    class LoginViewModel
    {
        public ICommand ConnectCommand { get; set; }

        public LoginViewModel()
        {
            ConnectCommand = new RelayCommand(async () => await ConnectToServerAsync("127.0.0.1", 8081));
        }
    
    private async Task ConnectToServerAsync(string ipAddress, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    await client.ConnectAsync(ipAddress, port);

                    // 연결 성공 시 MainWindow로 전환
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var mainWindow = new View.MainWindow();
                        mainWindow.Show();
                        Application.Current.MainWindow?.Close();
                        Application.Current.MainWindow = mainWindow;
                    });
                }
            }
            catch (Exception)
            {
                MessageBox.Show("서버 연결 실패", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}



