namespace ASSNlearningManagementSystem.Models
{
    public class AssignedExamViewModel
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public string CourseName { get; set; }
        public string TopicName { get; set; }
        public string InstructorName { get; set; }
        public string EvaluatorName { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public int Duration { get; set; }
        public DateTime ExamDate { get; set; } 
        public string Status { get; set; }
    }
}
