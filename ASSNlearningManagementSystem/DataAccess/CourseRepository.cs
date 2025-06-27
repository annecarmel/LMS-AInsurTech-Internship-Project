using ASSNlearningManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.DataAccess
{
    public class CourseRepository
    {
        private readonly string _connectionString;

        public CourseRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ✅ Get all courses with nested syllabuses and topics
        public List<Course> GetAllCourses()
        {
            var courses = new List<Course>();

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string courseQuery = "SELECT * FROM course ORDER BY CourseID DESC";

                using (var cmd = new MySqlCommand(courseQuery, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new Course
                        {
                            CourseID = Convert.ToInt32(reader["CourseID"]),
                            CourseName = reader["CourseName"].ToString(),
                            Description = reader["Description"].ToString(),
                            CreatedOn = reader["created_on"] as DateTime?,
                            UpdatedOn = reader["updated_on"] as DateTime?,
                            CreatedBy = reader["created_by"]?.ToString(),
                            UpdatedBy = reader["updated_by"]?.ToString(),
                            Syllabuses = new List<Syllabus>()
                        });
                    }
                }

                foreach (var course in courses)
                {
                    string syllabusQuery = "SELECT * FROM syllabus WHERE CourseID = @courseId";
                    using (var syllabusCmd = new MySqlCommand(syllabusQuery, conn))
                    {
                        syllabusCmd.Parameters.AddWithValue("@courseId", course.CourseID);
                        using (var reader = syllabusCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var syllabus = new Syllabus
                                {
                                    SyllabusID = Convert.ToInt32(reader["SyllabusID"]),
                                    Title = reader["Syllabus_name"].ToString(),
                                    CourseID = course.CourseID,
                                    CreatedOn = reader["created_on"] as DateTime?,
                                    UpdatedOn = reader["updated_on"] as DateTime?,
                                    CreatedBy = reader["created_by"]?.ToString(),
                                    UpdatedBy = reader["updated_by"]?.ToString(),
                                    Topics = new List<Topic>()
                                };
                                course.Syllabuses.Add(syllabus);
                            }
                        }
                    }

                    foreach (var syllabus in course.Syllabuses)
                    {
                        string topicQuery = "SELECT * FROM topic WHERE SyllabusID = @syllabusId";
                        using (var topicCmd = new MySqlCommand(topicQuery, conn))
                        {
                            topicCmd.Parameters.AddWithValue("@syllabusId", syllabus.SyllabusID);
                            using (var reader = topicCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var topic = new Topic
                                    {
                                        TopicID = Convert.ToInt32(reader["TopicID"]),
                                        Title = reader["TopicName"].ToString(),
                                        DurationHours = Convert.ToInt32(reader["Duration"] ?? 0),
                                        Description = reader["Description"]?.ToString(),
                                        CreatedOn = reader["created_on"] as DateTime?,
                                        UpdatedOn = reader["updated_on"] as DateTime?,
                                        CreatedBy = reader["created_by"]?.ToString(),
                                        UpdatedBy = reader["updated_by"]?.ToString()
                                    };
                                    syllabus.Topics.Add(topic);
                                }
                            }
                        }
                    }
                }
            }

            return courses;
        }

        // ✅ Get course by ID
        public Course GetCourseById(int courseId)
        {
            Course course = null;

            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string courseQuery = "SELECT * FROM course WHERE CourseID = @id";

                using (var cmd = new MySqlCommand(courseQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@id", courseId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            course = new Course
                            {
                                CourseID = Convert.ToInt32(reader["CourseID"]),
                                CourseName = reader["CourseName"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreatedOn = reader["created_on"] as DateTime?,
                                UpdatedOn = reader["updated_on"] as DateTime?,
                                CreatedBy = reader["created_by"]?.ToString(),
                                UpdatedBy = reader["updated_by"]?.ToString(),
                                Syllabuses = new List<Syllabus>()
                            };
                        }
                    }
                }

                if (course != null)
                {
                    string syllabusQuery = "SELECT * FROM syllabus WHERE CourseID = @courseId";
                    using (var syllabusCmd = new MySqlCommand(syllabusQuery, conn))
                    {
                        syllabusCmd.Parameters.AddWithValue("@courseId", course.CourseID);
                        using (var reader = syllabusCmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var syllabus = new Syllabus
                                {
                                    SyllabusID = Convert.ToInt32(reader["SyllabusID"]),
                                    Title = reader["Syllabus_name"].ToString(),
                                    CourseID = course.CourseID,
                                    CreatedOn = reader["created_on"] as DateTime?,
                                    UpdatedOn = reader["updated_on"] as DateTime?,
                                    CreatedBy = reader["created_by"]?.ToString(),
                                    UpdatedBy = reader["updated_by"]?.ToString(),
                                    Topics = new List<Topic>()
                                };
                                course.Syllabuses.Add(syllabus);
                            }
                        }
                    }

                    foreach (var syllabus in course.Syllabuses)
                    {
                        string topicQuery = "SELECT * FROM topic WHERE SyllabusID = @syllabusId";
                        using (var topicCmd = new MySqlCommand(topicQuery, conn))
                        {
                            topicCmd.Parameters.AddWithValue("@syllabusId", syllabus.SyllabusID);
                            using (var reader = topicCmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var topic = new Topic
                                    {
                                        TopicID = Convert.ToInt32(reader["TopicID"]),
                                        Title = reader["TopicName"].ToString(),
                                        DurationHours = Convert.ToInt32(reader["Duration"] ?? 0),
                                        Description = reader["Description"]?.ToString(),
                                        CreatedOn = reader["created_on"] as DateTime?,
                                        UpdatedOn = reader["updated_on"] as DateTime?,
                                        CreatedBy = reader["created_by"]?.ToString(),
                                        UpdatedBy = reader["updated_by"]?.ToString()
                                    };
                                    syllabus.Topics.Add(topic);
                                }
                            }
                        }
                    }
                }
            }

            return course;
        }

        // ✅ Add course with nested syllabuses & topics
        public bool AddCourse(Course course)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string courseQuery = @"
                            INSERT INTO course (CourseName, Description, created_on, created_by)
                            VALUES (@name, @desc, @createdOn, @createdBy);
                            SELECT LAST_INSERT_ID();";

                        int newCourseId;
                        using (var cmd = new MySqlCommand(courseQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@name", course.CourseName);
                            cmd.Parameters.AddWithValue("@desc", course.Description);
                            cmd.Parameters.AddWithValue("@createdOn", DateTime.Now);
                            cmd.Parameters.AddWithValue("@createdBy", course.CreatedBy ?? "1");

                            newCourseId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        if (course.Syllabuses != null && course.Syllabuses.Count > 0)
                        {
                            foreach (var syllabus in course.Syllabuses)
                            {
                                string syllabusQuery = @"
                                    INSERT INTO syllabus (Syllabus_name, CourseID, created_on, created_by, updated_on, updated_by)
                                    VALUES (@title, @courseId, @createdOn, @createdBy, @updatedOn, @updatedBy);
                                    SELECT LAST_INSERT_ID();";

                                int newSyllabusId;
                                using (var syllabusCmd = new MySqlCommand(syllabusQuery, conn, transaction))
                                {
                                    syllabusCmd.Parameters.AddWithValue("@title", syllabus.Title);
                                    syllabusCmd.Parameters.AddWithValue("@courseId", newCourseId);
                                    syllabusCmd.Parameters.AddWithValue("@createdOn", DateTime.Now);
                                    syllabusCmd.Parameters.AddWithValue("@createdBy", course.CreatedBy ?? "1");
                                    syllabusCmd.Parameters.AddWithValue("@updatedOn", DateTime.Now);
                                    syllabusCmd.Parameters.AddWithValue("@updatedBy", course.CreatedBy ?? "1");

                                    newSyllabusId = Convert.ToInt32(syllabusCmd.ExecuteScalar());
                                }

                                if (syllabus.Topics != null && syllabus.Topics.Count > 0)
                                {
                                    foreach (var topic in syllabus.Topics)
                                    {
                                        string topicQuery = @"
                                            INSERT INTO topic (TopicName, Duration, Description, SyllabusID, created_on, created_by, updated_on, updated_by)
                                            VALUES (@title, @duration, @desc, @syllabusId, @createdOn, @createdBy, @updatedOn, @updatedBy);";

                                        using (var topicCmd = new MySqlCommand(topicQuery, conn, transaction))
                                        {
                                            topicCmd.Parameters.AddWithValue("@title", topic.Title);
                                            topicCmd.Parameters.AddWithValue("@duration", topic.DurationHours);
                                            topicCmd.Parameters.AddWithValue("@desc", topic.Description ?? "");
                                            topicCmd.Parameters.AddWithValue("@syllabusId", newSyllabusId);
                                            topicCmd.Parameters.AddWithValue("@createdOn", DateTime.Now);
                                            topicCmd.Parameters.AddWithValue("@createdBy", course.CreatedBy ?? "1");
                                            topicCmd.Parameters.AddWithValue("@updatedOn", DateTime.Now);
                                            topicCmd.Parameters.AddWithValue("@updatedBy", course.CreatedBy ?? "1");

                                            topicCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        // ✅ Update course with nested syllabuses & topics
        public bool UpdateCourse(Course course)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string courseQuery = @"
                            UPDATE course
                            SET CourseName = @name, Description = @desc, updated_on = @updatedOn, updated_by = @updatedBy
                            WHERE CourseID = @id";

                        using (var cmd = new MySqlCommand(courseQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@name", course.CourseName);
                            cmd.Parameters.AddWithValue("@desc", course.Description);
                            cmd.Parameters.AddWithValue("@updatedOn", DateTime.Now);
                            cmd.Parameters.AddWithValue("@updatedBy", course.UpdatedBy ?? "1");
                            cmd.Parameters.AddWithValue("@id", course.CourseID);
                            cmd.ExecuteNonQuery();
                        }

                        string deleteTopics = @"DELETE t FROM topic t JOIN syllabus s ON t.SyllabusID = s.SyllabusID WHERE s.CourseID = @courseId";
                        using (var cmd = new MySqlCommand(deleteTopics, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@courseId", course.CourseID);
                            cmd.ExecuteNonQuery();
                        }

                        string deleteSyllabus = @"DELETE FROM syllabus WHERE CourseID = @courseId";
                        using (var cmd = new MySqlCommand(deleteSyllabus, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@courseId", course.CourseID);
                            cmd.ExecuteNonQuery();
                        }

                        if (course.Syllabuses != null && course.Syllabuses.Count > 0)
                        {
                            foreach (var syllabus in course.Syllabuses)
                            {
                                string syllabusQuery = @"
                                    INSERT INTO syllabus (Syllabus_name, CourseID, created_on, created_by, updated_on, updated_by)
                                    VALUES (@title, @courseId, @createdOn, @createdBy, @updatedOn, @updatedBy);
                                    SELECT LAST_INSERT_ID();";

                                int newSyllabusId;
                                using (var syllabusCmd = new MySqlCommand(syllabusQuery, conn, transaction))
                                {
                                    syllabusCmd.Parameters.AddWithValue("@title", syllabus.Title);
                                    syllabusCmd.Parameters.AddWithValue("@courseId", course.CourseID);
                                    syllabusCmd.Parameters.AddWithValue("@createdOn", DateTime.Now);
                                    syllabusCmd.Parameters.AddWithValue("@createdBy", course.UpdatedBy ?? "1");
                                    syllabusCmd.Parameters.AddWithValue("@updatedOn", DateTime.Now);
                                    syllabusCmd.Parameters.AddWithValue("@updatedBy", course.UpdatedBy ?? "1");

                                    newSyllabusId = Convert.ToInt32(syllabusCmd.ExecuteScalar());
                                }

                                if (syllabus.Topics != null && syllabus.Topics.Count > 0)
                                {
                                    foreach (var topic in syllabus.Topics)
                                    {
                                        string topicQuery = @"
                                            INSERT INTO topic (TopicName, Duration, Description, SyllabusID, created_on, created_by, updated_on, updated_by)
                                            VALUES (@title, @duration, @desc, @syllabusId, @createdOn, @createdBy, @updatedOn, @updatedBy);";

                                        using (var topicCmd = new MySqlCommand(topicQuery, conn, transaction))
                                        {
                                            topicCmd.Parameters.AddWithValue("@title", topic.Title);
                                            topicCmd.Parameters.AddWithValue("@duration", topic.DurationHours);
                                            topicCmd.Parameters.AddWithValue("@desc", topic.Description ?? "");
                                            topicCmd.Parameters.AddWithValue("@syllabusId", newSyllabusId);
                                            topicCmd.Parameters.AddWithValue("@createdOn", DateTime.Now);
                                            topicCmd.Parameters.AddWithValue("@createdBy", course.UpdatedBy ?? "1");
                                            topicCmd.Parameters.AddWithValue("@updatedOn", DateTime.Now);
                                            topicCmd.Parameters.AddWithValue("@updatedBy", course.UpdatedBy ?? "1");

                                            topicCmd.ExecuteNonQuery();
                                        }
                                    }
                                }
                            }
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        // ✅ Delete course with cascade
        public bool DeleteCourse(int courseId)
        {
            using (var conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                using (var transaction = conn.BeginTransaction())
                {
                    try
                    {
                        string deleteTopics = @"DELETE t FROM topic t JOIN syllabus s ON t.SyllabusID = s.SyllabusID WHERE s.CourseID = @courseId";
                        using (var cmd = new MySqlCommand(deleteTopics, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@courseId", courseId);
                            cmd.ExecuteNonQuery();
                        }

                        string deleteSyllabus = @"DELETE FROM syllabus WHERE CourseID = @courseId";
                        using (var cmd = new MySqlCommand(deleteSyllabus, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@courseId", courseId);
                            cmd.ExecuteNonQuery();
                        }

                        string deleteCourse = @"DELETE FROM course WHERE CourseID = @courseId";
                        using (var cmd = new MySqlCommand(deleteCourse, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@courseId", courseId);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}
