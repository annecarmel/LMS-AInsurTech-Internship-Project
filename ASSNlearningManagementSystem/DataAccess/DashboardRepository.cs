using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using ASSNlearningManagementSystem.Models;

namespace ASSNlearningManagementSystem.DataAccess
{
    public class DashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int GetTotalUsers()
        {
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM user", conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int GetActiveCourses()
        {
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM course", conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public int GetScheduledExams()
        {
            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM exam WHERE status IN ('Pending', 'Upcoming')", conn))
            {
                conn.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public List<CourseEnrollmentCount> GetCourseEnrollmentData()
        {
            var list = new List<CourseEnrollmentCount>();
            string query = @"
                SELECT c.CourseName, COUNT(e.user_id) AS StudentCount
                FROM course c
                LEFT JOIN courseenrollments e ON c.CourseID = e.course_id
                GROUP BY c.CourseName";

            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CourseEnrollmentCount
                        {
                            CourseName = reader["CourseName"].ToString(),
                            StudentCount = Convert.ToInt32(reader["StudentCount"])
                        });
                    }
                }
            }
            return list;
        }

        public List<CourseEnrollmentCount> GetTop5PopularCourses()
        {
            var list = new List<CourseEnrollmentCount>();
            string query = @"
                SELECT c.CourseName, COUNT(e.user_id) AS EnrollmentCount
                FROM course c
                LEFT JOIN courseenrollments e ON c.CourseID = e.course_id
                GROUP BY c.CourseName
                ORDER BY EnrollmentCount DESC
                LIMIT 5";

            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new CourseEnrollmentCount
                        {
                            CourseName = reader["CourseName"].ToString(),
                            StudentCount = Convert.ToInt32(reader["EnrollmentCount"])
                        });
                    }
                }
            }
            return list;
        }

        public List<UpcomingExam> GetUpcomingExams()
        {
            var list = new List<UpcomingExam>();
            string query = @"
                SELECT exam_title, exam_date, duration, status
                FROM exam
                WHERE status IN ('Pending', 'Upcoming')
                ORDER BY exam_date ASC";

            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new UpcomingExam
                        {
                            Title = reader["exam_title"].ToString(),
                            Date = Convert.ToDateTime(reader["exam_date"]),
                            Duration = Convert.ToInt32(reader["duration"]),
                            Status = reader["status"].ToString()
                        });
                    }
                }
            }
            return list;
        }

        public List<SessionOverview> GetSessionOverview()
        {
            var list = new List<SessionOverview>();
            string query = @"
                SELECT CONCAT(u.first_name, ' ', u.last_name) AS Trainer,
                       t.TopicName,
                       (SELECT COUNT(*) FROM courseenrollments ce WHERE ce.course_id = s.course_id) AS Students,
                       CASE 
                           WHEN s.session_date = CURDATE() THEN 'Ongoing'
                           WHEN s.session_date < CURDATE() THEN 'Completed'
                           ELSE 'Scheduled'
                       END AS Status
                FROM session s
                INNER JOIN user u ON s.instructor_id = u.user_id
                INNER JOIN topic t ON s.topic_id = t.TopicID
                ORDER BY s.session_date DESC";

            using (var conn = new MySqlConnection(_connectionString))
            using (var cmd = new MySqlCommand(query, conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SessionOverview
                        {
                            Trainer = reader["Trainer"].ToString(),
                            Topic = reader["TopicName"].ToString(),
                            Students = Convert.ToInt32(reader["Students"]),
                            Status = reader["Status"].ToString()
                        });
                    }
                }
            }
            return list;
        }
    }
}
