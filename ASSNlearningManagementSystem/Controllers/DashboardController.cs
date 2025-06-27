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

            // === Bar & Donut Chart Data ===
            var enrollmentData = _repo.GetCourseEnrollmentData();
            var popularityData = _repo.GetCoursePopularityByDepartment();

            var camelCaseSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            ViewBag.EnrollmentDataJson = JsonConvert.SerializeObject(enrollmentData, camelCaseSettings);
            ViewBag.PopularityDataJson = JsonConvert.SerializeObject(popularityData, camelCaseSettings);

            // === Tables ===
            ViewBag.UpcomingExams = _repo.GetUpcomingExams();
            ViewBag.SessionOverview = _repo.GetSessionOverview();

            return View("Dashboard");
        }
    }
}
