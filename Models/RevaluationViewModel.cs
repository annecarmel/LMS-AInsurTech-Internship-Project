namespace ASSNlearningManagementSystem.Models
{
    public class RevaluationViewModel
    {
        public int SubmissionID { get; set; }
        public string ExamTitle { get; set; }
        public string LearnerName { get; set; }
        public int MaxMarks { get; set; }
        public int ObtainedMarks { get; set; }
        public DateTime ExamDate { get; set; }
        public List<QuestionEvaluationViewModel> Questions { get; set; }
    }

    public class QuestionEvaluationViewModel
    {
        public int QuestionID { get; set; }
        public string QuestionText { get; set; }
        public int ObtainedMarks { get; set; }
    }

}
