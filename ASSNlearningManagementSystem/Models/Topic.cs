namespace ASSNlearningManagementSystem.Models
{
    public class Topic
    {
        public int TopicID { get; set; }
        public string Title { get; set; }
        public int DurationHours { get; set; }
        public string Description { get; set; }
        public int SyllabusID { get; set; }

        // ✅ Add these 4
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
