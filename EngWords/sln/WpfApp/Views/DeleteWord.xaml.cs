using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp.Model;

namespace WpfApp.Views
{
    /// <summary>
    /// DeleteWord.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DeleteWord : Window
    {
        private ObservableCollection<MainModel> Words { get; set; } = new ObservableCollection<MainModel>();
        private readonly string connectionString = "Server=localhost;Port=3306;Database=engword;Uid=root;Pwd=root;";


        public DeleteWord()
        {
            InitializeComponent();
            WordsListBox.ItemsSource = Words; 
            LoadWords();

        }

        // 데이터베이스에서 단어 불러오기
        private void LoadWords()
        {
            try
            {
                Words.Clear();
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Id, Eng, Kor FROM words";
                    using (var cmd = new MySqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Words.Add(new MainModel
                            {
                                Id = reader.GetInt32("Id"),
                                Eng = reader.GetString("Eng"),
                                Kor = reader.GetString("Kor"),
                                IsSelected = false
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"오류 발생: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSelectedWords_Click(object sender, RoutedEventArgs e)
        {
            var selectedWords = Words.Where(w => w.IsSelected).ToList(); 

            if (selectedWords.Count == 0)
            {
                MessageBox.Show("삭제할 단어를 선택해 주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                foreach (var word in selectedWords)
                {
                    string query = "DELETE FROM words WHERE Id = @Id";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Id", word.Id);
                        cmd.ExecuteNonQuery();
                    }
                    Words.Remove(word); 
                }
            }
            MessageBox.Show("선택된 단어가 삭제되었습니다.", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
