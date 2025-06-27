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
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        // User Details
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Department { get; set; }

        [Display(Name = "Role ID")]
        public int RoleId { get; set; }

        public string RoleName { get; set; } // To display in the User List

        // List for dropdown roles
        public List<UserRoleViewModel> RoleList { get; set; } = new List<UserRoleViewModel>();
    }

    public class UserRoleViewModel
    {
        public int role_id { get; set; }
        public string role_name { get; set; }
    }

    // ✅ Add this class — missing earlier
    public class UserPageViewModel
    {
        public UserViewModel User { get; set; } = new UserViewModel();
        public List<UserViewModel> UserList { get; set; } = new List<UserViewModel>();
    }
}
