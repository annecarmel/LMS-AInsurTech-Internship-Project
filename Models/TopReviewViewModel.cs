namespace ASSNlearningManagementSystem.Models
{
    public class TopReviewViewModel
    {
        public string StudentName { get; set; }  // or just "Name" if used consistently
        public string CourseName { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
    }
}
