using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Models
{
    public class ExamViewModel
    {
        public Exam Exam { get; set; }

        public List<Course> Courses { get; set; }
        public List<Syllabus> Syllabuses { get; set; }
        public List<Topic> Topics { get; set; }
        public List<User> Instructors { get; set; }
        public List<User> Evaluators { get; set; }
    }
}
