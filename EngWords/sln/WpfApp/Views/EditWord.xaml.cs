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
using System.Windows.Shapes;

namespace WpfApp.Views
{
    /// <summary>
    /// EditWord.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditWord : Window
    {
        public string Eng { get; set; }
        public string Kor { get; set; }

        public EditWord(string eng, string kor)
        {
            InitializeComponent();
            TxtEng.Text = eng;
            TxtKor.Text = kor;
        }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            Eng = TxtEng.Text;
            Kor = TxtKor.Text;

            if (string.IsNullOrWhiteSpace(Eng) || string.IsNullOrWhiteSpace(Kor))
            {
                MessageBox.Show("빈 값은 입력할 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
