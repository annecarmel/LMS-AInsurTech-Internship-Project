namespace ASSNlearningManagementSystem.Models
{
    public class PendingEvaluationViewModel
    {
        public int SubmissionID { get; set; }
        public string ExamTitle { get; set; }
        public string LearnerName { get; set; }
        public DateTime SubmissionDate { get; set; }
    }
    public class CompletedEvaluationViewModel
    {
        public int SubmissionID { get; set; }
        public string ExamTitle { get; set; }
        public string LearnerName { get; set; }
        public int MaxMarks { get; set; }
        public int ObtainedMarks { get; set; }
        public DateTime ExamDate { get; set; }
    }
    public class EvaluationPageViewModel
    {
        public List<PendingEvaluationViewModel> Pending { get; set; } = new List<PendingEvaluationViewModel>();
        public List<CompletedEvaluationViewModel> Completed { get; set; } = new List<CompletedEvaluationViewModel>();
    }


}
