namespace ASSNlearningManagementSystem.Models
{
    public class FeedbackViewModel
    {
        public int RatingId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string FeedbackText { get; set; }  = string.Empty;
    }
}
