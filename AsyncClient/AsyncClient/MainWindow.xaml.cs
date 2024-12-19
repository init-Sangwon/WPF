using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AsyncClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TcpClient AsyncClient;
        private NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ConnectButton(object sender, RoutedEventArgs e)
        {
            AsyncClient = new TcpClient();
            

            try
            {
                await AsyncClient.ConnectAsync("127.0.0.1" , 8081);
                stream = AsyncClient.GetStream();

                Log.Text = "연결 성공";
            }
            catch (SocketException ex)
            {
                Log.Text =  ex + "연결실패";
            }


        }

        private void DisConnectButton(object sender, RoutedEventArgs e)
        {
            AsyncClient.Close();

            if (AsyncClient.Connected != true)
            {
                Log.Text = "서버와 연결이 끊어졌습니다";
            }
        }

        private async void SendButton(object sender, RoutedEventArgs e)
        {
            string message = chat.Text.Trim();
            chat.Text = string.Empty;
            var pool = System.Buffers.ArrayPool<Byte>.Shared;
            int messageLength = Encoding.UTF8.GetByteCount(message);    

            byte[] buffer;

            if (messageLength > 1024)
            {
                buffer = pool.Rent(4 + messageLength);
            }
            else
            {
                buffer = new byte[4 + messageLength];
            }

            try {
                if (stream == null)
                {
                    Log.Text = "서버가 연결이 되어있지 않음";
                    return;
                }
                BitConverter.GetBytes(messageLength).CopyTo(buffer, 0);
                Encoding.UTF8.GetBytes(message, 0, message.Length, buffer, 4);
                await stream.WriteAsync(buffer, 0, 4 + messageLength);

                byte[] responseBuffer = new byte[1024];
                int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
                string responseMessage = Encoding.UTF8.GetString(responseBuffer, 0, bytesRead);
                Log.Text = $"서버 응답: {responseMessage}";

            }
            catch (Exception ex) {
                Log.Text += ex.ToString();
            }
            finally
            {
                if (messageLength > 1024)
                {
                    pool.Return(buffer);    
                }
               
            }
            
        }
    }
}