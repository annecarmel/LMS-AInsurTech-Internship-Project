using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace ASSNlearningManagementSystem.Controllers
{
    public class LearnerController : Controller
    {
        private readonly string _connectionString;

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
            var courses = GetCourses();

            var viewModel = new ViewLearnerModel
            {
                Enrollments = enrollments,
                Courses = courses
            };

            return View(viewModel);
        }

        private List<CourseEnrollment> GetCourseEnrollments()
        {
            var list = new List<CourseEnrollment>();

            using var con = new MySqlConnection(_connectionString);
            string query = @"
        SELECT e.enrollment_id, e.course_id, e.user_id, e.completion_status, 
               e.enrolled_on, e.completed_on, c.CourseName, r.feedback
        FROM courseenrollments e
        JOIN Course c ON e.course_id = c.CourseID
        LEFT JOIN rating r ON e.course_id = r.CourseID AND e.user_id = r.user_id";

            var cmd = new MySqlCommand(query, con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new CourseEnrollment
                {
                    EnrollmentId = Convert.ToInt32(reader["enrollment_id"]),
                    CourseId = Convert.ToInt32(reader["course_id"]),
                    UserId = Convert.ToInt32(reader["user_id"]),
                    CompletionStatus = reader["completion_status"].ToString(),
                    EnrolledOn = Convert.ToDateTime(reader["enrolled_on"]),
                    CompletedOn = reader.IsDBNull(reader.GetOrdinal("completed_on"))
                        ? (DateTime?)null
                        : Convert.ToDateTime(reader["completed_on"]),
                    CourseName = reader["CourseName"].ToString(), // ✅ ensure CourseName is populated
                    Feedback = reader.IsDBNull(reader.GetOrdinal("feedback"))
                        ? string.Empty
                        : reader["feedback"].ToString()
                });
            }

            return list;
        }



        private List<Course> GetCourses()
        {
            var list = new List<Course>();
            using var con = new MySqlConnection(_connectionString);
            string query = @"SELECT CourseID, CourseName, Description FROM Course";
            var cmd = new MySqlCommand(query, con);
            con.Open();

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Course
                {
                    CourseId = Convert.ToInt32(reader["CourseID"]),
                    CourseName = reader["CourseName"].ToString(),
                    Description = reader["Description"].ToString(),
                    Duration = "N/A" // Placeholder, since duration is not in your DB
                });
            }
            return list;
        }

        public IActionResult CourseDetails(int courseId)
        {
            var courseDetails = GetCourseDetails(courseId);
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
                    string syllabusName = reader["Syllabus_name"].ToString();

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
            var viewModel = new ViewLearnerModel
            {
                CourseDetails = GetAllCourseDetails()
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
                    courseReader["CourseName"].ToString()
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
                    string syllabusName = syllabusReader["Syllabus_name"].ToString();

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
        public IActionResult Unenroll(int enrollmentId)
        {
            int userId = GetCurrentUserId();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand("DELETE FROM courseenrollments WHERE enrollment_id = @EnrollmentId AND user_id = @UserID", con);
            cmd.Parameters.AddWithValue("@EnrollmentId", enrollmentId);
            cmd.Parameters.AddWithValue("@UserID", userId);

            int rows = cmd.ExecuteNonQuery();

            if (rows > 0)
            {
                TempData["Message"] = "Successfully unenrolled.";
            }
            else
            {
                TempData["Error"] = "Unable to unenroll. Please try again.";
            }

            return RedirectToAction("MyEnrollments");
        }
        [HttpPost]
        public IActionResult SubmitFeedback(int enrollmentId, string feedback)
        {
            int userId = GetCurrentUserId();

            // Get course_id from enrollment
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

            // Insert or update feedback in rating table
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
                INSERT INTO rating (CourseID, user_id, feedback) 
                VALUES (@CourseID, @UserID, @Feedback)", con);
                    insertCmd.Parameters.AddWithValue("@CourseID", courseId);
                    insertCmd.Parameters.AddWithValue("@UserID", userId);
                    insertCmd.Parameters.AddWithValue("@Feedback", feedback);
                    insertCmd.ExecuteNonQuery();
                }
                else
                {
                    var updateCmd = new MySqlCommand(@"
                UPDATE rating SET feedback = @Feedback 
                WHERE CourseID = @CourseID AND user_id = @UserID", con);
                    updateCmd.Parameters.AddWithValue("@Feedback", feedback);
                    updateCmd.Parameters.AddWithValue("@CourseID", courseId);
                    updateCmd.Parameters.AddWithValue("@UserID", userId);
                    updateCmd.ExecuteNonQuery();
                }
            }

            TempData["Message"] = "Feedback submitted successfully.";
            return RedirectToAction("MyEnrollments");
        }

        public IActionResult TimeTable()
        {
            List<TimeTableRow> timetable = new();

            try
            {
                using var con = new MySqlConnection(_connectionString);
                con.Open();

                int learnerId = (int)HttpContext.Session.GetInt32("UserId"); // use your session key

                var cmd = new MySqlCommand(@"
    SELECT c.CourseName, s.Syllabus_name, t.TopicName,
           sess.session_date, sess.start_time, sess.end_time,
           u.username AS TrainerName
    FROM session sess
    JOIN topic t ON sess.topic_id = t.TopicID
    JOIN syllabus s ON t.SyllabusID = s.SyllabusID
    JOIN Course c ON sess.course_id = c.CourseID
    JOIN user u ON t.instructor_id = u.user_id
    JOIN courseenrollments ce ON ce.course_id = c.CourseID
    WHERE ce.user_id = @learnerId
    ORDER BY sess.session_date, sess.start_time", con);

                cmd.Parameters.AddWithValue("@learnerId", learnerId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    timetable.Add(new TimeTableRow
                    {
                        CourseName = reader["CourseName"].ToString(),
                        SyllabusName = reader["Syllabus_name"].ToString(),
                        TopicName = reader["TopicName"].ToString(),
                        SessionDate = reader.IsDBNull(reader.GetOrdinal("session_date"))
                            ? (DateTime?)null
                            : Convert.ToDateTime(reader["session_date"]),
                        StartTime = reader.IsDBNull(reader.GetOrdinal("start_time"))
                            ? (TimeSpan?)null
                            : reader.GetTimeSpan(reader.GetOrdinal("start_time")),
                        EndTime = reader.IsDBNull(reader.GetOrdinal("end_time"))
                            ? (TimeSpan?)null
                            : reader.GetTimeSpan(reader.GetOrdinal("end_time")),
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
            List<ExamViewModel> exams = new List<ExamViewModel>();

            int learnerId = (int)HttpContext.Session.GetInt32("UserId");

            using (var con = new MySqlConnection(_connectionString))
            {
                con.Open();
                string query = @"
            SELECT e.exam_id, e.exam_title, e.duration, e.max_marks, e.passing_marks, t.TopicName
            FROM exam e
            JOIN topic t ON e.topic_id = t.TopicID
            JOIN syllabus s ON t.SyllabusID = s.SyllabusID
            JOIN course c ON s.CourseID = c.CourseID
            JOIN courseenrollments ce ON ce.course_id = c.CourseID
            WHERE ce.user_id = @learnerId";

                using var cmd = new MySqlCommand(query, con);
                cmd.Parameters.AddWithValue("@learnerId", learnerId);

                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    exams.Add(new ExamViewModel
                    {
                        ExamId = Convert.ToInt32(reader["exam_id"]),
                        ExamTitle = reader["exam_title"].ToString(),
                        Duration = Convert.ToInt32(reader["duration"]),
                        MaxMarks = Convert.ToInt32(reader["max_marks"]),
                        PassingMarks = Convert.ToInt32(reader["passing_marks"]),
                        TopicName = reader["TopicName"].ToString()
                    });
                }
            }

            return View(exams);
        }


        public IActionResult AttendExam(int examId)
        {
            List<ExamQuestionViewModel> questions = new();

            using var con = new MySqlConnection(_connectionString);
            con.Open();

            var cmd = new MySqlCommand(@"
        SELECT q.QuestionID, q.QuestionText, q.Marks, o.OptionID, o.OptionText
        FROM question q
        LEFT JOIN optiontable o ON q.QuestionID = o.QuestionID
        WHERE q.ExamID = @examId
        ORDER BY q.QuestionID, o.OptionID", con);

            cmd.Parameters.AddWithValue("@examId", examId);

            using var reader = cmd.ExecuteReader();

            Dictionary<int, ExamQuestionViewModel> questionDict = new();

            while (reader.Read())
            {
                int questionId = reader.GetInt32("QuestionID");

                // If first time seeing this question, create and add it
                if (!questionDict.ContainsKey(questionId))
                {
                    questionDict[questionId] = new ExamQuestionViewModel
                    {
                        QuestionId = questionId,
                        QuestionText = reader["QuestionText"].ToString(),
                        Marks = reader.GetInt32("Marks"),
                        Options = new List<OptionViewModel>()
                    };
                }

                // Add option if exists (LEFT JOIN may give NULL option)
                if (!reader.IsDBNull(reader.GetOrdinal("OptionID")))
                {
                    questionDict[questionId].Options.Add(new OptionViewModel
                    {
                        OptionId = reader.GetInt32("OptionID"),
                        OptionText = reader["OptionText"].ToString()
                    });
                }
            }

            questions = questionDict.Values.ToList();
            return View(questions);
        }


    }





}
