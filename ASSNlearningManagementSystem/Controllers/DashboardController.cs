using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ASSNlearningManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            // Get the logged-in username from session
            var username = HttpContext.Session.GetString("Username");

            // If not logged in, redirect to login page
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Login");
            }

            // Pass username to view for greeting or personalization
            ViewBag.Username = username;

            // Load the Dashboard.cshtml view from Views/Dashboard folder
            return View();
        }
    }
}
