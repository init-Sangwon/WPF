using MySql.Data.MySqlClient;
using System;
using System.Threading.Tasks;

namespace TaskServer
{
    internal class UserDB
    {
        private readonly string _connectionString;

        public UserDB(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task InsertUserAsync(string id, string password, string name)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                string query = "INSERT INTO users (id, password, name) VALUES (@id, @password, @name)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@password", password);
                    command.Parameters.AddWithValue("@name", name);

                    await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"사용자 정보 저장 성공: ID={id}, Name={name}");
                }
            }
        }
    }
}
