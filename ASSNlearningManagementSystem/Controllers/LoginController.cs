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
                HttpContext.Session.SetString("Username", user.Value.Username);
                HttpContext.Session.SetString("Role", user.Value.Role); // ✅ fixed

                // Redirect based on role
                switch (user.Value.Role.ToLower())
                {
                    case "admin":
                        return RedirectToAction("Dashboard", "Dashboard");

                    case "trainer":
                        return RedirectToAction("Dashboard", "Trainer");


                    case "learner":
                        return View("Dashboard", "Leaner");

                    default:
                        ViewBag.Error = "Unauthorized role";
                        return View("~/Views/Login/Login.cshtml");
                }
            }

            ViewBag.Error = "Invalid username or password";
            return View("~/Views/Login/Login.cshtml");
        }
    }
}
