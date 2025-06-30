namespace ASSNlearningManagementSystem.Models
{
    public class EvaluateExamViewModel
    {
        public int SubmissionID { get; set; }
        public string ExamTitle { get; set; }
        public string LearnerName { get; set; }
        public int PassingMarks { get; set; }
        public List<AnswerEvaluation> Answers { get; set; }
    }

    public class AnswerEvaluation
    {
        public int AnswerID { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }
        public int? MarksAwarded { get; set; }
    }
}
