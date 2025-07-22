using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System;

namespace ASSNlearningManagementSystem.Controllers
{
    public class TrainerExamController : Controller
    {
        private readonly string _connectionString;

        public TrainerExamController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // ✅ Assigned Exams
        public IActionResult AssignedExams()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var assignedExams = new List<AssignedExamViewModel>();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
        SELECT e.exam_id, e.exam_title, 
               c.CourseName, 
               t.TopicName, 
               u1.first_name AS InstructorName, 
               u2.first_name AS EvaluatorName,
               e.max_marks, e.passing_marks,
               e.duration, e.exam_date, e.status
        FROM exam e
        JOIN topic t ON e.topic_id = t.TopicID
        JOIN syllabus s ON t.SyllabusID = s.SyllabusID
        JOIN course c ON s.CourseID = c.CourseID
        LEFT JOIN user u1 ON e.instructor_id = u1.user_id
        LEFT JOIN user u2 ON e.evaluator_id = u2.user_id
        WHERE e.instructor_id = @userId OR e.evaluator_id = @userId", con);

            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                assignedExams.Add(new AssignedExamViewModel
                {
                    ExamId = reader.GetInt32("exam_id"),
                    ExamTitle = reader.GetString("exam_title"),
                    CourseName = reader.GetString("CourseName"),
                    TopicName = reader.GetString("TopicName"),
                    InstructorName = reader.IsDBNull(reader.GetOrdinal("InstructorName")) ? "" : reader.GetString("InstructorName"),
                    EvaluatorName = reader.IsDBNull(reader.GetOrdinal("EvaluatorName")) ? "" : reader.GetString("EvaluatorName"),
                    MaxMarks = reader.GetInt32("max_marks"),
                    PassingMarks = reader.GetInt32("passing_marks"),
                    Duration = reader.GetInt32("duration"),
                    ExamDate = reader.GetDateTime("exam_date"),
                    Status = reader.GetString("status")
                });
            }

