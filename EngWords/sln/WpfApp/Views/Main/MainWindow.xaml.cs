using System.Text;
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
using MySql.Data.MySqlClient;

namespace WpfApp.Views.Main

{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void AddWordButton_Click(object sender, RoutedEventArgs e)
        {
            AddWord addWord = new AddWord();
            addWord.ShowDialog(); 
        }

        private void DeleteWordButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteWord deleteWord = new DeleteWord();
            deleteWord.ShowDialog();
        }

        private void MistakesButton_Click(object sender, RoutedEventArgs e)
        {
            Mistakes mistakesWindow = new Mistakes();
            mistakesWindow.Show();
        }

        private void ViewWordsButton_Click(object sender, RoutedEventArgs e)
        {
            ListWord listWord = new ListWord();
            listWord.ShowDialog();
        }
    }
}