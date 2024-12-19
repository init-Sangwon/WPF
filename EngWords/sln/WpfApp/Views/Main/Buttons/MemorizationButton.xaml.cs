using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic;

namespace WpfApp.Views.Main.Buttons
{
    /// <summary>
    /// MemorizationButton.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MemorizationButton : UserControl
    {
        public MemorizationButton()
        {
            InitializeComponent();
            ButtonElement.Click += MemorizationButton_Click;
        }
        private void MemorizationButton_Click(object sender, RoutedEventArgs e)
        {
            string input = Interaction.InputBox("암기할 단어 개수를 입력하세요.", "단어 개수 선택", "10", -1, -1);

            // 입력값 검증
            if (int.TryParse(input, out int wordCount) && wordCount > 0)
            {
                // 단어 개수가 유효하면 암기 화면을 열고 개수 전달
                Memorization memorizationWindow = new Memorization(wordCount);
                memorizationWindow.Show();
            }
            else
            {
                MessageBox.Show("올바른 숫자를 입력해주세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
