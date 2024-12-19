using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MySql.Data.MySqlClient;
using WpfApp.Model;
using WpfApp.Utilities;

namespace WpfApp.ViewModel
{
    internal class MainViewModel : INotifyCollectionChanged
    {
        private readonly string connectionString = "Server=localhost;Port=3306;Database=engword;Uid=root;Pwd=root;";

        public ObservableCollection<MainModel> Words { get; set; }

        public ICommand LoadDataCommand { get; set; }

        public MainViewModel()
        {
            Words = new ObservableCollection<MainModel>();
            LoadDataCommand = new RelayCommand(LoadData);
        }
        private void LoadData()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM words";
                    MySqlCommand cmd = new MySqlCommand(query, connection);
                    MySqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Words.Add(new MainModel
                        {
                            Id = reader.GetInt32("Id"),
                            Eng = reader.GetString("Eng"),
                            Kor = reader.GetString("Kor")
                        });
                    }
                }
                catch (MySqlException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Database Error: " + ex.Message);
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
