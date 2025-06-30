using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Models
{
    public class ExamCreationViewModel
    {
        public string ExamTitle { get; set; }
        public int TopicId { get; set; }
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public int EvaluatorId { get; set; }
        public int InstructorId { get; set; }

        public List<Course> Courses { get; set; }
        public List<Topic> Topics { get; set; }
        public List<User> Users { get; set; }
    }
}
