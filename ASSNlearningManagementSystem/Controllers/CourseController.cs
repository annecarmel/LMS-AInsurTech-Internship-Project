using ASSNlearningManagementSystem.DataAccess;
using ASSNlearningManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // ✅ Needed for session
using System;

namespace ASSNlearningManagementSystem.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseRepository _courseRepo;

        public CourseController(CourseRepository courseRepo)
        {
            _courseRepo = courseRepo;
        }

        // ✅ Show course form + course list
        [HttpGet]
        public IActionResult Course()
        {
            var model = new CourseViewModel
            {
                NewCourse = new Course(),
                CourseList = _courseRepo.GetAllCourses()
            };

            return View(model);
        }

        // ✅ Save (Create or Update) course
        [HttpPost]
        public IActionResult Save(Course course)
        {
            // ✅ Get logged-in user ID from session
            var userId = HttpContext.Session.GetInt32("UserId")?.ToString();

            bool success;

            if (course.CourseID == 0)
            {
                // Add
                course.CreatedOn = DateTime.Now;
                course.CreatedBy = userId; // ✅ use actual user ID

                success = _courseRepo.AddCourse(course);
                TempData["Success"] = success ? "✅ Course saved successfully." : "❌ Failed to save course.";
            }
            else
            {
                // Update
                course.UpdatedOn = DateTime.Now;
                course.UpdatedBy = userId; // ✅ use actual user ID

                success = _courseRepo.UpdateCourse(course);
                TempData["Success"] = success ? "✅ Course updated successfully." : "❌ Failed to update course.";
            }

            return RedirectToAction("Course");
        }

        // ✅ Load course for editing
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var course = _courseRepo.GetCourseById(id);
            if (course == null)
            {
                TempData["Error"] = "❌ Course not found.";
                return RedirectToAction("Course");
            }

            var model = new CourseViewModel
            {
                NewCourse = course,
                CourseList = _courseRepo.GetAllCourses()
            };

            return View("Course", model);
        }

        // ✅ Delete course (secure via form post)
        [HttpPost]
        public IActionResult Delete(int id)
        {
            bool success = _courseRepo.DeleteCourse(id);
            TempData["Success"] = success
                ? "🗑️ Course deleted successfully."
                : "❌ Failed to delete course.";

            return RedirectToAction("Course");
        }
    }
}
