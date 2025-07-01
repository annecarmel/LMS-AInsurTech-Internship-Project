using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using ASSNlearningManagementSystem.Models;

namespace ASSNlearningManagementSystem.Repository
{
    public class SessionRepository
    {
        private readonly string connectionString;

        public SessionRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public List<Session> GetAllSessions()
        {
            List<Session> sessions = new List<Session>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT 
                        s.session_id, 
                        s.course_id, 
                        c.CourseName, 
                        s.syllabus_id, 
                        sy.Syllabus_name, 
                        s.topic_id, 
                        t.TopicName, 
                        s.instructor_id, 
                        u.first_name AS TrainerName,
                        s.session_date, 
                        s.start_time, 
                        s.end_time,
                        s.created_by, 
                        s.created_on, 
                        s.updated_by, 
                        s.updated_on,
                        IFNULL(enrolls.EnrolledCount, 0) AS EnrolledCount
                    FROM session s
                    LEFT JOIN course c ON s.course_id = c.CourseID
                    LEFT JOIN syllabus sy ON s.syllabus_id = sy.SyllabusID
                    LEFT JOIN topic t ON s.topic_id = t.TopicID
                    LEFT JOIN user u ON s.instructor_id = u.user_id
                    LEFT JOIN (
                        SELECT course_id, COUNT(*) AS EnrolledCount
                        FROM courseenrollments
                        GROUP BY course_id
                    ) enrolls ON enrolls.course_id = s.course_id;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var session = new Session
                        {
                            SessionID = Convert.ToInt32(reader["session_id"]),
                            CourseID = reader["course_id"] != DBNull.Value ? Convert.ToInt32(reader["course_id"]) : 0,
                            CourseName = reader["CourseName"]?.ToString(),
                            SyllabusID = reader["syllabus_id"] != DBNull.Value ? Convert.ToInt32(reader["syllabus_id"]) : 0,
                            SyllabusName = reader["Syllabus_name"]?.ToString(),
                            TopicID = reader["topic_id"] != DBNull.Value ? Convert.ToInt32(reader["topic_id"]) : 0,
                            TopicName = reader["TopicName"]?.ToString(),
                            TrainerID = reader["instructor_id"] != DBNull.Value ? Convert.ToInt32(reader["instructor_id"]) : 0,
                            TrainerName = reader["TrainerName"]?.ToString(),
                            SessionDate = reader["session_date"] != DBNull.Value ? Convert.ToDateTime(reader["session_date"]) : DateTime.MinValue,
                            StartTime = reader["start_time"] != DBNull.Value ? (TimeSpan)reader["start_time"] : TimeSpan.Zero,
                            EndTime = reader["end_time"] != DBNull.Value ? (TimeSpan)reader["end_time"] : TimeSpan.Zero,
                            CreatedBy = reader["created_by"] != DBNull.Value ? Convert.ToInt32(reader["created_by"]) : 0,
                            CreatedOn = reader["created_on"] != DBNull.Value ? Convert.ToDateTime(reader["created_on"]) : DateTime.MinValue,
                            UpdatedBy = reader["updated_by"] != DBNull.Value ? Convert.ToInt32(reader["updated_by"]) : 0,
                            UpdatedOn = reader["updated_on"] != DBNull.Value ? Convert.ToDateTime(reader["updated_on"]) : DateTime.MinValue,
                            Status = CalculateSessionStatus(reader["session_date"], reader["start_time"], reader["end_time"]),
                            EnrolledLearnerCount = reader["EnrolledCount"] != DBNull.Value ? Convert.ToInt32(reader["EnrolledCount"]) : 0
                        };

                        sessions.Add(session);
                    }
                }
            }

            return sessions;
        }

        private string CalculateSessionStatus(object sessionDateObj, object startTimeObj, object endTimeObj)
        {
            if (sessionDateObj == DBNull.Value || startTimeObj == DBNull.Value || endTimeObj == DBNull.Value)
                return "Unknown";

            DateTime sessionDate = Convert.ToDateTime(sessionDateObj);
            TimeSpan startTime = (TimeSpan)startTimeObj;
            TimeSpan endTime = (TimeSpan)endTimeObj;

            DateTime startDateTime = sessionDate.Date + startTime;
            DateTime endDateTime = sessionDate.Date + endTime;
            DateTime now = DateTime.Now;

            if (now < startDateTime)
                return "Scheduled";
            else if (now >= startDateTime && now <= endDateTime)
                return "Ongoing";
            else
                return "Completed";
        }

        public void SaveSession(Session session)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand cmd;

                if (session.SessionID > 0)
                {
                    cmd = new MySqlCommand(
                        @"UPDATE session 
                          SET course_id = @course_id,
                              syllabus_id = @syllabus_id,
                              topic_id = @topic_id,
                              instructor_id = @instructor_id,
                              session_date = @session_date,
                              start_time = @start_time,
                              end_time = @end_time,
                              updated_by = @updated_by,
                              updated_on = NOW()
                          WHERE session_id = @session_id", connection);
                }
                else
                {
                    cmd = new MySqlCommand(
                        @"INSERT INTO session 
                          (course_id, syllabus_id, topic_id, instructor_id, session_date, start_time, end_time, created_by, created_on)
                          VALUES
                          (@course_id, @syllabus_id, @topic_id, @instructor_id, @session_date, @start_time, @end_time, @created_by, NOW())", connection);
                }

                cmd.Parameters.AddWithValue("@course_id", session.CourseID);
                cmd.Parameters.AddWithValue("@syllabus_id", session.SyllabusID);
                cmd.Parameters.AddWithValue("@topic_id", session.TopicID);
                cmd.Parameters.AddWithValue("@instructor_id", session.TrainerID);
                cmd.Parameters.AddWithValue("@session_date", session.SessionDate);
                cmd.Parameters.AddWithValue("@start_time", session.StartTime);
                cmd.Parameters.AddWithValue("@end_time", session.EndTime);

                if (session.SessionID > 0)
                {
                    cmd.Parameters.AddWithValue("@updated_by", session.UpdatedBy);
                    cmd.Parameters.AddWithValue("@session_id", session.SessionID);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@created_by", session.CreatedBy);
                }

                cmd.ExecuteNonQuery();

                if (session.SessionID == 0)
                {
                    session.SessionID = (int)cmd.LastInsertedId;
                }
            }
        }

        public void DeleteSession(int sessionId)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(
                    @"DELETE FROM session WHERE session_id = @session_id", connection);
                cmd.Parameters.AddWithValue("@session_id", sessionId);
                cmd.ExecuteNonQuery();
            }
        }

        public Session GetSessionById(int sessionId)
        {
            Session session = null;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT * FROM session WHERE session_id = @session_id";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@session_id", sessionId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        session = new Session
                        {
                            SessionID = Convert.ToInt32(reader["session_id"]),
                            CourseID = reader["course_id"] != DBNull.Value ? Convert.ToInt32(reader["course_id"]) : 0,
                            SyllabusID = reader["syllabus_id"] != DBNull.Value ? Convert.ToInt32(reader["syllabus_id"]) : 0,
                            TopicID = reader["topic_id"] != DBNull.Value ? Convert.ToInt32(reader["topic_id"]) : 0,
                            TrainerID = reader["instructor_id"] != DBNull.Value ? Convert.ToInt32(reader["instructor_id"]) : 0,
                            SessionDate = reader["session_date"] != DBNull.Value ? Convert.ToDateTime(reader["session_date"]) : DateTime.MinValue,
                            StartTime = reader["start_time"] != DBNull.Value ? (TimeSpan)reader["start_time"] : TimeSpan.Zero,
                            EndTime = reader["end_time"] != DBNull.Value ? (TimeSpan)reader["end_time"] : TimeSpan.Zero,
                            CreatedBy = reader["created_by"] != DBNull.Value ? Convert.ToInt32(reader["created_by"]) : 0,
                            CreatedOn = reader["created_on"] != DBNull.Value ? Convert.ToDateTime(reader["created_on"]) : DateTime.MinValue,
                            UpdatedBy = reader["updated_by"] != DBNull.Value ? Convert.ToInt32(reader["updated_by"]) : 0,
                            UpdatedOn = reader["updated_on"] != DBNull.Value ? Convert.ToDateTime(reader["updated_on"]) : DateTime.MinValue
                        };
                    }
                }
            }

            return session;
        }

        public List<KeyValuePair<int, string>> GetTrainers()
        {
            var trainers = new List<KeyValuePair<int, string>>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT user_id, first_name FROM user WHERE role_id = 2";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        trainers.Add(new KeyValuePair<int, string>(
                            Convert.ToInt32(reader["user_id"]),
                            reader["first_name"].ToString()
                        ));
                    }
                }
            }

            return trainers;
        }

        public List<KeyValuePair<int, string>> GetCourses()
        {
            var courses = new List<KeyValuePair<int, string>>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT CourseID, CourseName FROM course";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        courses.Add(new KeyValuePair<int, string>(
                            Convert.ToInt32(reader["CourseID"]),
                            reader["CourseName"].ToString()
                        ));
                    }
                }
            }

            return courses;
        }

        public List<KeyValuePair<int, string>> GetTopics()
        {
            var topics = new List<KeyValuePair<int, string>>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TopicID, TopicName FROM topic";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        topics.Add(new KeyValuePair<int, string>(
                            Convert.ToInt32(reader["TopicID"]),
                            reader["TopicName"].ToString()
                        ));
                    }
                }
            }

            return topics;
        }

        public List<KeyValuePair<int, string>> GetSyllabuses()
        {
            var syllabuses = new List<KeyValuePair<int, string>>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT SyllabusID, Syllabus_name FROM syllabus";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        syllabuses.Add(new KeyValuePair<int, string>(
                            Convert.ToInt32(reader["SyllabusID"]),
                            reader["Syllabus_name"].ToString()
                        ));
                    }
                }
            }

            return syllabuses;
        }

        public List<KeyValuePair<int, string>> GetSyllabusesByCourseId(int courseId)
        {
            var syllabuses = new List<KeyValuePair<int, string>>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT SyllabusID, Syllabus_name FROM syllabus WHERE CourseID = @courseId";
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@courseId", courseId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    syllabuses.Add(new KeyValuePair<int, string>(
                        Convert.ToInt32(reader["SyllabusID"]),
                        reader["Syllabus_name"].ToString()
                    ));
                }
            }
            return syllabuses;
        }

        public List<KeyValuePair<int, string>> GetTopicsBySyllabusId(int syllabusId)
        {
            var topics = new List<KeyValuePair<int, string>>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TopicID, TopicName FROM topic WHERE SyllabusID = @syllabusId";
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@syllabusId", syllabusId);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    topics.Add(new KeyValuePair<int, string>(
                        Convert.ToInt32(reader["TopicID"]),
                        reader["TopicName"].ToString()
                    ));
                }
            }
            return topics;
        }

        public int GetEnrolledLearnerCountByCourseId(int courseId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM courseenrollments WHERE course_id = @courseId";
                using var cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@courseId", courseId);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // ✅ NEW METHODS REQUIRED BY THE CONTROLLER
        public string GetCourseNameById(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = new MySqlCommand("SELECT CourseName FROM course WHERE CourseID = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        public string GetSyllabusNameById(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = new MySqlCommand("SELECT Syllabus_name FROM syllabus WHERE SyllabusID = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        public string GetTopicNameById(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = new MySqlCommand("SELECT TopicName FROM topic WHERE TopicID = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        public string GetTrainerNameById(int id)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();
            var cmd = new MySqlCommand("SELECT first_name FROM user WHERE user_id = @id", connection);
            cmd.Parameters.AddWithValue("@id", id);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }
    }
}
