using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ASSNlearningManagementSystem.DataAccess;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

            // === Bar & Pie Chart Data ===
            var enrollmentData = _repo.GetCourseEnrollmentData();         // Bar chart
            var popularityData = _repo.GetTop5PopularCourses();          // Pie chart (Top 5 only)

            var camelCaseSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ViewBag.EnrollmentDataJson = JsonConvert.SerializeObject(enrollmentData, camelCaseSettings);
            ViewBag.PopularityDataJson = JsonConvert.SerializeObject(popularityData, camelCaseSettings);

            // === Tables ===

            // ✅ Show only "Scheduled" exams (i.e., upcoming)
            var allExams = _repo.GetUpcomingExams();
            var scheduledExams = allExams
                .Where(e => e.Status != null && e.Status.Trim().ToLower() == "scheduled")
                .OrderBy(e => e.Date) // Optional: show nearest upcoming first
                .ToList();

            ViewBag.UpcomingExams = scheduledExams;

            ViewBag.SessionOverview = _repo.GetSessionOverview();

            return View("Dashboard");
        }
    }
}
