using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace ASSNlearningManagementSystem.Controllers
{
    public class ExamController : Controller
    {
        private readonly string _connectionString;

        public ExamController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IActionResult Create()
        {
            ViewBag.Courses = GetCourses();
            ViewBag.Topics = GetTopics();
            ViewBag.Users = GetUsers();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Exam exam, List<Question> questions)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var examCmd = new MySqlCommand(@"INSERT INTO exam 
                (exam_title, topic_id, exam_date, duration, status, max_marks, passing_marks, evaluator_id, instructor_id, created_on) 
                VALUES (@title, @topic, @date, @duration, 'Scheduled', @maxmarks, @passing, @evaluator, @instructor, NOW());
                SELECT LAST_INSERT_ID();", con);

            examCmd.Parameters.AddWithValue("@title", exam.ExamTitle);
            examCmd.Parameters.AddWithValue("@topic", exam.TopicId);
            examCmd.Parameters.AddWithValue("@date", exam.ExamDate);
            examCmd.Parameters.AddWithValue("@duration", exam.Duration);
            examCmd.Parameters.AddWithValue("@maxmarks", exam.MaxMarks);
            examCmd.Parameters.AddWithValue("@passing", exam.PassingMarks);
            examCmd.Parameters.AddWithValue("@evaluator", exam.EvaluatorId);
            examCmd.Parameters.AddWithValue("@instructor", exam.InstructorId);

            int examId = Convert.ToInt32(examCmd.ExecuteScalar());

            foreach (var q in questions)
            {
                var qCmd = new MySqlCommand(@"INSERT INTO question 
                    (ExamID, QuestionText, QuestionType, Marks, created_on) 
                    VALUES (@examid, @text, @type, @marks, NOW());
                    SELECT LAST_INSERT_ID();", con);
                qCmd.Parameters.AddWithValue("@examid", examId);
                qCmd.Parameters.AddWithValue("@text", q.QuestionText);
                qCmd.Parameters.AddWithValue("@type", q.QuestionType);
                qCmd.Parameters.AddWithValue("@marks", q.Marks);
                int questionId = Convert.ToInt32(qCmd.ExecuteScalar());

                if (q.QuestionType == "MCQ" && q.Options != null)
                {
                    foreach (var opt in q.Options)
                    {
                        var oCmd = new MySqlCommand(@"INSERT INTO optiontable
                            (QuestionID, OptionText, IsCorrect, created_on)
                            VALUES (@qid, @text, @iscorrect, NOW());", con);
                        oCmd.Parameters.AddWithValue("@qid", questionId);
                        oCmd.Parameters.AddWithValue("@text", opt.OptionText);
                        oCmd.Parameters.AddWithValue("@iscorrect", opt.IsCorrect);
                        oCmd.ExecuteNonQuery();
                    }
                }
            }

            TempData["Success"] = "Exam created successfully!";
            return RedirectToAction("Create");
        }

        private List<Course> GetCourses()
        {
            var list = new List<Course>();
            using var con = new MySqlConnection(_connectionString);
            con.Open();
            var cmd = new MySqlCommand("SELECT CourseId, CourseName FROM course", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Course
                {
                    CourseId = reader.GetInt32("CourseId"),
                    CourseName = reader.GetString("CourseName")
                });
            }
            return list;
        }

        private List<Topic> GetTopics()
        {
            var list = new List<Topic>();
            using var con = new MySqlConnection(_connectionString);
            con.Open();
            var cmd = new MySqlCommand("SELECT TopicID, TopicName FROM topic", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Topic
                {
                    TopicID = reader.GetInt32("TopicID"),
                    TopicName = reader.GetString("TopicName")
                });
            }
            return list;
        }

        private List<UserProfileViewModel> GetUsers()
        {
            var list = new List<UserProfileViewModel>();
            using var con = new MySqlConnection(_connectionString);
            con.Open();
            var cmd = new MySqlCommand("SELECT user_id, first_name, last_name FROM user", con);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new UserProfileViewModel
                {
                    UserId = reader.GetInt32("user_id"),
                    FirstName = reader.GetString("first_name"),
                    LastName = reader.GetString("last_name")
                });
            }
            return list;
        }
        public IActionResult PendingEvaluations()
        {
            int? evaluatorId = HttpContext.Session.GetInt32("UserId");
            if (evaluatorId == null) return RedirectToAction("Login", "Account");

            var pendingList = new List<PendingEvaluationViewModel>();
            using var con = new MySqlConnection(_connectionString);
            con.Open();
            var cmd = new MySqlCommand(@"
        SELECT es.SubmissionID, e.exam_title, u.first_name, u.last_name, es.SubmissionDate 
        FROM examsubmission es
        JOIN exam e ON es.ExamID = e.exam_id
        JOIN user u ON es.user_id = u.user_id
        LEFT JOIN examresult er ON es.ExamID = er.exam_id AND es.user_id = er.user_id
        WHERE e.evaluator_id = @evaluatorId AND er.result_id IS NULL", con);

            cmd.Parameters.AddWithValue("@evaluatorId", evaluatorId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                pendingList.Add(new PendingEvaluationViewModel
                {
                    SubmissionID = reader.GetInt32("SubmissionID"),
                    ExamTitle = reader.GetString("exam_title"),
                    LearnerName = reader.GetString("first_name") + " " + reader.GetString("last_name"),
                    SubmissionDate = reader.GetDateTime("SubmissionDate")
                });
            }
            return View(pendingList);
        }

        public IActionResult Evaluate(int submissionId)
        {
            var model = new EvaluateExamViewModel();
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            // Fetch submission, learner, passing marks
            var cmd1 = new MySqlCommand(@"
        SELECT e.exam_title, u.first_name, u.last_name, e.passing_marks
        FROM examsubmission es
        JOIN exam e ON es.ExamID = e.exam_id
        JOIN user u ON es.user_id = u.user_id
        WHERE es.SubmissionID = @subId", con);
            cmd1.Parameters.AddWithValue("@subId", submissionId);

            using var reader1 = cmd1.ExecuteReader();
            if (reader1.Read())
            {
                model.SubmissionID = submissionId;
                model.ExamTitle = reader1.GetString("exam_title");
                model.LearnerName = reader1.GetString("first_name") + " " + reader1.GetString("last_name");
                model.PassingMarks = reader1.GetInt32("passing_marks");
            }
            reader1.Close();

            // Fetch answers
            var cmd2 = new MySqlCommand(@"
        SELECT q.QuestionText, a.AnswerText, a.AnswerID, a.MarksAwarded
        FROM answer a
        JOIN question q ON a.QuestionID = q.QuestionID
        WHERE a.SubmissionID = @subId", con);
            cmd2.Parameters.AddWithValue("@subId", submissionId);

            using var reader2 = cmd2.ExecuteReader();
            model.Answers = new List<AnswerEvaluation>();
            while (reader2.Read())
            {
                model.Answers.Add(new AnswerEvaluation
                {
                    AnswerID = reader2.GetInt32(reader2.GetOrdinal("AnswerID")),
                    QuestionText = reader2.GetString(reader2.GetOrdinal("QuestionText")),
                    AnswerText = reader2.IsDBNull(reader2.GetOrdinal("AnswerText"))
                                    ? ""
                                    : reader2.GetString(reader2.GetOrdinal("AnswerText")),
                    MarksAwarded = reader2.IsDBNull(reader2.GetOrdinal("MarksAwarded"))
                                    ? (int?)null
                                    : reader2.GetInt32(reader2.GetOrdinal("MarksAwarded"))
                });
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Evaluate(EvaluateExamViewModel model)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            int totalMarks = 0;
            foreach (var ans in model.Answers)
            {
                var cmd = new MySqlCommand("UPDATE answer SET MarksAwarded = @marks WHERE AnswerID = @aid", con);
                cmd.Parameters.AddWithValue("@marks", ans.MarksAwarded);
                cmd.Parameters.AddWithValue("@aid", ans.AnswerID);
                cmd.ExecuteNonQuery();

                totalMarks += ans.MarksAwarded ?? 0;
            }

            var passFlag = totalMarks >= model.PassingMarks ? 1 : 0;

            var resCmd = new MySqlCommand(@"INSERT INTO examresult 
        (exam_id, user_id, marks_obtained, passed, created_on) 
        SELECT es.ExamID, es.user_id, @marks, @passed, NOW()
        FROM examsubmission es WHERE es.SubmissionID = @subId", con);
            resCmd.Parameters.AddWithValue("@marks", totalMarks);
            resCmd.Parameters.AddWithValue("@passed", passFlag);
            resCmd.Parameters.AddWithValue("@subId", model.SubmissionID);

            resCmd.ExecuteNonQuery();

            TempData["Success"] = "Evaluation saved successfully!";
            return RedirectToAction("PendingEvaluations");
        }

    }
}
