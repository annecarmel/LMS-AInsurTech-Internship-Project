using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Controllers
{
    public class ExamQuestionController : Controller
    {
        private readonly string _connectionString;

        public ExamQuestionController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // GET: AssignQuestions
        public IActionResult AssignQuestions(int examId)
        {
            ViewBag.ExamId = examId;
            return View(new List<Question>());
        }

        // POST: AssignQuestions
        [HttpPost]
        public IActionResult AssignQuestions(int examId, List<Question> questions)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

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

                // Insert options if MCQ
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

            TempData["Success"] = "Questions assigned successfully!";
            return RedirectToAction("ViewQuestions", new { examId });
        }

        // GET: ViewQuestions
        public IActionResult ViewQuestions(int examId)
        {
            var questions = new List<Question>();
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var qCmd = new MySqlCommand("SELECT * FROM question WHERE ExamID = @eid", con);
            qCmd.Parameters.AddWithValue("@eid", examId);

            using var reader = qCmd.ExecuteReader();
            while (reader.Read())
            {
                questions.Add(new Question
                {
                    QuestionID = reader.GetInt32("QuestionID"),
                    ExamID = reader.GetInt32("ExamID"),
                    QuestionText = reader.GetString("QuestionText"),
                    QuestionType = reader.GetString("QuestionType"),
                    Marks = reader.GetInt32("Marks"),
                    Options = new List<QuestionOption>()
                });
            }
            reader.Close();

            // Load options for MCQs
            foreach (var q in questions)
            {
                if (q.QuestionType == "MCQ")
                {
                    var optCmd = new MySqlCommand("SELECT * FROM optiontable WHERE QuestionID = @qid", con);
                    optCmd.Parameters.AddWithValue("@qid", q.QuestionID);

                    using var optReader = optCmd.ExecuteReader();
                    while (optReader.Read())
                    {
                        q.Options.Add(new QuestionOption
                        {
                            OptionID = optReader.GetInt32("OptionID"),
                            OptionText = optReader.GetString("OptionText"),
                            IsCorrect = optReader.GetBoolean("IsCorrect")
                        });
                    }
                }
            }

            ViewBag.ExamId = examId;
            return View(questions);
        }

        // GET: EditQuestion
        public IActionResult EditQuestion(int questionId)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var qCmd = new MySqlCommand("SELECT * FROM question WHERE QuestionID = @qid", con);
            qCmd.Parameters.AddWithValue("@qid", questionId);

            using var reader = qCmd.ExecuteReader();
            if (!reader.Read()) return NotFound();

            var q = new Question
            {
                QuestionID = reader.GetInt32("QuestionID"),
                ExamID = reader.GetInt32("ExamID"),
                QuestionText = reader.GetString("QuestionText"),
                QuestionType = reader.GetString("QuestionType"),
                Marks = reader.GetInt32("Marks"),
                Options = new List<QuestionOption>()
            };
            reader.Close();

            // Load options
            if (q.QuestionType == "MCQ")
            {
                var optCmd = new MySqlCommand("SELECT * FROM optiontable WHERE QuestionID = @qid", con);
                optCmd.Parameters.AddWithValue("@qid", questionId);

                using var optReader = optCmd.ExecuteReader();
                while (optReader.Read())
                {
                    q.Options.Add(new QuestionOption
                    {
                        OptionID = optReader.GetInt32("OptionID"),
                        OptionText = optReader.GetString("OptionText"),
                        IsCorrect = optReader.GetBoolean("IsCorrect")
                    });
                }
            }

            return View(q);
        }

        // POST: EditQuestion
        [HttpPost]
        public IActionResult EditQuestion(Question q)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            // Update question details
            var cmd = new MySqlCommand(@"UPDATE question 
                SET QuestionText = @text, Marks = @marks 
                WHERE QuestionID = @qid", con);

            cmd.Parameters.AddWithValue("@text", q.QuestionText);
            cmd.Parameters.AddWithValue("@marks", q.Marks);
            cmd.Parameters.AddWithValue("@qid", q.QuestionID);

            cmd.ExecuteNonQuery();

            // Handle options if MCQ
            if (q.QuestionType == "MCQ" && q.Options != null)
            {
                foreach (var opt in q.Options)
                {
                    if (opt.IsDeleted)
                    {
                        // Delete option
                        var delCmd = new MySqlCommand("DELETE FROM optiontable WHERE OptionID = @oid", con);
                        delCmd.Parameters.AddWithValue("@oid", opt.OptionID);
                        delCmd.ExecuteNonQuery();
                    }
                    else if (opt.OptionID == 0)
                    {
                        // Insert new option
                        var insertCmd = new MySqlCommand(@"INSERT INTO optiontable
                            (QuestionID, OptionText, IsCorrect, created_on)
                            VALUES (@qid, @text, @iscorrect, NOW());", con);

                        insertCmd.Parameters.AddWithValue("@qid", q.QuestionID);
                        insertCmd.Parameters.AddWithValue("@text", opt.OptionText);
                        insertCmd.Parameters.AddWithValue("@iscorrect", opt.IsCorrect);
                        insertCmd.ExecuteNonQuery();
                    }
                    else
                    {
                        // Update existing option
                        var updateCmd = new MySqlCommand(@"UPDATE optiontable
                            SET OptionText = @text, IsCorrect = @iscorrect
                            WHERE OptionID = @oid", con);

                        updateCmd.Parameters.AddWithValue("@text", opt.OptionText);
                        updateCmd.Parameters.AddWithValue("@iscorrect", opt.IsCorrect);
                        updateCmd.Parameters.AddWithValue("@oid", opt.OptionID);
                        updateCmd.ExecuteNonQuery();
                    }
                }
            }

            TempData["Success"] = "Question updated successfully!";
            return RedirectToAction("ViewQuestions", new { examId = q.ExamID });
        }

        // GET: DeleteQuestion
        public IActionResult DeleteQuestion(int questionId, int examId)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            // Delete options first
            var optCmd = new MySqlCommand("DELETE FROM optiontable WHERE QuestionID = @qid", con);
            optCmd.Parameters.AddWithValue("@qid", questionId);
            optCmd.ExecuteNonQuery();

            // Then delete question
            var qCmd = new MySqlCommand("DELETE FROM question WHERE QuestionID = @qid", con);
            qCmd.Parameters.AddWithValue("@qid", questionId);
            qCmd.ExecuteNonQuery();

            TempData["Success"] = "Question deleted successfully!";
            return RedirectToAction("ViewQuestions", new { examId });
        }
    }
}
