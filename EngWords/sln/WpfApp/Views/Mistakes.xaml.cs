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
using ClosedXML.Excel;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using WpfApp.Model;

namespace WpfApp.Views
{
    /// <summary>
    /// Mistakes.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Mistakes : Window
    {
        private List<MainModel> allWords; // 전체 단어 목록
        private int currentPage = 1;      // 현재 페이지 번호
        private int itemsPerPage = 10;    // 페이지당 아이템 수

        private readonly string connectionString = "Server=localhost;Port=3306;Database=engword;Uid=root;Pwd=root;";


        public Mistakes()
        {
            InitializeComponent();
            LoadMistakes();
            DisplayPage();

        }

        private int GetNextId()
        {
            int nextId = 1; // 기본값 1로 초기화

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT MIN(t.Id + 1) AS NextId
                FROM (
                    SELECT Id FROM words
                    UNION ALL
                    SELECT 0 AS Id
                ) t
                WHERE t.Id + 1 NOT IN (SELECT Id FROM words);";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        var result = cmd.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            nextId = Convert.ToInt32(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return nextId;
        }

        private void LoadMistakes()
        {
            try
            {
                allWords = new List<MainModel>(); // 전체 단어 리스트 초기화

                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                SELECT w.Id, w.Eng, w.Kor 
                FROM mistakes m
                INNER JOIN words w ON m.WordId = w.Id";

                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                allWords.Add(new MainModel
                                {
                                    Id = reader.GetInt32("Id"),
                                    Eng = reader.GetString("Eng"),
                                    Kor = reader.GetString("Kor")
                                });
                            }
                        }
                    }
                }

                DisplayPage(); // 페이지 표시
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            if (allWords == null || allWords.Count == 0)
            {
                MessageBox.Show("내보낼 데이터가 없습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // SaveFileDialog를 사용해 파일 저장 위치 지정
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                Title = "엑셀 파일 저장",
                FileName = "오답노트.xlsx"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // ClosedXML의 XLWorkbook 생성
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Mistakes");

                        // 엑셀 헤더 작성
                        worksheet.Cell(1, 1).Value = "영어 단어";
                        worksheet.Cell(1, 2).Value = "한글 뜻";

                        // 데이터 채우기
                        for (int i = 0; i < allWords.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = allWords[i].Eng;
                            worksheet.Cell(i + 2, 2).Value = allWords[i].Kor;
                        }

                        // 열 너비 자동 조정
                        worksheet.Columns().AdjustToContents();

                        // 파일 저장
                        workbook.SaveAs(saveFileDialog.FileName);
                    }

                    MessageBox.Show("엑셀 파일로 내보내기가 완료되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DisplayPage()
        {
            // 현재 페이지의 데이터를 가져와 ListView에 바인딩
            var pagedData = allWords
                .Skip((currentPage - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToList();

            MistakesListView.ItemsSource = pagedData;

            // 페이지 정보 업데이트
            PageInfoText.Text = $"  페이지 {currentPage} / {GetTotalPages()}";
        }

        private int GetTotalPages()
        {
            return (allWords.Count + itemsPerPage - 1) / itemsPerPage; // 전체 페이지 수
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
    }
}

