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

        public (int UserId, string Username, string Department, string Role)? GetUserByUsernameAndPassword(string username, string password)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT u.user_id, u.full_name AS username, u.department, ur.role_name
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
                            int userId = reader.GetInt32("user_id");
                            string foundUsername = reader.GetString("username");
                            string department = reader.IsDBNull(reader.GetOrdinal("department"))
                                ? "" : reader.GetString("department");
                            string role = reader.GetString("role_name");

                            return (userId, foundUsername, department, role);
                        }
                    }
                }
            }

            return null; // User not found
        }

        public void UpdateLastLoginAndIsActive(int userId, bool isActive)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE user SET last_login = NOW(), is_active = @isActive WHERE user_id = @userId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@isActive", isActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateIsActive(int userId, bool isActive)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE user SET is_active = @isActive WHERE user_id = @userId";
                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@isActive", isActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
