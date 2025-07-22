using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Mysqlx.Cursor;

namespace ASSNlearningManagementSystem.Controllers
{
    public class TrainerController : Controller
    {
        private readonly string _connectionString;

        public TrainerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Dashboard()
        {
            var model = new TrainerDashboardViewModel
            {
                TotalLearners = GetTotalLearners(),
                AssignedCourses = GetAssignedCourses(),
                TotalCourses = GetTotalCourses(),
                Sessions = GetSessionSchedules(),
                Evaluations = GetEvaluations(),
                // Fix for BarChartData
                BarChartData = GetSessionCountsPerCourse()
        .ToDictionary(item => item.CourseName, item => item.SessionCount),

                // Fix for PieChartData (assuming same structure applies)
                PieChartData = GetExamSubmissionsPerCourse()
        .ToDictionary(item => item.CourseName, item => item.SubmissionCount)
            };

            return View(model);
        }

        private int GetTotalLearners()
        {
            using var con = new MySqlConnection(_connectionString);
            var cmd = new MySqlCommand("SELECT COUNT(*) FROM user WHERE role_id = 3", con);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private int GetAssignedCourses()
        {
            using var con = new MySqlConnection(_connectionString);
            var cmd = new MySqlCommand("SELECT COUNT(DISTINCT course_id) FROM courseenrollments", con);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private int GetTotalCourses()
        {
            using var con = new MySqlConnection(_connectionString);
            var cmd = new MySqlCommand("SELECT COUNT(*) FROM course", con);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private List<SessionSchedule> GetSessionSchedules()
        {
            var list = new List<SessionSchedule>();

            // ✅ Get logged-in user id from session
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                // Handle unauthenticated case as needed
                return list;
            }

            using var con = new MySqlConnection(_connectionString);
            string query = @"
        SELECT s.session_id, t.TopicName, s.session_date, CONCAT(u.first_name, ' ', u.last_name) AS LearnerName
        FROM session s
        JOIN topic t ON s.topic_id = t.TopicID
        JOIN user u ON s.instructor_id = u.user_id
        WHERE s.instructor_id = @userId";

            var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@userId", userId);

            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SessionSchedule
                {
                    SessionId = reader.GetInt32("session_id"),
                    Topic = reader["TopicName"].ToString(),
                    Date = reader.GetDateTime("session_date"),
                    LearnerName = reader["LearnerName"].ToString()
                });
            }
            return list;
        }


        private List<EvaluationInfo> GetEvaluations()
        {
            var list = new List<EvaluationInfo>();
            using var con = new MySqlConnection(_connectionString);

            string query = @"
SELECT 
    e.exam_id, 
    e.exam_title, 
    CONCAT(u.first_name, ' ', u.last_name) AS LearnerName,
    es.TotalMarks,
    er.marks_obtained,
    'Pending' AS Status
FROM examsubmission es
JOIN exam e ON es.ExamID = e.exam_id
JOIN user u ON es.user_id = u.user_id
LEFT JOIN examresult er ON es.ExamID = er.exam_id AND es.user_id = er.user_id
WHERE er.marks_obtained IS NULL";

    var cmd = new MySqlCommand(query, con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new EvaluationInfo
                {
                    ExamId = reader.GetInt32("exam_id"),
                    ExamName = reader["exam_title"].ToString(),
                    LearnerName = reader["LearnerName"].ToString(),
                    Status = reader["Status"].ToString(),
                    TotalMarks = reader["TotalMarks"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["TotalMarks"]),
                    MarksObtained = reader["marks_obtained"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["marks_obtained"])
                });
            }
            return list;
        }



        private List<BarChartData> GetSessionCountsPerCourse()
        {
            var list = new List<BarChartData>();
            using var con = new MySqlConnection(_connectionString);
            string query = @"
                SELECT c.CourseName, COUNT(s.session_id) AS SessionCount
                FROM session s
                JOIN topic t ON s.topic_id = t.TopicID
                JOIN syllabus sy ON t.SyllabusID = sy.SyllabusID
                JOIN course c ON sy.CourseID = c.CourseID
                GROUP BY c.CourseName";
            var cmd = new MySqlCommand(query, con);
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new BarChartData
                {
                    CourseName = reader["CourseName"].ToString(),
                    SessionCount = Convert.ToInt32(reader["SessionCount"])
                });
            }
            return list;
        }

        private List<PieChartData> GetExamSubmissionsPerCourse()
        {
            var list = new List<PieChartData>();
            using var con = new MySqlConnection(_connectionString);
            string query = @"
                SELECT c.CourseName, COUNT(es.SubmissionID) AS SubmissionCount
                FROM examsubmission es
                JOIN exam e ON es.ExamID = e.exam_id
                JOIN topic t ON e.topic_id = t.TopicID
                JOIN syllabus s ON t.SyllabusID = s.SyllabusID
                JOIN course c ON s.CourseID = c.CourseID
                GROUP BY c.CourseName";
            var cmd = new MySqlCommand(query, con);
            con.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PieChartData
                {
                    CourseName = reader["CourseName"].ToString(),
                    SubmissionCount = Convert.ToInt32(reader["SubmissionCount"])
                });
            }
            return list;
        }
    }
}
