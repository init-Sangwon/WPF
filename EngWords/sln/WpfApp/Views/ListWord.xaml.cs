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
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using WpfApp.Model;
using System.IO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
namespace WpfApp.Views
{
    /// <summary>
    /// ListWord.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ListWord : Window
    {
        private MainModel _selectedWord;
        private List<MainModel> allWords; // 모든 단어
        private int currentPage = 1;      // 현재 페이지
        private int itemsPerPage = 10;    // 페이지당 항목 수
        private readonly string connectionString = "Server=localhost;Port=3306;Database=engword;Uid=root;Pwd=root;";

        public ListWord()
        {
            InitializeComponent();
            LoadWords();
            DisplayPage();
        }
        private void LoadWords()
        {
            allWords = new List<MainModel>();

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Eng, Kor FROM words";

                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            allWords.Add(new MainModel
                            {
                                Id = reader.GetInt32("Id"),
                                Eng = reader.IsDBNull(reader.GetOrdinal("Eng")) ? "N/A" : reader.GetString("Eng"),
                                Kor = reader.IsDBNull(reader.GetOrdinal("Kor")) ? "N/A" : reader.GetString("Kor")
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void DisplayPage()
        {
            var pagedData = allWords
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            WordsListView.ItemsSource = pagedData;
            PageInfoText.Text = $"페이지 {currentPage} / {GetTotalPages()}";
        }

        private int GetTotalPages()
        {
            return (allWords.Count + itemsPerPage - 1) / itemsPerPage;
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                DisplayPage();
            }
        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage < GetTotalPages())
            {
                currentPage++;
                DisplayPage();
            }
        }
        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "엑셀 파일 저장",
                FileName = "단어장.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // ClosedXML을 사용하여 엑셀 파일 생성
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("단어장");

                        // 헤더 추가
                        worksheet.Cell(1, 1).Value = "영어 단어";
                        worksheet.Cell(1, 2).Value = "한글 뜻";

                        // 데이터 추가
                        for (int i = 0; i < allWords.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = allWords[i].Eng; // 영어 단어
                            worksheet.Cell(i + 2, 2).Value = allWords[i].Kor; // 한글 뜻
                        }

                        // 컬럼 너비 자동 조정
                        worksheet.Columns().AdjustToContents();

                        // 파일 저장
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("엑셀 파일이 성공적으로 저장되었습니다.", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void WordsListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WordsListView.SelectedItem is MainModel selectedWord)
            {
                _selectedWord = selectedWord; // 선택한 단어 저장

                // 컨텍스트 메뉴 표시
                var menu = (ContextMenu)WordsListView.Resources["EditDeleteMenu"];
                menu.PlacementTarget = sender as ListView;
                menu.IsOpen = true;
            }
        }

        private void EditMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWord != null)
            {
                var editWindow = new EditWord(_selectedWord.Eng, _selectedWord.Kor)
                {
                    Owner = this
                };

                if (editWindow.ShowDialog() == true)
                {
                    // 수정된 값 저장
                    _selectedWord.Eng = editWindow.Eng;
                    _selectedWord.Kor = editWindow.Kor;
                    UpdateWordInDatabase(_selectedWord);
                    DisplayPage();
                }
            }
        }
        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedWord != null)
            {
                var result = MessageBox.Show("정말 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteWordFromDatabase(_selectedWord.Id);
                    allWords.Remove(_selectedWord);
                    DisplayPage();
                }
            }
        }
        private void DeleteWordFromDatabase(int wordId)
        {
            string query = "DELETE FROM words WHERE Id = @Id";
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", wordId);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("단어가 삭제되었습니다.", "삭제 완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"단어 삭제 중 오류가 발생했습니다: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateWordInDatabase(MainModel word)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE words SET Eng = @Eng, Kor = @Kor WHERE Id = @Id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Eng", word.Eng);
                        cmd.Parameters.AddWithValue("@Kor", word.Kor);
                        cmd.Parameters.AddWithValue("@Id", word.Id);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터베이스 업데이트 중 오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
