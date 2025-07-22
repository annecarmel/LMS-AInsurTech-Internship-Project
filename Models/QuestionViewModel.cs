using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Models
{
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public int ExamId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionType { get; set; } // "MCQ" or "ShortAnswer"
        public int Marks { get; set; }

        public List<MCQOptionModel> Options { get; set; } = new List<MCQOptionModel>();
    }
}
