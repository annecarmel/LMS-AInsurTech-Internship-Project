namespace ASSNlearningManagementSystem.Models
{
    public class Question
    {
        public int QuestionID { get; set; }
        public int ExamID { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; }
        public int Marks { get; set; }
        public List<QuestionOption> Options { get; set; }
    }

    public class QuestionOption
    {
        public int OptionID { get; set; }
        public string OptionText { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsDeleted { get; set; }

    }

}
