using System;

namespace ASSNlearningManagementSystem.Models
{
    public class ExamResult
    {
        public int ResultID { get; set; }
        public int ExamID { get; set; }
        public string ExamTitle { get; set; }
        public int PassingMarks { get; set; } // ✅ NEW: Passmark for the exam

        public int UserID { get; set; }
        public string LearnerName { get; set; }
        public int MarksObtained { get; set; }
        public bool Passed { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
