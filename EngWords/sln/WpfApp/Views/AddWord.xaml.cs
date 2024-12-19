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
    /// AddWord.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddWord : Window
    {
        private readonly string connectionString = "Server=localhost;Port=3306;Database=engword;Uid=root;Pwd=root;";

        public AddWord()
        {

            InitializeComponent();
        }

        private int GetNextAvailableId()
        {
            int nextId = 1; // 기본값은 1
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
            return nextId;
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string eng = EngTextBox.Text;
            string kor = KorTextBox.Text;

            if (string.IsNullOrWhiteSpace(eng) || string.IsNullOrWhiteSpace(kor))
            {
                MessageBox.Show("영어 단어와 한글 뜻을 모두 입력하세요.", "입력 오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                int newId = GetNextAvailableId(); // 새 ID를 구함

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO words (Id, Eng, Kor, IsSelected) VALUES (@Id, @Eng, @Kor, 0)";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", newId);
                        cmd.Parameters.AddWithValue("@Eng", eng);
                        cmd.Parameters.AddWithValue("@Kor", kor);
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("단어가 성공적으로 추가되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("데이터베이스 오류: " + ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
