using MySql.Data.MySqlClient;
using System.Threading.Tasks;

namespace TaskServer
{
    public class UserDB
    {
        private readonly string _connectionString;

        public UserDB(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task InsertUserAsync(string id, string password, string name)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            string query = "INSERT INTO users (id, password, name) VALUES (@id, @password, @name)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@password", password);
            command.Parameters.AddWithValue("@name", name);
            await command.ExecuteNonQueryAsync();
        }

        public async Task<bool> ValidateUserAsync(string id, string password)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            string query = "SELECT COUNT(*) FROM users WHERE id = @id AND password = @password";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@password", password);
            long count = (long)await command.ExecuteScalarAsync();
            return count > 0;
        }
    }
}
