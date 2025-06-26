using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ASSNlearningManagementSystem.DataAccess;
using Newtonsoft.Json;
using System.Linq;

namespace ASSNlearningManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardRepository _repo;

        public DashboardController(IConfiguration configuration)
        {
            _repo = new DashboardRepository(configuration);
        }

        public IActionResult Dashboard()
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Username = username;

            // === Top Card Data ===
            ViewBag.TotalUsers = _repo.GetTotalUsers();
            ViewBag.ActiveCourses = _repo.GetActiveCourses();
            ViewBag.ScheduledExams = _repo.GetScheduledExams();

            // === Bar Chart: Course Enrollment ===
            var enrollmentData = _repo.GetCourseEnrollmentData(); // returns list of { CourseName, Count }
            ViewBag.EnrollmentDataJson = JsonConvert.SerializeObject(enrollmentData);

            // === Donut Chart: Course Popularity by Department ===
            var popularityData = _repo.GetCoursePopularityByDepartment(); // returns list of { CourseName, Department, Count }
            ViewBag.PopularityDataJson = JsonConvert.SerializeObject(popularityData);

            // === Upcoming Exams Table ===
            ViewBag.UpcomingExams = _repo.GetUpcomingExams(); // returns list of { Title, Date, Duration, Status }

            // === Session Overview Table ===
            ViewBag.SessionOverview = _repo.GetSessionOverview(); // returns list of { Trainer, Topic, Students, Status }

            return View("Dashboard");
        }
    }
}
