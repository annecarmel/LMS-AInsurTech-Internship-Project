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

                string query = "SELECT COUNT(*) FROM user WHERE username = @username AND password = @password";
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

        public (string Username, string Role)? GetUserByUsernameAndPassword(string username, string password)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT u.username, ur.role_name
                    FROM user u
                    INNER JOIN userrole ur ON u.role_id = ur.role_id
                    WHERE u.username = @username AND u.password = @password";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string foundUsername = reader.GetString("username");
                            string role = reader.GetString("role_name");
                            return (foundUsername, role);
                        }
                    }
                }
            }

            return null; // User not found
        }
    }
}
