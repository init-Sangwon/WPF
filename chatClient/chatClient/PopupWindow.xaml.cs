using System.Windows;

namespace chatClient
{
    public partial class PopupWindow : Window
    {
        public PopupWindow(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }
    }
}