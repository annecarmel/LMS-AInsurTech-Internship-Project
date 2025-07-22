namespace ASSNlearningManagementSystem.Models
{
    public class MCQOptionModel
    {
        public int OptionID { get; set; }
        public int QuestionId { get; set; }
        public string OptionText { get; set; }
        // IsCorrect field is skipped as evaluation is manual
    }
}
