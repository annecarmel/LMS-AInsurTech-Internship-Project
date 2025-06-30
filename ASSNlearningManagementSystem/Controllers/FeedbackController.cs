using Microsoft.AspNetCore.Mvc;
using ASSNlearningManagementSystem.Models;
using MySql.Data.MySqlClient;

namespace ASSNlearningManagementSystem.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IConfiguration _configuration;

        public FeedbackController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            int trainerId = HttpContext.Session.GetInt32("UserId") ?? 0;

            var model = new TrainerFeedbackPageViewModel
            {
                Feedbacks = GetFeedbacks(),
                AssignedTopics = GetAssignedTopics(trainerId)
            };

            return View(model);
        }

        private List<FeedbackViewModel> GetFeedbacks()
        {
            List<FeedbackViewModel> feedbacks = new List<FeedbackViewModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand(@"
                    SELECT r.RatingID, CONCAT(u.first_name, ' ', u.last_name) AS user_name, 
                           c.CourseName AS course_name, r.Feedback
                    FROM rating r
                    JOIN user u ON r.user_id = u.user_id
                    JOIN course c ON r.CourseID = c.CourseID", con);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        feedbacks.Add(new FeedbackViewModel
                        {
                            RatingId = reader.GetInt32("RatingID"),
                            UserName = reader.GetString("user_name"),
                            CourseName = reader.GetString("course_name"),
                            FeedbackText = reader.GetString("Feedback")
                        });
                    }
                }
            }

            return feedbacks;
        }

        private List<AssignedTopicViewModel> GetAssignedTopics(int trainerId)
        {
            List<AssignedTopicViewModel> topics = new List<AssignedTopicViewModel>();
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand(@"
                    SELECT c.CourseName, t.TopicName, t.Description
                    FROM topic t
                    JOIN syllabus s ON t.SyllabusID = s.SyllabusID
                    JOIN course c ON s.CourseID = c.CourseID
                    WHERE t.instructor_id = @trainerId", con);

                cmd.Parameters.AddWithValue("@trainerId", trainerId);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topics.Add(new AssignedTopicViewModel
                        {
                            CourseName = reader.GetString("CourseName"),
                            TopicName = reader.GetString("TopicName"),
                            TopicDescription = reader.GetString("Description")
                        });
                    }
                }
            }

            return topics;
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (var con = new MySqlConnection(connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand("DELETE FROM rating WHERE RatingID = @id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
