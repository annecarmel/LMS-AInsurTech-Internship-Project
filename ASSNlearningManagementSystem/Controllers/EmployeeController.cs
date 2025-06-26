using ASSNlearningManagementSystem.Models;
using ASSNlearningManagementSystem.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace ASSNlearningManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly EmployeeRepository _employeeRepository;

        public EmployeeController(IConfiguration configuration)
        {
            _employeeRepository = new EmployeeRepository(configuration);
        }

        [HttpGet]
        public IActionResult Employee()
        {
            var model = new EmployeePageViewModel
            {
                Employee = new EmployeeViewModel(),
                EmployeeList = _employeeRepository.GetAllEmployees()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveEmployee(EmployeeViewModel employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (employee.user_id > 0)
                    {
                        // Update case
                        employee.updated_on = DateTime.Now;
                        _employeeRepository.UpdateEmployee(employee);
                        TempData["SuccessMessage"] = "User updated successfully!";
                    }
                    else
                    {
                        // Create case
                        employee.username = $"{employee.first_name}.{employee.last_name}".ToLower();
                        employee.password = GenerateRandomPassword();
                        employee.full_name = $"{employee.first_name} {employee.last_name}";
                        employee.created_on = DateTime.Now;
                        employee.updated_on = DateTime.Now;
                        employee.is_active = true;

                        _employeeRepository.AddEmployee(employee);
                        TempData["SuccessMessage"] = "User saved successfully!";
                    }
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error: {ex.Message}";
                }

                return RedirectToAction("Employee");
            }

            var model = new EmployeePageViewModel
            {
                Employee = employee,
                EmployeeList = _employeeRepository.GetAllEmployees()
            };

            return View("Employee", model);
        }

        [HttpGet]
        public IActionResult EditEmployee(int id)
        {
            var employee = _employeeRepository.GetEmployeeById(id);

            if (employee == null)
            {
                TempData["ErrorMessage"] = "User not found!";
                return RedirectToAction("Employee");
            }

            var model = new EmployeePageViewModel
            {
                Employee = employee,
                EmployeeList = _employeeRepository.GetAllEmployees()
            };

            return View("Employee", model);
        }

        private string GenerateRandomPassword()
        {
            return "Pass@" + Guid.NewGuid().ToString("N")[..6];
        }

        [HttpGet]
        public IActionResult DeleteEmployee(int id)
        {
            try
            {
                _employeeRepository.DeleteEmployee(id);
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting user: {ex.Message}";
            }

            return RedirectToAction("Employee");
        }

    }
}
