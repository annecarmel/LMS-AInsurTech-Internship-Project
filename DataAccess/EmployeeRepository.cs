using ASSNlearningManagementSystem.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.DataAccess
{
    public class EmployeeRepository
    {
        private readonly string _connectionString = string.Empty;

        public EmployeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void AddEmployee(EmployeeViewModel employee)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = @"INSERT INTO user 
                        (first_name, last_name, email, phone_number, date_of_birth, gender, city, state, country, department, role_id,
                         username, password, full_name, created_on, updated_on, is_active, created_by, updated_by)
                        VALUES 
                        (@FirstName, @LastName, @Email, @PhoneNumber, @DateOfBirth, @Gender, @City, @State, @Country, @Department, @RoleId,
                         @Username, @Password, @FullName, @CreatedOn, @UpdatedOn, @IsActive, @CreatedBy, @UpdatedBy)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", employee.first_name);
                cmd.Parameters.AddWithValue("@LastName", employee.last_name);
                cmd.Parameters.AddWithValue("@Email", employee.email);
                cmd.Parameters.AddWithValue("@PhoneNumber", employee.phone_number);
                cmd.Parameters.AddWithValue("@DateOfBirth", employee.date_of_birth);
                cmd.Parameters.AddWithValue("@Gender", employee.gender);
                cmd.Parameters.AddWithValue("@City", employee.city);
                cmd.Parameters.AddWithValue("@State", employee.state);
                cmd.Parameters.AddWithValue("@Country", employee.country);
                cmd.Parameters.AddWithValue("@Department", employee.department);
                cmd.Parameters.AddWithValue("@RoleId", employee.role_id);

                cmd.Parameters.AddWithValue("@Username", employee.username);
                cmd.Parameters.AddWithValue("@Password", employee.password);
                cmd.Parameters.AddWithValue("@FullName", employee.full_name);
                cmd.Parameters.AddWithValue("@CreatedOn", employee.created_on);
                cmd.Parameters.AddWithValue("@UpdatedOn", employee.updated_on);
                cmd.Parameters.AddWithValue("@IsActive", employee.is_active);

                // ✅ New fields
                cmd.Parameters.AddWithValue("@CreatedBy", employee.created_by);
                cmd.Parameters.AddWithValue("@UpdatedBy", employee.updated_by);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public List<EmployeeViewModel> GetAllEmployees()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = @"SELECT user_id, first_name, last_name, email, phone_number, date_of_birth, gender,
                                        city, state, country, department, role_id
                                 FROM user";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        employees.Add(new EmployeeViewModel
                        {
                            user_id = reader["user_id"] != DBNull.Value ? Convert.ToInt32(reader["user_id"]) : 0,
                            first_name = reader["first_name"]?.ToString(),
                            last_name = reader["last_name"]?.ToString(),
                            email = reader["email"]?.ToString(),
                            phone_number = reader["phone_number"]?.ToString(),
                            date_of_birth = reader["date_of_birth"] != DBNull.Value ? Convert.ToDateTime(reader["date_of_birth"]) : null,
                            gender = reader["gender"]?.ToString(),
                            city = reader["city"]?.ToString(),
                            state = reader["state"]?.ToString(),
                            country = reader["country"]?.ToString(),
                            department = reader["department"]?.ToString(),
                            role_id = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0
                        });
                    }
                }
            }

            return employees;
        }

        public EmployeeViewModel GetEmployeeById(int id)
        {
            EmployeeViewModel employee = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = @"SELECT user_id, first_name, last_name, email, phone_number, date_of_birth, gender,
                                city, state, country, department, role_id
                         FROM user
                         WHERE user_id = @Id";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Id", id);
                conn.Open();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        employee = new EmployeeViewModel
                        {
                            user_id = Convert.ToInt32(reader["user_id"]),
                            first_name = reader["first_name"]?.ToString(),
                            last_name = reader["last_name"]?.ToString(),
                            email = reader["email"]?.ToString(),
                            phone_number = reader["phone_number"]?.ToString(),
                            date_of_birth = reader["date_of_birth"] != DBNull.Value ? Convert.ToDateTime(reader["date_of_birth"]) : null,
                            gender = reader["gender"]?.ToString(),
                            city = reader["city"]?.ToString(),
                            state = reader["state"]?.ToString(),
                            country = reader["country"]?.ToString(),
                            department = reader["department"]?.ToString(),
                            role_id = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0
                        };
                    }
                }
            }

            return employee;
        }

        public void UpdateEmployee(EmployeeViewModel employee)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = @"UPDATE user SET 
                            first_name = @FirstName,
                            last_name = @LastName,
                            email = @Email,
                            phone_number = @PhoneNumber,
                            date_of_birth = @DateOfBirth,
                            gender = @Gender,
                            city = @City,
                            state = @State,
                            country = @Country,
                            department = @Department,
                            role_id = @RoleId,
                            updated_on = @UpdatedOn,
                            updated_by = @UpdatedBy
                         WHERE user_id = @UserId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@FirstName", employee.first_name);
                cmd.Parameters.AddWithValue("@LastName", employee.last_name);
                cmd.Parameters.AddWithValue("@Email", employee.email);
                cmd.Parameters.AddWithValue("@PhoneNumber", employee.phone_number);
                cmd.Parameters.AddWithValue("@DateOfBirth", employee.date_of_birth);
                cmd.Parameters.AddWithValue("@Gender", employee.gender);
                cmd.Parameters.AddWithValue("@City", employee.city);
                cmd.Parameters.AddWithValue("@State", employee.state);
                cmd.Parameters.AddWithValue("@Country", employee.country);
                cmd.Parameters.AddWithValue("@Department", employee.department);
                cmd.Parameters.AddWithValue("@RoleId", employee.role_id);
                cmd.Parameters.AddWithValue("@UpdatedOn", employee.updated_on);
                cmd.Parameters.AddWithValue("@UpdatedBy", employee.updated_by);
                cmd.Parameters.AddWithValue("@UserId", employee.user_id);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteEmployee(int userId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = "DELETE FROM user WHERE user_id = @UserId";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
