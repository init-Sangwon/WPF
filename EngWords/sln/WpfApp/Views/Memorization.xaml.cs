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
using MySql.Data.MySqlClient;

namespace WpfApp.Views
{
    /// <summary>
    /// Memorization.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Memorization : Window
    {
        private int currentWordId; // 현재 단어의 ID를 저장
        private readonly string connectionString = "Server=localhost;Port=3306;Database=engword;Uid=root;Pwd=root;";
        private string correctMeaning = "";
        private int totalWordsToLearn; // 학습할 단어 개수
        private int currentWordIndex = 0; // 현재 단어 진행 상태

        public Memorization(int wordsToLearn)
        {
            InitializeComponent();
            totalWordsToLearn = wordsToLearn; // 단어 개수 저장
            LoadWordAndMeanings(); // 첫 단어 로드
        }

        private void LoadWordAndMeanings()
        {
            if (currentWordIndex >= totalWordsToLearn)
            {
                MessageBox.Show("학습을 완료했습니다!", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // 학습 완료 시 창 닫기
                return;
            }

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 영어 단어 하나 가져오기
                    string wordQuery = "SELECT Id, Eng, Kor FROM words ORDER BY RAND() LIMIT 1";
                    using (var wordCmd = new MySqlCommand(wordQuery, connection))
                    using (var wordReader = wordCmd.ExecuteReader())
                    {
                        if (wordReader.Read())
                        {
                            currentWordId = wordReader.GetInt32("Id");
                            string englishWord = wordReader.GetString("Eng");
                            correctMeaning = wordReader.GetString("Kor");

                            WordTextBlock.Text = englishWord; // 영어 단어 표시
                        }
                    }

                    // 뜻 목록 가져오기
                    List<string> meanings = new List<string> { correctMeaning };
                    string meaningsQuery = "SELECT Kor FROM words WHERE Kor != @CorrectMeaning ORDER BY RAND() LIMIT 4";

                    using (var meaningsCmd = new MySqlCommand(meaningsQuery, connection))
                    {
                        meaningsCmd.Parameters.AddWithValue("@CorrectMeaning", correctMeaning);
                        using (var meaningsReader = meaningsCmd.ExecuteReader())
                        {
                            while (meaningsReader.Read())
                            {
                                meanings.Add(meaningsReader.GetString("Kor"));
                            }
                        }
                    }

                    // 뜻 리스트 섞기
                    meanings = meanings.OrderBy(x => Guid.NewGuid()).ToList();
                    MeaningListBox.ItemsSource = meanings; // ListBox에 데이터 바인딩
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"데이터베이스 오류: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItems = MeaningListBox.SelectedItems.Cast<string>().ToList();

            if (selectedItems.Contains(correctMeaning))
            {
                MessageBox.Show("정답입니다!", "결과", MessageBoxButton.OK, MessageBoxImage.Information);
                RemoveFromMistakes(currentWordId);
            }
            else
            {
                SaveMistakeToDatabase(currentWordId);
                MessageBox.Show($"틀렸습니다. 정답은 '{correctMeaning}' 입니다.", "결과", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            currentWordIndex++; // 다음 단어로 넘어가기
            LoadWordAndMeanings(); // 다음 단어 로드
        }

        private void RemoveFromMistakes(int wordId)
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM mistakes WHERE WordId = @WordId";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@WordId", wordId);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("오답 제거 실패: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void SaveMistakeToDatabase(int currentWordId)
        {
            string connectionString = "Server=localhost;Database=engword;Uid=root;Pwd=root;";

            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // 중복 확인 쿼리
                    string checkQuery = "SELECT COUNT(*) FROM mistakes WHERE WordId = @WordId";
                    using (var checkCmd = new MySqlCommand(checkQuery, connection))
                    {
                        checkCmd.Parameters.AddWithValue("@WordId", currentWordId);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            return; // 중복이므로 저장하지 않음
                        }
                    }

                    // 누락된 ID 찾기
                    string findMissingIdQuery = @"
                SELECT MIN(t1.id + 1) AS nextId
                FROM mistakes t1
                LEFT JOIN mistakes t2 ON t1.id + 1 = t2.id
                WHERE t2.id IS NULL;";

                    int nextId;
                    using (var findCmd = new MySqlCommand(findMissingIdQuery, connection))
                    {
                        var result = findCmd.ExecuteScalar();
                        nextId = result != DBNull.Value ? Convert.ToInt32(result) : 1; // 없으면 1부터 시작
                    }

                    // 누락된 ID에 삽입
                    string insertQuery = "INSERT INTO mistakes (id, WordId) VALUES (@id, @WordId)";
                    using (var insertCmd = new MySqlCommand(insertQuery, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@id", nextId);
                        insertCmd.Parameters.AddWithValue("@WordId", currentWordId);

                        insertCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}