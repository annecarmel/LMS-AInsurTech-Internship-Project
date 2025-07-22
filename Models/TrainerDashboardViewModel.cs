namespace ASSNlearningManagementSystem.Models

{
    public class TrainerDashboardViewModel
    {
        public int TotalLearners { get; set; }
        public int AssignedCourses { get; set; }
        public int TotalCourses { get; set; }

        public List<SessionSchedule> Sessions { get; set; }
        public List<EvaluationInfo> Evaluations { get; set; }
        public Dictionary<string, int> BarChartData { get; set; }
        public Dictionary<string, int> PieChartData { get; set; }
    }

    public class SessionSchedule
    {
        public int SessionId { get; set; }
        public string Topic { get; set; }
        public DateTime Date { get; set; }
        public string LearnerName { get; set; }
    }

    public class EvaluationInfo
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string LearnerName { get; set; }
        public string Status { get; set; }
        public int? TotalMarks { get; set; }
        public int? MarksObtained { get; set; }
    }

    public class BarChartData
    {
        public string CourseName { get; set; }
        public int SessionCount { get; set; }
    }

    public class PieChartData
    {
        public string CourseName { get; set; }
        public int SubmissionCount { get; set; }
    }

}
