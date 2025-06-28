using System;
using System.ComponentModel.DataAnnotations;

namespace ASSNlearningManagementSystem.Models
{
    public class Session
    {
        public int SessionID { get; set; }

        
        public int CourseID { get; set; }

        public string CourseName { get; set; }

        
        public int SyllabusID { get; set; }

        public string SyllabusName { get; set; }

       
        public int TopicID { get; set; }

        public string TopicName { get; set; }

       
        public int TrainerID { get; set; }

        public string TrainerName { get; set; }

        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

       
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

       
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Range(1, 1000, ErrorMessage = "Student count must be greater than 0")]
        public int StudentCount { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        public string Status { get; set; }

        public int EnrolledLearnerCount { get; set; }
    }
}
