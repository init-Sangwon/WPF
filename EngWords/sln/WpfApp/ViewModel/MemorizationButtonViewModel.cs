using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualBasic;
using WpfApp.ViewModel;
using WpfApp.Views;
using WpfApp.Utilities;

namespace WpfApp.ViewModels
{
    public class MemorizationButtonViewModel
    {
        public ICommand MemorizationCommand { get; }

        public MemorizationButtonViewModel()
        {
            // ICommand를 RelayCommand로 연결
            MemorizationCommand = new RelayCommand(ExecuteMemorizationCommand);
        }

        private void ExecuteMemorizationCommand(object parameter)
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
