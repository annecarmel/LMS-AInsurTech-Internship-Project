namespace ASSNlearningManagementSystem.Models
{
    public class ViewLearnerModel
    {
        public List<CourseEnrollment> Enrollments { get; set; }
        public List<Course> Courses { get; set; }

        // ✅ Optional: only if you plan to load all course details at once
        public List<CourseLearnerViewModel> CourseDetails { get; set; }
     
    }

    public class CourseEnrollment
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public int UserId { get; set; }
        public string CourseName { get; set; }
        public string CompletionStatus { get; set; }
        public DateTime EnrolledOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Feedback { get; set; }

    }

    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
    }

    public class CourseLearnerViewModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public List<SyllabusViewModel> Syllabuses { get; set; }
    }

    public class SyllabusViewModel
    {
        public int SyllabusID { get; set; }
        public string SyllabusName { get; set; }
        public List<TopicViewModel> Topics { get; set; }
    }

    public class TopicViewModel
    {
        public string TopicName { get; set; }
        public string TrainerName { get; set; }
        public DateTime? SessionDate { get; set; }
    }
    public class TimeTableRow
    {
        public string CourseName { get; set; }
        public string SyllabusName { get; set; }
        public string TopicName { get; set; }
        public string Duration { get; set; }
        public DateTime? SessionDate { get; set; }
        public TimeSpan? StartTime { get; set; }   
        public TimeSpan? EndTime { get; set; }
        public string TrainerName { get; set; }
    }
    public class ExamViewModel
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int Duration { get; set; }
        public int MaxMarks { get; set; }
        public int PassingMarks { get; set; }
        public string TopicName { get; set; }
    }
    public class ExamQuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int Marks { get; set; }
        public List<OptionViewModel> Options { get; set; }
    }

    public class OptionViewModel
    {
        public int OptionId { get; set; }
        public string OptionText { get; set; }

    }
    }

