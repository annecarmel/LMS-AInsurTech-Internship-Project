using ASSNlearningManagementSystem.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Repository
{
    public class RatingRepository
    {
        private readonly string _connectionString;

        public RatingRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        // ---------------------------
        // GET ALL RATINGS
        // ---------------------------
        public List<Rating> GetAllRatings()
        {
            List<Rating> ratings = new List<Rating>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    SELECT 
                      r.RatingID,
                      c.CourseName,
                      CONCAT(u.first_name, ' ', u.last_name) AS LearnerName,
                      r.Rating_Value,
                      r.Feedback,
                      r.created_by,
                      r.created_on,
                      r.updated_by,
                      r.updated_on
                    FROM 
                      rating r
                      LEFT JOIN course c ON r.CourseID = c.CourseID
                      LEFT JOIN user u ON r.user_id = u.user_id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ratings.Add(new Rating
                            {
                                RatingID = reader.GetInt32("RatingID"),
                                CourseName = reader["CourseName"]?.ToString(),
                                LearnerName = reader["LearnerName"]?.ToString(),
                                RatingValue = reader.GetInt32("Rating_Value"),
                                Feedback = reader["Feedback"]?.ToString(),
                                CreatedBy = reader["created_by"] as int?,
                                CreatedOn = reader["created_on"] as DateTime?,
                                UpdatedBy = reader["updated_by"] as int?,
                                UpdatedOn = reader["updated_on"] as DateTime?
                            });
                        }
                    }
                }
            }

            return ratings;
        }

        // ---------------------------
        // ADD NEW RATING
        // ---------------------------
        public void AddRating(Rating rating)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO rating 
                    (CourseID, user_id, Rating_Value, Feedback, created_by, created_on)
                    VALUES
                    (@CourseID, @UserID, @RatingValue, @Feedback, @CreatedBy, @CreatedOn)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", rating.CourseID);
                    cmd.Parameters.AddWithValue("@UserID", rating.UserID);
                    cmd.Parameters.AddWithValue("@RatingValue", rating.RatingValue);
                    cmd.Parameters.AddWithValue("@Feedback", rating.Feedback);
                    cmd.Parameters.AddWithValue("@CreatedBy", rating.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedOn", DateTime.Now);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ---------------------------
        // DELETE RATING BY ID
        // ---------------------------
        public void DeleteRating(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM rating WHERE RatingID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ---------------------------
        // UPDATE RATING (Optional)
        // ---------------------------
        public void UpdateRating(Rating rating)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    UPDATE rating SET
                        CourseID = @CourseID,
                        user_id = @UserID,
                        Rating_Value = @RatingValue,
                        Feedback = @Feedback,
                        updated_by = @UpdatedBy,
                        updated_on = @UpdatedOn
                    WHERE RatingID = @RatingID";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CourseID", rating.CourseID);
                    cmd.Parameters.AddWithValue("@UserID", rating.UserID);
                    cmd.Parameters.AddWithValue("@RatingValue", rating.RatingValue);
                    cmd.Parameters.AddWithValue("@Feedback", rating.Feedback);
                    cmd.Parameters.AddWithValue("@UpdatedBy", rating.UpdatedBy);
                    cmd.Parameters.AddWithValue("@UpdatedOn", DateTime.Now);
                    cmd.Parameters.AddWithValue("@RatingID", rating.RatingID);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
