using System.Windows;
using Chess.ViewModels;

namespace Chess.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ChessBoardViewModel(); // 뷰모델 설정
        }
    }
}
