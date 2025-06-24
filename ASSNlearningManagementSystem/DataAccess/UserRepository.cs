using MySql.Data.MySqlClient;
using System;

namespace ASSNlearningManagementSystem.DataAccess
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool ValidateUser(string username, string password)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM Users WHERE username = @username AND password = @password";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    var result = cmd.ExecuteScalar();
                    int count = Convert.ToInt32(result);

                    return count > 0;
                }
            }
        }
    }
}
