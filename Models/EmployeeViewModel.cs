using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ASSNlearningManagementSystem.Models
{
    public class EmployeeViewModel
    {
        public int user_id { get; set; }

        [Required]
        public string first_name { get; set; }

        [Required]
        public string last_name { get; set; }

        [DataType(DataType.Date)]
        public DateTime? date_of_birth { get; set; }

        public string gender { get; set; }

        [Phone]
        public string phone_number { get; set; }

        [EmailAddress]
        public string email { get; set; }

        public string city { get; set; }

        public string state { get; set; }

        public string country { get; set; }

        public string department { get; set; }

        public int role_id { get; set; }

        // ✅ Auto-generated fields
        public string? username { get; set; } = "";

        public string? password { get; set; } = "";

        public string? full_name { get; set; }

        public DateTime created_on { get; set; }

        public DateTime updated_on { get; set; }

        public bool is_active { get; set; }

        // ✅ NEW: created_by & updated_by for auditing
        public int? created_by { get; set; }

        public int? updated_by { get; set; }

        // For displaying user list
        public List<EmployeeViewModel> EmployeeList { get; set; } = new List<EmployeeViewModel>();
    }
}
