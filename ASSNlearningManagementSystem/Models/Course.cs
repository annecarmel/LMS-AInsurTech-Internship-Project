using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Models
{
    public class Course
    {
        public int CourseID { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        // ✅ New: Collection of syllabuses
        public List<Syllabus> Syllabuses { get; set; } = new List<Syllabus>();
    }
}
