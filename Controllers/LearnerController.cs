using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using static ASSNlearningManagementSystem.Models.ViewLearnerModel;

namespace ASSNlearningManagementSystem.Controllers
{
    public class LearnerController : Controller
    {
        private readonly string? _connectionString;

        public LearnerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private int GetCurrentUserId()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not logged in or session expired.");
            }
            return userId.Value;
        }

        public IActionResult Dashboard()
        {
            var enrollments = GetCourseEnrollments();
            int userId = GetCurrentUserId();
            var results = GetResultsForCurrentUser(userId);
            var popularCourses = GetPopularCourses();

            var courses = GetCourses();



            // ✅ Count of exams to attend
            int examCount = 0;
            using (var con = new MySqlConnection(_connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand(@"
            SELECT COUNT(*) FROM exam e
            JOIN topic t ON e.topic_id = t.TopicID
            JOIN syllabus s ON t.SyllabusID = s.SyllabusID
            JOIN course c ON s.CourseID = c.CourseID
            JOIN courseenrollments ce ON ce.course_id = c.CourseID
            WHERE ce.user_id = @userId
              AND (ce.completion_status IS NULL OR ce.completion_status = 'Enrolled')", con);

                cmd.Parameters.AddWithValue("@userId", userId);
                examCount = Convert.ToInt32(cmd.ExecuteScalar());
            }

            var viewModel = new ViewLearnerModel
            {
                Enrollments = enrollments,
                Courses = courses,
                Results = results,
                ExamCount = examCount,
                PopularCourses = popularCourses
            };

            return View(viewModel);
        }


        private List<CourseEnrollment> GetCourseEnrollments()
        {
            var list = new List<CourseEnrollment>();
            int userId = GetCurrentUserId();

            using var con = new MySqlConnection(_connectionString);
            string query = @"
        SELECT e.enrollment_id, e.course_id, e.user_id, e.completion_status, 
               e.enrolled_on, e.completed_on, c.CourseName, r.feedback, r.Rating_Value
        FROM courseenrollments e
        JOIN Course c ON e.course_id = c.CourseID
        LEFT JOIN rating r ON e.course_id = r.CourseID AND e.user_id = r.user_id
        WHERE e.user_id = @UserID 
              AND (e.completion_status IS NULL OR e.completion_status = 'Enrolled')";

            var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@UserID", userId);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CourseEnrollment
                {
                    EnrollmentId = Convert.ToInt32(reader["enrollment_id"]),
                    CourseId = Convert.ToInt32(reader["course_id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    EnrolledOn = Convert.ToDateTime(reader["enrolled_on"]),
                    CompletedOn = reader.IsDBNull(reader.GetOrdinal("completed_on"))
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["completed_on"]),
                    CourseName = reader["CourseName"].ToString(),
                    Feedback = reader.IsDBNull(reader.GetOrdinal("feedback"))
                        ? string.Empty
                        : reader["feedback"].ToString(),
                    Rating = reader.IsDBNull(reader.GetOrdinal("Rating_Value"))
                        ? (int?)null
                        : Convert.ToInt32(reader["Rating_Value"])
                });
            }

            return list;
        }




        private List<LearnerCourseViewModel> GetCourses()
        {
            var list = new List<LearnerCourseViewModel>();
            using var con = new MySqlConnection(_connectionString);
            string query = @"SELECT CourseID, CourseName, Description FROM Course";
            var cmd = new MySqlCommand(query, con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new LearnerCourseViewModel
                {
                    CourseId = Convert.ToInt32(reader["CourseID"]),
                    CourseName = reader["CourseName"]?.ToString() ?? "",
                    Description = reader["Description"]?.ToString() ?? "",
                    Duration = "N/A" // Or get actual duration if needed
                });
            }
            return list;
        }


        public IActionResult CourseDetails(int courseId)
        {
            var courseDetails = GetCourseDetails(courseId);

            int userId = GetCurrentUserId();
            var enrollments = GetCourseEnrollments().FindAll(e => e.UserId == userId);


            courseDetails.UserEnrollments = enrollments;

            return View("CourseDetailsLearner", courseDetails);
        }

        private CourseLearnerViewModel GetCourseDetails(int courseId)
        {
            var course = new CourseLearnerViewModel
            {
                CourseId = courseId,
                Syllabuses = new List<SyllabusViewModel>()
            };

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            // Get course name
            using (var cmd = new MySqlCommand("SELECT CourseName FROM Course WHERE CourseID = @CourseID", con))
            {
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                    course.CourseName = reader["CourseName"].ToString();
            }

            // Get syllabuses, topics, trainers, session dates
            using (var cmd = new MySqlCommand(@"
                SELECT s.SyllabusID, s.Syllabus_name, t.TopicID, t.TopicName, u.username AS TrainerName, sess.session_date
                FROM syllabus s
                LEFT JOIN topic t ON s.SyllabusID = t.SyllabusID
                LEFT JOIN user u ON t.instructor_id = u.user_id
                LEFT JOIN session sess ON sess.course_id = s.CourseID AND sess.topic_id = t.TopicID
                WHERE s.CourseID = @CourseID
                ORDER BY s.SyllabusID", con))
            {
                cmd.Parameters.AddWithValue("@CourseID", courseId);
                using var reader = cmd.ExecuteReader();

                Dictionary<int, SyllabusViewModel> syllabusDict = new();

                while (reader.Read())
                {
                    int syllabusId = Convert.ToInt32(reader["SyllabusID"]);
                    string syllabusName = reader["Syllabus_name"]?.ToString() ?? "N/A";


                    if (!syllabusDict.ContainsKey(syllabusId))
                    {
                        syllabusDict[syllabusId] = new SyllabusViewModel
                        {
                            SyllabusID = syllabusId,
                            SyllabusName = syllabusName,
                            Topics = new List<TopicViewModel>()
                        };
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("TopicName")))
                    {
                        syllabusDict[syllabusId].Topics.Add(new TopicViewModel
                        {
                            TopicName = reader["TopicName"].ToString(),
                            TrainerName = reader["TrainerName"]?.ToString() ?? "N/A",
                            SessionDate = reader.IsDBNull(reader.GetOrdinal("session_date"))
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["session_date"])
                        });
                    }
                }

                course.Syllabuses = new List<SyllabusViewModel>(syllabusDict.Values);
            }

            return course;
        }

        [HttpPost]
        public IActionResult Enroll(int courseId)
        {
            int userId = GetCurrentUserId();
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
                INSERT INTO courseenrollments (course_id, user_id, completion_status, enrolled_on)
                VALUES (@CourseID, @UserID, 'Enrolled', NOW())", con);
            cmd.Parameters.AddWithValue("@CourseID", courseId);
            cmd.Parameters.AddWithValue("@UserID", userId);

            cmd.ExecuteNonQuery();

            TempData["EnrollmentSuccess"] = "Enrollment successful!";
            return RedirectToAction("MyEnrollments");
        }

        public IActionResult MyEnrollments()
        {
            int userId = GetCurrentUserId();

            var allEnrollments = GetCourseEnrollments();
            var enrollmentsForCurrentUser = allEnrollments
                .FindAll(e => e.UserId == userId);

            return View("MyEnrollments", enrollmentsForCurrentUser);
        }


        public IActionResult CourseList()
        {
            int userId = GetCurrentUserId(); // get current logged-in user ID

            var viewModel = new ViewLearnerModel
            {
                CourseDetails = GetAllCourseDetails(),
                Enrollments = GetCourseEnrollments().FindAll(e => e.UserId == userId) // get current user's enrollments
            };

            return View("courselearner", viewModel);
        }


        private List<CourseLearnerViewModel> GetAllCourseDetails()
        {
            var courseDetailsList = new List<CourseLearnerViewModel>();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var courseCmd = new MySqlCommand("SELECT CourseID, CourseName FROM Course", con);
            using var courseReader = courseCmd.ExecuteReader();
            var courses = new List<(int Id, string Name)>();
            while (courseReader.Read())
            {
                courses.Add((
                    Convert.ToInt32(courseReader["CourseID"]),
                    courseReader["CourseName"].ToString() ?? "N/A"
                ));
            }
            courseReader.Close();

            foreach (var course in courses)
            {
                var courseVm = new CourseLearnerViewModel
                {
                    CourseId = course.Id,
                    CourseName = course.Name,
                    Syllabuses = new List<SyllabusViewModel>()
                };

                using var syllabusCmd = new MySqlCommand(@"
                    SELECT s.SyllabusID, s.Syllabus_name, t.TopicName, u.username AS TrainerName
                    FROM syllabus s
                    LEFT JOIN topic t ON s.SyllabusID = t.SyllabusID
                    LEFT JOIN user u ON t.instructor_id = u.user_id
                    WHERE s.CourseID = @CourseID
                    ORDER BY s.SyllabusID", con);

                syllabusCmd.Parameters.AddWithValue("@CourseID", course.Id);

                using var syllabusReader = syllabusCmd.ExecuteReader();

                Dictionary<int, SyllabusViewModel> syllabusDict = new();

                while (syllabusReader.Read())
                {
                    int syllabusId = Convert.ToInt32(syllabusReader["SyllabusID"]);
                    string syllabusName = syllabusReader["Syllabus_name"]?.ToString() ?? "N/A";


                    if (!syllabusDict.ContainsKey(syllabusId))
                    {
                        syllabusDict[syllabusId] = new SyllabusViewModel
                        {
                            SyllabusID = syllabusId,
                            SyllabusName = syllabusName,
                            Topics = new List<TopicViewModel>()
                        };
                    }

                    if (!syllabusReader.IsDBNull(syllabusReader.GetOrdinal("TopicName")))
                    {
                        syllabusDict[syllabusId].Topics.Add(new TopicViewModel
                        {
                            TopicName = syllabusReader["TopicName"].ToString(),
                            TrainerName = syllabusReader["TrainerName"]?.ToString() ?? "N/A"
                        });
                    }
                }
                syllabusReader.Close();

                courseVm.Syllabuses = new List<SyllabusViewModel>(syllabusDict.Values);
                courseDetailsList.Add(courseVm);
            }

            return courseDetailsList;
        }
        [HttpPost]

        [HttpPost]
        public IActionResult Unenroll(int enrollmentId)
        {
            int userId = GetCurrentUserId();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
        UPDATE courseenrollments 
        SET completion_status = 'Unenrolled',
            updated_by = @UserID,
            updated_on = NOW()
        WHERE enrollment_id = @EnrollmentId AND user_id = @UserID", con);

            cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
            cmd.Parameters.AddWithValue("@UserID", userId);

            cmd.ExecuteNonQuery();

            TempData["Message"] = "Successfully unenrolled.";
            return RedirectToAction("MyEnrollments");
        }

        [HttpPost]
        public IActionResult SubmitFeedback(int enrollmentId, string feedback, int rating)
        {
            int userId = GetCurrentUserId();


            int courseId;
            using (var con = new MySqlConnection(_connectionString))
            {
                con.Open();
                var cmd = new MySqlCommand("SELECT course_id FROM courseenrollments WHERE enrollment_id = @EnrollmentId", con);
                cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    TempData["Error"] = "Enrollment not found.";
                    return RedirectToAction("MyEnrollments");
                }
                courseId = Convert.ToInt32(result);
            }


            using (var con = new MySqlConnection(_connectionString))
            {
                con.Open();

                var checkCmd = new MySqlCommand(@"
            SELECT COUNT(*) FROM rating WHERE CourseID = @CourseID AND user_id = @UserID", con);
                checkCmd.Parameters.AddWithValue("@CourseID", courseId);
                checkCmd.Parameters.AddWithValue("@UserID", userId);

                long count = (long)checkCmd.ExecuteScalar();

                if (count == 0)
                {
                    var insertCmd = new MySqlCommand(@"
                INSERT INTO rating (CourseID, user_id, feedback, Rating_Value, created_by, created_on) 
                VALUES (@CourseID, @UserID, @Feedback, @Rating, @UserID, NOW())", con);
                    insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                    insertCmd.Parameters.AddWithValue("@UserID", userId);
                    insertCmd.Parameters.AddWithValue("@Feedback", feedback);
                    insertCmd.Parameters.AddWithValue("@Rating", rating);
                    insertCmd.ExecuteNonQuery();
                }
                else
                {
                    var updateCmd = new MySqlCommand(@"
                UPDATE rating 
                SET feedback = @Feedback, Rating_Value = @Rating, updated_by = @UserID, updated_on = NOW() 
                WHERE CourseID = @CourseID AND user_id = @UserID", con);
                    updateCmd.Parameters.AddWithValue("@Feedback", feedback);
                    updateCmd.Parameters.AddWithValue("@Rating", rating);
                    updateCmd.Parameters.AddWithValue("@CourseID", courseId);
                    updateCmd.Parameters.AddWithValue("@UserID", userId);
                    updateCmd.ExecuteNonQuery();
                }
            }

            TempData["Message"] = "Feedback & rating submitted successfully.";
            return RedirectToAction("MyEnrollments");
        }

        public IActionResult TimeTable()
        {
            List<TimeTableRow> timetable = new();

            try
            {
                using var con = new MySqlConnection(_connectionString);
                con.Open();

                int learnerId = GetCurrentUserId();

                var cmd = new MySqlCommand(@"
                    SELECT c.CourseName, s.Syllabus_name, t.TopicName,
       sess.session_date, sess.start_time, sess.end_time,
       u.username AS TrainerName
FROM session sess
LEFT JOIN topic t ON sess.topic_id = t.TopicID
LEFT JOIN syllabus s ON t.SyllabusID = s.SyllabusID
JOIN Course c ON sess.course_id = c.CourseID
JOIN user u ON sess.instructor_id = u.user_id
JOIN courseenrollments ce ON ce.course_id = c.CourseID AND ce.user_id = @learnerId
WHERE (ce.completion_status IS NULL OR ce.completion_status = 'Enrolled')
ORDER BY sess.session_date, sess.start_time;


            ", con);

                cmd.Parameters.AddWithValue("@learnerId", learnerId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    timetable.Add(new TimeTableRow
                    {
                        CourseName = reader["CourseName"].ToString(),
                        SyllabusName = reader["Syllabus_name"].ToString(),
                        TopicName = reader["TopicName"].ToString(),
                        SessionDate = reader.IsDBNull(reader.GetOrdinal("session_date")) ? (DateTime?)null : Convert.ToDateTime(reader["session_date"]),
                        StartTime = reader.IsDBNull(reader.GetOrdinal("start_time")) ? (TimeSpan?)null : reader.GetTimeSpan(reader.GetOrdinal("start_time")),
                        EndTime = reader.IsDBNull(reader.GetOrdinal("end_time")) ? (TimeSpan?)null : reader.GetTimeSpan(reader.GetOrdinal("end_time")),
                        TrainerName = reader["TrainerName"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching timetable: {ex.Message}");
                TempData["Error"] = "Unable to load timetable.";
            }

            return View(timetable);
        }


        public IActionResult ExamListing()
        {
            List<LearnerExamViewModel> exams = new List<LearnerExamViewModel>();


            int? learnerIdNullable = HttpContext.Session.GetInt32("UserId");
            if (!learnerIdNullable.HasValue)
            {
                throw new UnauthorizedAccessException("User not logged in or session expired.");
            }
            int learnerId = learnerIdNullable.Value;

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            string query = @"
SELECT e.exam_id, e.exam_title, e.duration, e.max_marks, e.passing_marks, t.TopicName
FROM exam e
JOIN topic t ON e.topic_id = t.TopicID
JOIN syllabus s ON t.SyllabusID = s.SyllabusID
JOIN course c ON s.CourseID = c.CourseID
JOIN courseenrollments ce ON ce.course_id = c.CourseID AND ce.user_id = @learnerId
WHERE (ce.completion_status IS NULL OR ce.completion_status = 'Enrolled')
AND NOT EXISTS (
    SELECT 1 FROM examsubmission es
    WHERE es.ExamID = e.exam_id 
    AND es.user_id = ce.user_id
    AND es.SubmissionDate IS NOT NULL
    AND es.created_on >= ce.enrolled_on  -- ❗ Only block if submitted *after this enrollment*
)";


            using var cmd = new MySqlCommand(query, con);
            cmd.Parameters.AddWithValue("@learnerId", learnerId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                exams.Add(new LearnerExamViewModel
                {
                    ExamId = Convert.ToInt32(reader["exam_id"]),
                    ExamTitle = reader["exam_title"].ToString(),
                    Duration = Convert.ToInt32(reader["duration"]),
                    MaxMarks = Convert.ToInt32(reader["max_marks"]),
                    PassingMarks = Convert.ToInt32(reader["passing_marks"]),
                    TopicName = reader["TopicName"].ToString()
                });

            }

            return View(exams);
        }


        [HttpGet]
        public IActionResult AttendExam(int examId)
        {
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            List<ExamQuestionViewModel> questions = LoadExamQuestions(examId, con);

            ViewBag.ExamID = examId;
            ViewBag.UserID = GetCurrentUserId();

            return View(questions);
        }
        private List<ExamQuestionViewModel> LoadExamQuestions(int examId, MySqlConnection con)
        {
            var questions = new List<ExamQuestionViewModel>();
            var cmd = new MySqlCommand(@"
SELECT q.QuestionID, q.QuestionText, q.Marks, q.QuestionType, 
       o.OptionID, o.OptionText
FROM question q
LEFT JOIN optiontable o ON q.QuestionID = o.QuestionID
WHERE q.ExamID = @examId
ORDER BY q.QuestionID, o.OptionID", con);

            cmd.Parameters.AddWithValue("@examId", examId);

            using var reader = cmd.ExecuteReader();
            var questionDict = new Dictionary<int, ExamQuestionViewModel>();

            while (reader.Read())
            {
                int questionId = reader.GetInt32("QuestionID");

                if (!questionDict.ContainsKey(questionId))
                {
                    questionDict[questionId] = new ExamQuestionViewModel
                    {
                        QuestionId = questionId,
                        QuestionText = reader["QuestionText"].ToString(),
                        Marks = reader.GetInt32("Marks"),
                        QuestionType = reader["QuestionType"].ToString(),
                        Options = new List<OptionViewModel>()
                    };
                }

                if (!reader.IsDBNull(reader.GetOrdinal("OptionID")))
                {
                    questionDict[questionId].Options.Add(new OptionViewModel
                    {
                        OptionId = reader.GetInt32("OptionID"),
                        OptionText = reader["OptionText"].ToString()
                    });
                }
            }

            return questionDict.Values.ToList();
        }


        [HttpPost]
        public IActionResult AttendExam(int examId, List<AnswerSubmissionModel> answers)
        {
            int userId = GetCurrentUserId();
            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var submissionCmd = new MySqlCommand(@"
INSERT INTO examsubmission (ExamID, user_id, SubmissionDate, created_by, created_on)
VALUES (@ExamID, @UserID, NOW(), @UserID, NOW());
SELECT LAST_INSERT_ID();", con);
            submissionCmd.Parameters.AddWithValue("@ExamID", examId);
            submissionCmd.Parameters.AddWithValue("@UserID", userId);

            int submissionId = Convert.ToInt32(submissionCmd.ExecuteScalar());

            foreach (var answer in answers)
            {
                var cmd = new MySqlCommand(@"
INSERT INTO answer (SubmissionID, QuestionID, OptionID, AnswerText, created_by, created_on)
VALUES (@SubmissionID, @QuestionID, @OptionID, @AnswerText, @UserID, NOW())", con);

                cmd.Parameters.AddWithValue("@SubmissionID", submissionId);
                cmd.Parameters.AddWithValue("@QuestionID", answer.QuestionId);
                cmd.Parameters.AddWithValue("@OptionID", (object?)answer.SelectedOptionId ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@AnswerText", (object?)answer.AnswerText ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UserID", userId);

                cmd.ExecuteNonQuery();
            }

            List<ExamQuestionViewModel> questions = LoadExamQuestions(examId, con);

            ViewBag.ExamID = examId;
            ViewBag.UserID = userId;
            ViewBag.SuccessMessage = "Exam submitted successfully!";

            return View(questions);
        }
        private List<ResultViewModel> GetResultsForCurrentUser(int userId)
        {
            var results = new List<ResultViewModel>();

            using var con = new MySqlConnection(_connectionString);
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

            using var reader = cmd.ExecuteReader();
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

            return results;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                using var con = new MySqlConnection(_connectionString);
                con.Open();

                var cmd = new MySqlCommand(@"
            SELECT u.username, r.role_name
            FROM user u
            JOIN userrole r ON u.role_id = r.role_id
            WHERE u.user_id = @UserId", con);

                cmd.Parameters.AddWithValue("@UserId", userId);

                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    ViewBag.UserName = reader["username"]?.ToString() ?? "Unknown User";
                    ViewBag.UserRole = reader["role_name"]?.ToString() ?? "Unknown Role";
                }
            }
        }
        private Dictionary<string, int> GetPopularCourses()
        {
            var popularCourses = new Dictionary<string, int>();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
        SELECT c.CourseName, COUNT(*) AS EnrollmentCount
        FROM courseenrollments ce
        JOIN Course c ON ce.course_id = c.CourseID
        WHERE ce.completion_status IS NULL OR ce.completion_status = 'Enrolled'
        GROUP BY c.CourseName
        ORDER BY EnrollmentCount DESC
        LIMIT 5;", con);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())

            {
                string courseName = reader["CourseName"].ToString();
                int count = Convert.ToInt32(reader["EnrollmentCount"]);
                popularCourses[courseName] = count;
            }

            return popularCourses;
        }





    }
}