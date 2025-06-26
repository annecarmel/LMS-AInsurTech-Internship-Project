using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Models
{
    public class EmployeePageViewModel
    {
        public EmployeeViewModel Employee { get; set; }
        public List<EmployeeViewModel> EmployeeList { get; set; }
    }
}
