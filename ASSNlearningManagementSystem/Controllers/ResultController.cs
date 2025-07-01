using Microsoft.AspNetCore.Mvc;
using ASSNlearningManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace ASSNlearningManagementSystem.Controllers
{
    public class ResultController : Controller
    {
        private readonly IConfiguration _configuration;

        public ResultController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Result()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Login");
            }

            List<ResultViewModel> results = new List<ResultViewModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand(@"
                    SELECT 
                        e.exam_id, 
                        e.exam_title, 
                        e.max_marks, 
                        er.marks_obtained
                    FROM examresult er
                    JOIN exam e ON er.exam_id = e.exam_id
                    WHERE er.user_id = @userId", con);

                cmd.Parameters.AddWithValue("@userId", userId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(new ResultViewModel
                        {
                            ExamId = reader.GetInt32("exam_id"),
                            ExamTitle = reader.GetString("exam_title"),
                            MaxMarks = reader.GetInt32("max_marks"),
                            MarksObtained = reader.GetInt32("marks_obtained")
                        });
                    }
                }
            }

            return View(results);
        }
    }
}