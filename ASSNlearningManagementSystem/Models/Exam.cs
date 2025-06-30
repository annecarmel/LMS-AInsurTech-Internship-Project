namespace ASSNlearningManagementSystem.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int TopicId { get; set; }
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        public string Status { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public int EvaluatorId { get; set; }
        public int InstructorId { get; set; }
    }

}
