using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace ASSNlearningManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Login");
            }

            ViewBag.Username = username;
            return View("Dashboard", "Admin");
        }
    }
}
