using ASSNlearningManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Repository
{
    public class ExamResultRepository
    {
        private readonly string _connectionString;

        public ExamResultRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<ExamResult> GetAllExamResults()
        {
            var results = new List<ExamResult>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        er.result_id, 
                        er.exam_id, 
                        e.exam_title, 
                        e.passing_marks,  -- ✅ Pull pass mark
                        er.user_id,
                        CONCAT(u.first_name, ' ', u.last_name) AS LearnerName,
                        er.marks_obtained,
                        er.created_by,
                        er.created_on,
                        er.updated_by,
                        er.updated_on
                    FROM examresult er
                    LEFT JOIN exam e ON er.exam_id = e.exam_id
                    LEFT JOIN user u ON er.user_id = u.user_id";

                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int marksObtained = Convert.ToInt32(reader["marks_obtained"]);
                            int passingMarks = Convert.ToInt32(reader["passing_marks"]);

                            results.Add(new ExamResult
                            {
                                ResultID = Convert.ToInt32(reader["result_id"]),
                                ExamID = Convert.ToInt32(reader["exam_id"]),
                                ExamTitle = reader["exam_title"]?.ToString(),
                                PassingMarks = passingMarks,
                                UserID = Convert.ToInt32(reader["user_id"]),
                                LearnerName = reader["LearnerName"]?.ToString(),
                                MarksObtained = marksObtained,
                                Passed = marksObtained >= passingMarks, // ✅ Calculate here
                                CreatedBy = reader["created_by"] != DBNull.Value ? Convert.ToInt32(reader["created_by"]) : 0,
                                CreatedOn = reader["created_on"] != DBNull.Value ? Convert.ToDateTime(reader["created_on"]) : DateTime.MinValue,
                                UpdatedBy = reader["updated_by"] != DBNull.Value ? Convert.ToInt32(reader["updated_by"]) : 0,
                                UpdatedOn = reader["updated_on"] != DBNull.Value ? Convert.ToDateTime(reader["updated_on"]) : DateTime.MinValue
                            });
                        }
                    }
                }
            }

            return results;
        }
    }
}