            return View(assignedExams);
        }


        // ✅ Manage Questions
        public IActionResult ManageQuestions(int examId)
        {
            var questions = new List<QuestionViewModel>();

            using var con = new MySqlConnection(_connectionString);
            con.Open();
            var cmd = new MySqlCommand(@"
                SELECT QuestionID, QuestionText, QuestionType, Marks
                FROM question
                WHERE ExamID = @examId", con);
            cmd.Parameters.AddWithValue("@examId", examId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                questions.Add(new QuestionViewModel
                {
                    QuestionId = reader.GetInt32("QuestionID"),
                    ExamId = examId,
                    QuestionText = reader.GetString("QuestionText"),
                    QuestionType = reader.GetString("QuestionType"),
                    Marks = reader.GetInt32("Marks"),
                    Options = new List<MCQOptionModel>()
                });
            }

            ViewBag.ExamId = examId;
            return View(questions);
        }

        // ✅ Add Question POST
        [HttpPost]
        public IActionResult AddQuestion(int examId, QuestionViewModel model)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO question (ExamID, QuestionText, QuestionType, Marks, created_on)
                VALUES (@examId, @text, @type, @marks, NOW());
                SELECT LAST_INSERT_ID();", con);
            cmd.Parameters.AddWithValue("@examId", examId);
            cmd.Parameters.AddWithValue("@text", model.QuestionText);
            cmd.Parameters.AddWithValue("@type", model.QuestionType);
            cmd.Parameters.AddWithValue("@marks", model.Marks);

            int questionId = Convert.ToInt32(cmd.ExecuteScalar());

            if (model.QuestionType == "MCQ" && model.Options != null)
            {
                foreach (var opt in model.Options)
                {
                    var optCmd = new MySqlCommand(@"
                        INSERT INTO optiontable (QuestionID, OptionText , created_on)
                        VALUES (@qid, @text, NOW());", con);
                    optCmd.Parameters.AddWithValue("@qid", questionId);
                    optCmd.Parameters.AddWithValue("@text", opt.OptionText);
                    optCmd.ExecuteNonQuery();
                }
            }

            return RedirectToAction("ManageQuestions", new { examId });
        }

        // ✅ Edit Question GET
        public IActionResult EditQuestion(int id)
        {
            var question = new QuestionViewModel();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
                SELECT QuestionID, QuestionText, QuestionType, Marks, ExamID
                FROM question
                WHERE QuestionID = @qid", con);
            cmd.Parameters.AddWithValue("@qid", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                question.QuestionId = reader.GetInt32("QuestionID");
                question.ExamId = reader.GetInt32("ExamID");
                question.QuestionText = reader.GetString("QuestionText");
                question.QuestionType = reader.GetString("QuestionType");
                question.Marks = reader.GetInt32("Marks");
            }
            reader.Close();

            // Load options if MCQ
            if (question.QuestionType == "MCQ")
            {
                question.Options = new List<MCQOptionModel>();
                var optCmd = new MySqlCommand(@"
                    SELECT OptionID, OptionText AS MCQOptionText
                    FROM optiontable
                    WHERE QuestionID = @qid", con);
                optCmd.Parameters.AddWithValue("@qid", id);

                using var optReader = optCmd.ExecuteReader();
                while (optReader.Read())
                {
                    question.Options.Add(new MCQOptionModel
                    {
                        OptionID = optReader.GetInt32("OptionID"),
                        OptionText = optReader.GetString("MCQOptionText")
                    });
                }
            }

            return View(question);
        }

        // ✅ Edit Question POST
        [HttpPost]
        public IActionResult EditQuestion(QuestionViewModel model)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
                UPDATE question
                SET QuestionText = @text, Marks = @marks
                WHERE QuestionID = @qid", con);
            cmd.Parameters.AddWithValue("@text", model.QuestionText);
            cmd.Parameters.AddWithValue("@marks", model.Marks);
            cmd.Parameters.AddWithValue("@qid", model.QuestionId);
            cmd.ExecuteNonQuery();

            if (model.QuestionType == "MCQ")
            {
                foreach (var opt in model.Options)
                {
                    if (opt.OptionID == 0)
                    {
                        var insertOpt = new MySqlCommand(@"
                            INSERT INTO optiontable (QuestionID, OptionText, created_on)
                            VALUES (@qid, @text, NOW());", con);
                        insertOpt.Parameters.AddWithValue("@qid", model.QuestionId);
                        insertOpt.Parameters.AddWithValue("@text", opt.OptionText);
                        insertOpt.ExecuteNonQuery();
                    }
                    else
                    {
                        var updateOpt = new MySqlCommand(@"
                            UPDATE optiontable
                            SET OptionText = @text
                            WHERE OptionID = @oid", con);
                        updateOpt.Parameters.AddWithValue("@text", opt.OptionText);
                        updateOpt.Parameters.AddWithValue("@oid", opt.OptionID);
                        updateOpt.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToAction("ManageQuestions", new { examId = model.ExamId });
        }

        // ✅ Delete Question
        public IActionResult DeleteQuestion(int id)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var deleteOpt = new MySqlCommand("DELETE FROM optiontable WHERE QuestionID = @qid", con);
            deleteOpt.Parameters.AddWithValue("@qid", id);
            deleteOpt.ExecuteNonQuery();

            var deleteQ = new MySqlCommand("DELETE FROM question WHERE QuestionID = @qid", con);
            deleteQ.Parameters.AddWithValue("@qid", id);
            deleteQ.ExecuteNonQuery();

            return Redirect(Request.Headers["Referer"].ToString());
        }
        public IActionResult CompletedEvaluations()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var evaluations = new List<CompletedEvaluationViewModel>();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
        SELECT s.submission_id, e.exam_title, u.first_name AS LearnerName, 
               e.max_marks, s.obtained_marks, e.exam_date
        FROM examsubmission s
        JOIN exam e ON s.exam_id = e.exam_id
        JOIN user u ON s.user_id = u.user_id
        WHERE s.status = 'Evaluated' AND e.evaluator_id = @userId", con);

            cmd.Parameters.AddWithValue("@userId", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                evaluations.Add(new CompletedEvaluationViewModel
                {
                    SubmissionID = reader.GetInt32("submission_id"),
                    ExamTitle = reader.GetString("exam_title"),
                    LearnerName = reader.GetString("LearnerName"),
                    MaxMarks = reader.GetInt32("max_marks"),
                    ObtainedMarks = reader.GetInt32("obtained_marks"),
                    ExamDate = reader.GetDateTime("exam_date")
                });
            }

            return View(evaluations);
        }
        


    }
}
