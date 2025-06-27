using ASSNlearningManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.DataAccess
{
    public class UserRepository
    {
        private readonly string _connectionString;

        // For Dependency Injection via IConfiguration
        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // --- LOGIN VALIDATION ---
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

        // --- INSERT USER ---
        public void AddUser(UserViewModel user)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = @"INSERT INTO user 
                                (first_name, last_name, email, phone_number, date_of_birth, gender, city, state, full_name, role_id, username, password, department)
                                VALUES 
                                (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Gender, @City, @State, @FullName, @RoleId, @Username, @Password, @Department)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                cmd.Parameters.AddWithValue("@LastName", user.LastName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", user.Gender);
                cmd.Parameters.AddWithValue("@City", user.City);
                cmd.Parameters.AddWithValue("@State", user.State);
                cmd.Parameters.AddWithValue("@FullName", user.FullName);
                cmd.Parameters.AddWithValue("@RoleId", user.RoleId);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                
                cmd.Parameters.AddWithValue("@Department", user.Department);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // --- GET ALL USERS ---
        public List<UserViewModel> GetAllUsers()
        {
            List<UserViewModel> users = new List<UserViewModel>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = @"SELECT u.user_id, u.first_name, u.last_name, u.email, u.phone_number, u.date_of_birth, u.gender, u.city, u.state,
                                        u.full_name, u.role_id, r.role_name, u.username, u.password, u.department
                                 FROM user u
                                 LEFT JOIN userrole r ON u.role_id = r.role_id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new UserViewModel
                        {
                            UserId = reader.GetInt32("user_id"),
                            FirstName = reader["first_name"] == DBNull.Value ? null : reader.GetString("first_name"),
                            LastName = reader["last_name"] == DBNull.Value ? null : reader.GetString("last_name"),
                            Email = reader["email"] == DBNull.Value ? null : reader.GetString("email"),
                            PhoneNumber = reader["phone_number"] == DBNull.Value ? null : reader.GetString("phone_number"),
                            DateOfBirth = reader["date_of_birth"] == DBNull.Value ? null : (DateTime?)reader.GetDateTime("date_of_birth"),
                            Gender = reader["gender"] == DBNull.Value ? null : reader.GetString("gender"),
                            City = reader["city"] == DBNull.Value ? null : reader.GetString("city"),
                            State = reader["state"] == DBNull.Value ? null : reader.GetString("state"),
                            FullName = reader["full_name"] == DBNull.Value ? null : reader.GetString("full_name"),
                            RoleId = reader["role_id"] == DBNull.Value ? 0 : reader.GetInt32("role_id"),
                            RoleName = reader["role_name"] == DBNull.Value ? null : reader["role_name"].ToString(),
                            Username = reader["username"] == DBNull.Value ? null : reader.GetString("username"),
                            Password = reader["password"] == DBNull.Value ? null : reader.GetString("password"),
                            Department = reader["department"] == DBNull.Value ? null : reader.GetString("department")
                        });

                    }
                }
            }

            return users;
        }
    }
}
