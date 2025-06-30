using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly IConfiguration _configuration;

        public CourseController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Dashboard()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            List<Course> courseList = new List<Course>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT CourseId, CourseName FROM Course";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Course course = new Course
                            {
                                CourseId = Convert.ToInt32(reader["CourseId"]),
                                CourseName = reader["CourseName"].ToString()
                            };
                            courseList.Add(course);
                        }
                    }
                }
            }



            return View(courseList);
        }
    }
}