namespace ASSNlearningManagementSystem.Models
{
    public class Question
    {
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } // MCQ or ShortAnswer
        public int Marks { get; set; }
        public List<Option> Options { get; set; } // only if MCQ
    }

}
