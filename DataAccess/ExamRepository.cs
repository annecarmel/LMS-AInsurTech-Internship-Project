using ASSNlearningManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Repository
{
    public class ExamRepository
    {
        private readonly string _connectionString;

        public ExamRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Get all exams with full display info
        public List<Exam> GetAllExams()
        {
            var exams = new List<Exam>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                SELECT 
                    e.exam_id,
                    e.exam_title,
                    e.exam_date,
                    e.start_time,
                    e.duration,
                    e.max_marks,
                    e.passing_marks,
                    e.evaluator_id,
                    e.instructor_id,
                    e.created_by,
                    e.created_on,
                    e.updated_by,
                    e.updated_on,
                    u1.full_name AS EvaluatorName,
                    u2.full_name AS InstructorName,
                    t.TopicID,
                    t.TopicName,
                    s.SyllabusID,
                    s.Syllabus_name AS SyllabusName,
                    c.CourseID,
                    c.CourseName AS CourseName
                FROM exam e
                JOIN topic t ON e.topic_id = t.TopicID
                JOIN syllabus s ON t.SyllabusID = s.SyllabusID
                JOIN course c ON s.CourseID = c.CourseID
                LEFT JOIN user u1 ON e.evaluator_id = u1.user_id
                LEFT JOIN user u2 ON e.instructor_id = u2.user_id
                ORDER BY e.exam_date DESC;";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var examDate = reader.GetDateTime("exam_date");
                        var startTime = reader.IsDBNull(reader.GetOrdinal("start_time")) ? (TimeSpan?)null : reader.GetTimeSpan("start_time");


                        // ✅ Dynamic status logic
                        string status;
                        var now = DateTime.Now;
                        var examDateTime = startTime.HasValue ? examDate.Date + startTime.Value : examDate.Date;

                        if (examDateTime > now)
                            status = "Scheduled";
                        else if (examDateTime <= now && examDateTime.AddMinutes(reader.GetInt32("duration")) >= now)
                            status = "Ongoing";
                        else
                            status = "Completed";

                        exams.Add(new Exam
                        {
                            ExamId = reader.GetInt32("exam_id"),
                            ExamTitle = reader.GetString("exam_title"),
                            ExamDate = examDate,
                            StartTime = startTime,
                            Duration = reader.GetInt32("duration"),
                            MaxMarks = reader.GetInt32("max_marks"),
                            PassingMarks = reader.GetInt32("passing_marks"),
                            EvaluatorId = reader.IsDBNull(reader.GetOrdinal("evaluator_id")) ? null : reader.GetInt32("evaluator_id"),
                            InstructorId = reader.IsDBNull(reader.GetOrdinal("instructor_id")) ? null : reader.GetInt32("instructor_id"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("created_by")) ? null : reader.GetInt32("created_by"),
                            CreatedOn = reader.IsDBNull(reader.GetOrdinal("created_on")) ? null : reader.GetDateTime("created_on"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("updated_by")) ? null : reader.GetInt32("updated_by"),
                            UpdatedOn = reader.IsDBNull(reader.GetOrdinal("updated_on")) ? null : reader.GetDateTime("updated_on"),
                            EvaluatorName = reader.IsDBNull(reader.GetOrdinal("EvaluatorName")) ? null : reader.GetString("EvaluatorName"),
                            InstructorName = reader.IsDBNull(reader.GetOrdinal("InstructorName")) ? null : reader.GetString("InstructorName"),
                            TopicId = reader.GetInt32("TopicID"),
                            TopicName = reader.IsDBNull(reader.GetOrdinal("TopicName")) ? null : reader.GetString("TopicName"),
                            SyllabusId = reader.GetInt32("SyllabusID"),
                            SyllabusName = reader.GetString("SyllabusName"),
                            CourseId = reader.GetInt32("CourseID"),
                            CourseName = reader.GetString("CourseName"),
                            Status = status
                        });
                    }
                }
            }

            return exams;
        }

        public void InsertExam(Exam exam)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                INSERT INTO exam 
                    (exam_title, topic_id, exam_date, start_time, duration, status, max_marks, passing_marks, evaluator_id, instructor_id, created_by, created_on) 
                VALUES 
                    (@ExamTitle, @TopicId, @ExamDate, @StartTime, @Duration, @Status, @MaxMarks, @PassingMarks, @EvaluatorId, @InstructorId, @CreatedBy, @CreatedOn);";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExamTitle", exam.ExamTitle);
                    command.Parameters.AddWithValue("@TopicId", exam.TopicId);
                    command.Parameters.AddWithValue("@ExamDate", exam.ExamDate);
                    command.Parameters.AddWithValue("@StartTime", exam.StartTime.HasValue ? exam.StartTime.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Duration", exam.Duration);
                    command.Parameters.AddWithValue("@Status", exam.Status ?? "Scheduled");
                    command.Parameters.AddWithValue("@MaxMarks", exam.MaxMarks);
                    command.Parameters.AddWithValue("@PassingMarks", exam.PassingMarks);
                    command.Parameters.AddWithValue("@EvaluatorId", exam.EvaluatorId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@InstructorId", exam.InstructorId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedBy", exam.CreatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    command.ExecuteNonQuery();
                }
            }
        }

        public Exam GetExamById(int examId)
        {
            Exam exam = null;

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                SELECT 
                    e.exam_id,
                    e.exam_title,
                    e.exam_date,
                    e.start_time,
                    e.duration,
                    e.status,
                    e.max_marks,
                    e.passing_marks,
                    e.evaluator_id,
                    e.instructor_id,
                    t.TopicID,
                    t.TopicName,
                    s.SyllabusID,
                    s.Syllabus_name,
                    c.CourseID,
                    c.CourseName
                FROM exam e
                JOIN topic t ON e.topic_id = t.TopicID
                JOIN syllabus s ON t.SyllabusID = s.SyllabusID
                JOIN course c ON s.CourseID = c.CourseID
                WHERE e.exam_id = @ExamId;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExamId", examId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            exam = new Exam
                            {
                                ExamId = reader.GetInt32("exam_id"),
                                ExamTitle = reader.GetString("exam_title"),
                                ExamDate = reader.GetDateTime("exam_date"),
                                StartTime = reader.IsDBNull(reader.GetOrdinal("start_time")) ? null : reader.GetTimeSpan("start_time"),
                                Duration = reader.GetInt32("duration"),
                                Status = reader.GetString("status"),
                                MaxMarks = reader.GetInt32("max_marks"),
                                PassingMarks = reader.GetInt32("passing_marks"),
                                EvaluatorId = reader.IsDBNull(reader.GetOrdinal("evaluator_id")) ? null : reader.GetInt32("evaluator_id"),
                                InstructorId = reader.IsDBNull(reader.GetOrdinal("instructor_id")) ? null : reader.GetInt32("instructor_id"),
                                TopicId = reader.GetInt32("TopicID"),
                                TopicName = reader.GetString("TopicName"),
                                SyllabusId = reader.GetInt32("SyllabusID"),
                                SyllabusName = reader.GetString("Syllabus_name"),
                                CourseId = reader.GetInt32("CourseID"),
                                CourseName = reader.GetString("CourseName")
                            };
                        }
                    }
                }
            }

            return exam;
        }

        public void UpdateExam(Exam exam)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                UPDATE exam 
                SET 
                    exam_title = @ExamTitle,
                    topic_id = @TopicId,
                    exam_date = @ExamDate,
                    start_time = @StartTime,
                    duration = @Duration,
                    status = @Status,
                    max_marks = @MaxMarks,
                    passing_marks = @PassingMarks,
                    evaluator_id = @EvaluatorId,
                    instructor_id = @InstructorId,
                    updated_by = @UpdatedBy,
                    updated_on = @UpdatedOn
                WHERE exam_id = @ExamId;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExamTitle", exam.ExamTitle);
                    command.Parameters.AddWithValue("@TopicId", exam.TopicId);
                    command.Parameters.AddWithValue("@ExamDate", exam.ExamDate);
                    command.Parameters.AddWithValue("@StartTime", exam.StartTime.HasValue ? exam.StartTime.Value : (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Duration", exam.Duration);
                    command.Parameters.AddWithValue("@Status", exam.Status ?? "Scheduled");
                    command.Parameters.AddWithValue("@MaxMarks", exam.MaxMarks);
                    command.Parameters.AddWithValue("@PassingMarks", exam.PassingMarks);
                    command.Parameters.AddWithValue("@EvaluatorId", exam.EvaluatorId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@InstructorId", exam.InstructorId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedBy", exam.UpdatedBy ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                    command.Parameters.AddWithValue("@ExamId", exam.ExamId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteExam(int examId)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"DELETE FROM exam WHERE exam_id = @ExamId;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ExamId", examId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public List<Course> GetCourses()
        {
            var courses = new List<Course>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT CourseID, CourseName FROM course;";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseID = reader.GetInt32("CourseID"),
                            CourseName = reader.GetString("CourseName")
                        });
                    }
                }
            }

            return courses;
        }

        public List<Syllabus> GetSyllabusesByCourseId(int courseId)
        {
            var syllabuses = new List<Syllabus>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT SyllabusID, Syllabus_name FROM syllabus WHERE CourseID = @CourseID;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CourseID", courseId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            syllabuses.Add(new Syllabus
                            {
                                SyllabusID = reader.GetInt32("SyllabusID"),
                                Title = reader.GetString("Syllabus_name")
                            });
                        }
                    }
                }
            }

            return syllabuses;
        }

        public List<Topic> GetTopicsBySyllabusId(int syllabusId)
        {
            var topics = new List<Topic>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT TopicID, TopicName FROM topic WHERE SyllabusID = @SyllabusID;";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SyllabusID", syllabusId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            topics.Add(new Topic
                            {
                                TopicID = reader.GetInt32("TopicID"),
                                Title = reader.GetString("TopicName")
                            });
                        }
                    }
                }
            }

            return topics;
        }

        public List<User> GetTrainers()
        {
            var trainers = new List<User>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT user_id, full_name FROM user WHERE role_id = 2;";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trainers.Add(new User
                        {
                            UserId = reader.GetInt32("user_id"),
                            FullName = reader.IsDBNull(reader.GetOrdinal("full_name")) ? null : reader.GetString("full_name")
                        });
                    }
                }
            }

            return trainers;
        }
    }
}
