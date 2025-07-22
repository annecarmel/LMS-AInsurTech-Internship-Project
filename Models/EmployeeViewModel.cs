using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASSNlearningManagementSystem.Models
{
    public class EmployeeViewModel : IValidatableObject
    {
        public int user_id { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        public string first_name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        public string last_name { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime? date_of_birth { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string gender { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be 10 digits.")]
        public string phone_number { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string email { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public string city { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public string state { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public string country { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        public string department { get; set; }

        [Required(ErrorMessage = "Role is required.")]
        public int role_id { get; set; }

        public string? username { get; set; } = "";
        public string? password { get; set; } = "";
        public string? full_name { get; set; }

        public DateTime created_on { get; set; }
        public DateTime updated_on { get; set; }
        public bool is_active { get; set; }
        public int? created_by { get; set; }
        public int? updated_by { get; set; }

        public List<EmployeeViewModel> EmployeeList { get; set; } = new List<EmployeeViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (date_of_birth != null)
            {
                var today = DateTime.Today;
                var minDOB = today.AddYears(-13);
                if (date_of_birth > minDOB)
                {
                    yield return new ValidationResult("User must be at least 13 years old.", new[] { nameof(date_of_birth) });
                }
            }
        }
    }
}
