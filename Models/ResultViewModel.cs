
namespace ASSNlearningManagementSystem.Models
{
    public class ResultViewModel
    {
        // Properties for the result view model
        // Add properties for exam details and result status

        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int MaxMarks { get; set; }
        public int MarksObtained { get; set; }
        public int PassingMarks { get; set; }   // Add this
        public string Status { get; set; }       // Add this
    }
}