using ASSNlearningManagementSystem.Models;
using ASSNlearningManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace ASSNlearningManagementSystem.Controllers
{
    public class ExamController : Controller
    {
        private readonly string _connectionString;
        private readonly ExamRepository _examRepo;
        private readonly ExamResultRepository _resultRepo;

        public ExamController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _examRepo = new ExamRepository(connStr);
            _resultRepo = new ExamResultRepository(connStr);
        }

        public IActionResult Exam()
        {
            var exams = _examRepo.GetAllExams();
            ViewBag.Courses = _examRepo.GetCourses();
            ViewBag.Instructors = _examRepo.GetTrainers();
            ViewBag.Evaluators = _examRepo.GetTrainers();
            return View(exams);
        }

        // Exam Master - Create new exam
        [HttpPost]
        public IActionResult Create(Exam exam)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                exam.CreatedBy = userId.Value;
                exam.UpdatedBy = userId.Value;
            }

            exam.CreatedOn = DateTime.Now;
            exam.UpdatedOn = DateTime.Now;
            exam.Status = "Scheduled";

            _examRepo.InsertExam(exam);
            TempData["ExamMessage"] = "saved";
            return RedirectToAction("Exam");
        }

        // Exam Master - Edit GET
        public IActionResult Edit(int id)
        {
            var examToEdit = _examRepo.GetExamById(id);
            var exams = _examRepo.GetAllExams();

            ViewBag.ExamToEdit = examToEdit;
            ViewBag.Courses = _examRepo.GetCourses();
            ViewBag.Instructors = _examRepo.GetTrainers();
            ViewBag.Evaluators = _examRepo.GetTrainers();

            return View("Exam", exams);
        }

        // Exam Master - Edit POST
        [HttpPost]
        public IActionResult Edit(Exam exam)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                exam.UpdatedBy = userId.Value;
            }

            exam.UpdatedOn = DateTime.Now;

            _examRepo.UpdateExam(exam);
            TempData["ExamMessage"] = "updated";
            return RedirectToAction("Exam");
        }

        // Exam Master - Delete
        public IActionResult Delete(int id)
        {
            try
            {
                _examRepo.DeleteExam(id);
                TempData["ExamMessage"] = "deleted";
            }
            catch (Exception)
            {
                TempData["ExamMessage"] = "Cannot delete this exam. It is already in use.";
            }

            return RedirectToAction("Exam");
        }

        //Dependent dropdown: Syllabuses by Course
        public IActionResult GetSyllabuses(int courseId)
        {
            var syllabuses = _examRepo.GetSyllabusesByCourseId(courseId);
            return Json(syllabuses);
        }

        // Dependent dropdown: Topics by Syllabus
        public IActionResult GetTopics(int syllabusId)
        {
            var topics = _examRepo.GetTopicsBySyllabusId(syllabusId);
            return Json(topics);
        }

        // Exam Results page
        public IActionResult Results()
        {
            var results = _resultRepo.GetAllExamResults();
            return View("Results", results);
        }
        public IActionResult Create()
        {
            ViewBag.Courses = _examRepo.GetCourses();
            ViewBag.Instructors = _examRepo.GetTrainers();
            ViewBag.Evaluators = _examRepo.GetTrainers();

            var exams = _examRepo.GetAllExams(); 

            return View("~/Views/Trainer/Exam/Create.cshtml", exams);
        }
        // Pending Evaluations for Evaluator (Trainer)
        public IActionResult PendingEvaluations()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToAction("Login", "Account");

            var pending = new List<PendingEvaluationViewModel>();
            var completed = new List<CompletedEvaluationViewModel>();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            // Fetch pending evaluations
            var cmdPending = new MySqlCommand(@"SELECT s.SubmissionID, e.exam_title, u.first_name AS LearnerName, s.SubmissionDate
FROM examsubmission s
JOIN exam e ON s.ExamID = e.exam_id
JOIN user u ON s.user_id = u.user_id
LEFT JOIN examresult r ON s.ExamID = r.exam_id AND s.user_id = r.user_id
WHERE r.result_id IS NULL AND e.evaluator_id = @userId;", con);

            cmdPending.Parameters.AddWithValue("@userId", userId);

            using var readerPending = cmdPending.ExecuteReader();
            while (readerPending.Read())
            {
                pending.Add(new PendingEvaluationViewModel
                {
                    SubmissionID = readerPending.GetInt32("SubmissionID"),
                    ExamTitle = readerPending.GetString("exam_title"),
                    LearnerName = readerPending.GetString("LearnerName"),
                    SubmissionDate = readerPending.GetDateTime("SubmissionDate")
                });
            }
            readerPending.Close();

            // Fetch completed evaluations
            var cmdCompleted = new MySqlCommand(@"
        SELECT s.SubmissionID, e.exam_title, u.first_name AS LearnerName, 
       e.max_marks, r.marks_obtained, e.exam_date
FROM examsubmission s
JOIN exam e ON s.ExamID = e.exam_id
JOIN user u ON s.user_id = u.user_id
JOIN examresult r ON s.ExamID = r.exam_id AND s.user_id = r.user_id
WHERE e.evaluator_id = @userId;", con);

            cmdCompleted.Parameters.AddWithValue("@userId", userId);

            using var readerCompleted = cmdCompleted.ExecuteReader();
            while (readerCompleted.Read())
            {
                completed.Add(new CompletedEvaluationViewModel
                {
                    SubmissionID = readerCompleted.GetInt32("SubmissionID"),
                    ExamTitle = readerCompleted.GetString("exam_title"),
                    LearnerName = readerCompleted.GetString("LearnerName"),
                    MaxMarks = readerCompleted.GetInt32("max_marks"),
                    ObtainedMarks = readerCompleted.GetInt32("marks_obtained"),
                    ExamDate = readerCompleted.GetDateTime("exam_date")
                });
            }

            var model = new EvaluationPageViewModel
            {
                Pending = pending,
                Completed = completed
            };

            return View(model);
        }

        //  GET Evaluate Exam
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

            // Fetch answers with SelectedOptionID for MCQs
            var cmd2 = new MySqlCommand(@"
        SELECT q.QuestionText, q.QuestionType, a.AnswerText, a.AnswerID, a.MarksAwarded, a.OptionID AS SelectedOptionID, q.QuestionID
        FROM answer a
        JOIN question q ON a.QuestionID = q.QuestionID
        WHERE a.SubmissionID = @subId", con);
            cmd2.Parameters.AddWithValue("@subId", submissionId);

            using var reader2 = cmd2.ExecuteReader();
            model.Answers = new List<AnswerEvaluation>();
            while (reader2.Read())
            {
                var answer = new AnswerEvaluation
                {
                    AnswerID = reader2.GetInt32(reader2.GetOrdinal("AnswerID")),
                    QuestionText = reader2.GetString(reader2.GetOrdinal("QuestionText")),
                    AnswerText = reader2.IsDBNull(reader2.GetOrdinal("AnswerText"))
                                    ? ""
                                    : reader2.GetString(reader2.GetOrdinal("AnswerText")),
                    MarksAwarded = reader2.IsDBNull(reader2.GetOrdinal("MarksAwarded"))
                                    ? (int?)null
                                    : reader2.GetInt32(reader2.GetOrdinal("MarksAwarded")),
                    QuestionType = reader2.GetString(reader2.GetOrdinal("QuestionType")),
                    SelectedOptionID = reader2.IsDBNull(reader2.GetOrdinal("SelectedOptionID"))
                                        ? (int?)null
                                        : reader2.GetInt32(reader2.GetOrdinal("SelectedOptionID")),
                    Options = new List<MCQOptionModel>()
                };

                if (answer.QuestionType == "MCQ")
                {
                    using var con2 = new MySqlConnection(_connectionString);
                    con2.Open();

                    using var optCmd = new MySqlCommand(@"
                SELECT OptionID, OptionText, IsCorrect
                FROM optiontable
                WHERE QuestionID = @qid", con2);
                    optCmd.Parameters.AddWithValue("@qid", reader2.GetInt32(reader2.GetOrdinal("QuestionID")));

                    using var optReader = optCmd.ExecuteReader();
                    while (optReader.Read())
                    {
                        answer.Options.Add(new MCQOptionModel
                        {
                            OptionID = optReader.GetInt32(optReader.GetOrdinal("OptionID")),
                            OptionText = optReader.GetString(optReader.GetOrdinal("OptionText")),
                        });
                    }
                }

                model.Answers.Add(answer);
            }

            return View("Evaluate", model);
        }


        // POST Evaluate Exam
        [HttpPost]
        public IActionResult Evaluate(EvaluateExamViewModel model)
        {
            if (model.Answers == null || !model.Answers.Any())
            {
                TempData["Error"] = "No answers found for evaluation.";
                return RedirectToAction("PendingEvaluations");
            }
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

            TempData["EvaluateSuccess"] = "Evaluation saved successfully!";
            return RedirectToAction("PendingEvaluations");
        }
        public IActionResult Revaluation(int submissionId)
        {
            var model = new RevaluationViewModel();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            // Fetch basic submission info
            var cmd = new MySqlCommand(@"SELECT s.SubmissionID,e.exam_title,u.first_name AS LearnerName, 
    e.max_marks, 
    r.marks_obtained, 
    e.exam_date
FROM examsubmission s
JOIN exam e ON s.ExamID = e.exam_id
JOIN user u ON s.user_id = u.user_id
JOIN examresult r ON s.ExamID = r.exam_id AND s.user_id = r.user_id
WHERE s.SubmissionID = @sid;", con);

            cmd.Parameters.AddWithValue("@sid", submissionId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                model.SubmissionID = reader.GetInt32("SubmissionID");
                model.ExamTitle = reader.GetString("exam_title");
                model.LearnerName = reader.GetString("LearnerName");
                model.MaxMarks = reader.GetInt32("max_marks");
                model.ObtainedMarks = reader.GetInt32("marks_obtained");
                model.ExamDate = reader.GetDateTime("exam_date");
            }
            reader.Close();

            // Fetch per question marks
            model.Questions = new List<QuestionEvaluationViewModel>();
            var qCmd = new MySqlCommand(@"
        SELECT q.QuestionID, q.QuestionText
FROM question q
JOIN exam e ON q.ExamID = e.exam_id
JOIN examsubmission s ON s.ExamID = e.exam_id
WHERE s.SubmissionID = @sid;", con);

            qCmd.Parameters.AddWithValue("@sid", submissionId);

            using var qReader = qCmd.ExecuteReader();
            while (qReader.Read())
            {
                model.Questions.Add(new QuestionEvaluationViewModel
                {
                    QuestionID = qReader.GetInt32("QuestionID"),
                    QuestionText = qReader.GetString("QuestionText")
                });
            }

            return View(model);
        }
        [HttpPost]
        public IActionResult Revaluation(RevaluationViewModel model)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            // 1. Update each answer's MarksAwarded
            foreach (var q in model.Questions)
            {
                var cmd = new MySqlCommand(@"
            UPDATE answer
            SET MarksAwarded = @marks, updated_on = NOW()
            WHERE SubmissionID = @sid AND QuestionID = @qid", con);

                cmd.Parameters.AddWithValue("@marks", q.ObtainedMarks);
                cmd.Parameters.AddWithValue("@sid", model.SubmissionID);
                cmd.Parameters.AddWithValue("@qid", q.QuestionID);

                cmd.ExecuteNonQuery();
            }

            // 2. Calculate total marks after revaluation
            var totalMarks = model.Questions.Sum(q => q.ObtainedMarks);

            // 3. Update examsubmission with new TotalMarks
            var totalCmd = new MySqlCommand(@"
        UPDATE examsubmission
        SET TotalMarks = @total, updated_on = NOW()
        WHERE SubmissionID = @sid", con);

            totalCmd.Parameters.AddWithValue("@total", totalMarks);
            totalCmd.Parameters.AddWithValue("@sid", model.SubmissionID);
            totalCmd.ExecuteNonQuery();

            // 4. Get ExamID, LearnerID, Passing Marks
            var examInfoCmd = new MySqlCommand(@"
        SELECT e.exam_id, e.passing_marks, es.user_id 
        FROM exam e
        JOIN examsubmission es ON e.exam_id = es.ExamID
        WHERE es.SubmissionID = @sid", con);

            examInfoCmd.Parameters.AddWithValue("@sid", model.SubmissionID);

            int examId = 0, passingMarks = 0, learnerId = 0;
            using (var reader = examInfoCmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    examId = reader.GetInt32("exam_id");
                    passingMarks = reader.GetInt32("passing_marks");
                    learnerId = reader.GetInt32("user_id");
                }
            }

            // 5. Update examresult
            var passed = totalMarks >= passingMarks ? 1 : 0;

            var resultCmd = new MySqlCommand(@"
        UPDATE examresult
        SET marks_obtained = @total, passed = @passed, updated_on = NOW()
        WHERE exam_id = @examId AND user_id = @learnerId", con);

            resultCmd.Parameters.AddWithValue("@total", totalMarks);
            resultCmd.Parameters.AddWithValue("@passed", passed);
            resultCmd.Parameters.AddWithValue("@examId", examId);
            resultCmd.Parameters.AddWithValue("@learnerId", learnerId);

            resultCmd.ExecuteNonQuery();

            TempData["RevaluateSuccess"] = "Revaluation completed successfully! Result updated.";
            return RedirectToAction("PendingEvaluations");
        }


    }
}