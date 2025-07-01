using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ASSNlearningManagementSystem.DataAccess;

namespace ASSNlearningManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserRepository _userRepository;

        public LoginController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _userRepository.GetUserByUsernameAndPassword(username, password);

            if (user != null)
            {
                // ✅ Save session details
                HttpContext.Session.SetInt32("UserId", user.Value.UserId);
                HttpContext.Session.SetString("Username", user.Value.Username);
                HttpContext.Session.SetString("Role", user.Value.Role);
                HttpContext.Session.SetString("Department", user.Value.Department ?? "");
                HttpContext.Session.SetInt32("IsActive", 1);

                // ✅ Update last_login and set is_active = 1
                _userRepository.UpdateLastLoginAndIsActive(user.Value.UserId, true);

                // ✅ Redirect based on role
                switch (user.Value.Role.ToLower())
                {
                    case "admin":
                        return RedirectToAction("Dashboard", "Dashboard");
                    case "trainer":
                        return RedirectToAction("Dashboard", "Trainer");
                    case "learner":
                        return RedirectToAction("Dashboard", "Learner");
                    default:
                        ViewBag.Error = "Unauthorized role";
                        return View("~/Views/Login/Login.cshtml");
                }
            }

            ViewBag.Error = "Invalid username or password";
            return View("~/Views/Login/Login.cshtml");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                HttpContext.Session.SetInt32("IsActive", 0);

                // ✅ Set is_active = 0 when logging out
                _userRepository.UpdateIsActive(userId.Value, false);
            }

            // ✅ Clear session
            HttpContext.Session.Clear();

            // ✅ Redirect back to Login page
            return RedirectToAction("Login", "Login");
        }

        

    }
}
