namespace ASSNlearningManagementSystem.Models
{
    public class Rating
    {
        public int RatingID { get; set; }
        public int? CourseID { get; set; }
        public int? UserID { get; set; }
        public int RatingValue { get; set; }
        public string Feedback { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        // Additional for displaying joined info
        public string CourseName { get; set; }
        public string LearnerName { get; set; }
    }
}
