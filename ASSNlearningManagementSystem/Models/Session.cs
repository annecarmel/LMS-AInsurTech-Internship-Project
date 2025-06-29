using System;
using System.ComponentModel.DataAnnotations;

namespace ASSNlearningManagementSystem.Models
{
    public class Session
    {
        public int SessionID { get; set; }

        [Required]
        public int CourseID { get; set; }

        public string CourseName { get; set; }

        [Required]
        public int SyllabusID { get; set; }

        public string SyllabusName { get; set; }

        [Required]
        public int TopicID { get; set; }

        public string TopicName { get; set; }

        [Required]
        public int TrainerID { get; set; }

        public string TrainerName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime SessionDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        public int EnrolledLearnerCount { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }

        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }

        // DO NOT add [Required] here because it's calculated automatically
        public string Status { get; set; }
    }
}
