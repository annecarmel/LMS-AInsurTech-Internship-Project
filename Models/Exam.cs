
namespace ASSNlearningManagementSystem.Models
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }

        public int? CourseId { get; set; }          // 🆕 Add this
        public int? SyllabusId { get; set; }        // 🆕 Add this
        public int? TopicId { get; set; }

        public DateTime ExamDate { get; set; }
        public TimeSpan? StartTime { get; set; }    // ✅ Already exists

        public int Duration { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }

        public int? InstructorId { get; set; }
        public int? EvaluatorId { get; set; }

        public string Status { get; set; }

        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        // For display:
        public string CourseName { get; set; }
        public string SyllabusName { get; set; }
        public string TopicName { get; set; }
        public string InstructorName { get; set; }
        public string EvaluatorName { get; set; }
    }
}