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
            // Server-side validation
            if (_employeeRepository.EmailExists(employee.email, employee.user_id))
            {
                ModelState.AddModelError("email", "Email already exists. Please enter another email.");
            }

            if (_employeeRepository.PhoneNumberExists(employee.phone_number, employee.user_id))
            {
                ModelState.AddModelError("phone_number", "Phone number already exists. Please enter another phone number.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (employee.user_id > 0)
                    {
                        // ✅ UPDATE
                        employee.updated_on = DateTime.Now;
                        employee.updated_by = HttpContext.Session.GetInt32("UserId");

                        _employeeRepository.UpdateEmployee(employee);
                        TempData["SuccessMessage"] = "User updated successfully!";
                    }
                    else
                    {
                        // ✅ CREATE
                        employee.username = $"{employee.first_name}.{employee.last_name}".ToLower();
                        employee.password = GenerateRandomPassword();
                        employee.full_name = $"{employee.first_name} {employee.last_name}";
                        employee.created_on = DateTime.Now;
                        employee.updated_on = DateTime.Now;
                        employee.is_active = false;

                        employee.created_by = HttpContext.Session.GetInt32("UserId");
                        employee.updated_by = HttpContext.Session.GetInt32("UserId");

                        _employeeRepository.AddEmployee(employee);
                        TempData["SuccessMessage"] = "User saved successfully!";
                    }

                    return RedirectToAction("Employee");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error: {ex.Message}";
                    return RedirectToAction("Employee");
                }
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
