namespace ASSNlearningManagementSystem.Models
{
    public class UserProfileViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // If FullName is computed, it's fine
        public string FullName => $"{FirstName} {LastName}";
    }

}
