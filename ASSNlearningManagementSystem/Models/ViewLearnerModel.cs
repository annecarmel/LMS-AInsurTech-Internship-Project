namespace ASSNlearningManagementSystem.Models
{
    public class ViewLearnerModel
    {
        public List<CourseEnrollment> Enrollments { get; set; } = new();
        public List<Course> Courses { get; set; } = new();
        public List<CourseLearnerViewModel> CourseDetails { get; set; } = new();
        public List<ResultViewModel>? Results { get; set; }
        public double? AverageRating { get; set; }


    }

    public class CourseEnrollment
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        // public string CompletionStatus { get; set; } 
        public DateTime EnrolledOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Feedback { get; set; } = string.Empty;
        public int? Rating { get; set; }


    }

    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
    }

    public class CourseLearnerViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public List<SyllabusViewModel> Syllabuses { get; set; } = new();
        public List<CourseEnrollment> UserEnrollments { get; set; }

    }

    public class SyllabusViewModel
    {
        public int SyllabusID { get; set; }
        public string SyllabusName { get; set; } = string.Empty;
        public List<TopicViewModel> Topics { get; set; } = new();
    }

    public class TopicViewModel
    {
        public string TopicName { get; set; } = string.Empty;
        public string TrainerName { get; set; } = string.Empty;
        public DateTime? SessionDate { get; set; }
    }

    public class TimeTableRow
    {
        public string CourseName { get; set; } = string.Empty;
        public string SyllabusName { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public DateTime? SessionDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string TrainerName { get; set; } = string.Empty;
    }

    public class ExamViewModel
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int Duration { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public string TopicName { get; set; } = string.Empty;
    }

    public class ExamQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int Marks { get; set; }
        public string QuestionType { get; set; } = "MCQ"; 
        public List<OptionViewModel> Options { get; set; } = new();
    }


    public class OptionViewModel
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; } = string.Empty;
    }

    public class AnswerSubmissionModel
    {
        public int QuestionId { get; set; }
        public int? SelectedOptionId { get; set; }
        public string? AnswerText { get; set; }
    }

}