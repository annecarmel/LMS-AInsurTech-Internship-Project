namespace ASSNlearningManagementSystem.Models
{
    public class Syllabus
    {
        public int SyllabusID { get; set; }
        public string Title { get; set; }
        public int CourseID { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public List<Topic> Topics { get; set; }
    }
}
