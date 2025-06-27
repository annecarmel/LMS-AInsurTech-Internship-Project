using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASSNlearningManagementSystem.Models
{
    public class UserViewModel
    {
        // Personal Details
        public int UserId { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }

        // User Details
        [Display(Name = "Full Name")]
        public string? FullName { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? Department { get; set; }

        [Display(Name = "Role ID")]
        public int RoleId { get; set; }

        public string? RoleName { get; set; }

        public int? CourseId { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public string? EmploymentStatus { get; set; }

        public int? Duration { get; set; }

        public bool? IsActive { get; set; }
    }

    public class UserRoleViewModel
    {
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public class UserPageViewModel
    {
        public UserViewModel User { get; set; } = new UserViewModel();
        public List<UserViewModel> UserList { get; set; } = new List<UserViewModel>();
    }
}
