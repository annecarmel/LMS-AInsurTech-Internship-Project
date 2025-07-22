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
            using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM exam WHERE LOWER(TRIM(status)) = 'scheduled'", conn))
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
                WHERE LOWER(TRIM(status)) = 'scheduled'
                AND exam_date >= CURDATE()
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

        public List<TopStudentViewModel> GetTop5Students()
        {
            var students = new List<TopStudentViewModel>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
    SELECT 
        CONCAT(u.first_name, ' ', u.last_name) AS StudentName,
        c.CourseName AS CourseName,
        e.exam_title AS ExamName,
        er.marks_obtained AS MarksObtained
    FROM examresult er
    JOIN user u ON er.user_id = u.user_id
    JOIN exam e ON er.exam_id = e.exam_id
    JOIN topic t ON e.topic_id = t.TopicID
    JOIN syllabus s ON t.SyllabusID = s.SyllabusID
    JOIN course c ON s.CourseID = c.CourseID
    ORDER BY er.marks_obtained DESC
    LIMIT 5;";



                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new TopStudentViewModel
                        {
                            StudentName = reader["StudentName"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            ExamName = reader["ExamName"].ToString(),
                            MarksObtained = Convert.ToInt32(reader["MarksObtained"])
                        });
                    }
                }
            }

            return students;
        }

        public List<TopReviewViewModel> GetTop5Reviews()
        {
            var reviews = new List<TopReviewViewModel>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
            SELECT 
                r.Feedback AS Comment,
                r.Rating_Value AS Rating,
                CONCAT(u.first_name, ' ', u.last_name) AS StudentName,
                c.CourseName
            FROM rating r
            JOIN user u ON r.user_id = u.user_id
            JOIN course c ON r.CourseID = c.CourseID
            ORDER BY r.Rating_Value DESC
            LIMIT 5";

                using (var cmd = new MySqlCommand(query, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        reviews.Add(new TopReviewViewModel
                        {
                            StudentName = reader["StudentName"].ToString(),
                            CourseName = reader["CourseName"].ToString(),
                            Rating = Convert.ToDouble(reader["Rating"]),
                            Comment = reader["Comment"].ToString()
                        });
                    }
                }
            }

            return reviews;
        }

    }
}
