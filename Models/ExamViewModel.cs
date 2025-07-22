using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Models
{
    public class ExamViewModel
    {
        public Exam Exam { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public string TopicName { get; set; } = string.Empty;

        public List<Course> Courses { get; set; }
        public List<Syllabus> Syllabuses { get; set; }
        public List<Topic> Topics { get; set; }
        public List<User> Instructors { get; set; }
        public List<User> Evaluators { get; set; }
    }
}
