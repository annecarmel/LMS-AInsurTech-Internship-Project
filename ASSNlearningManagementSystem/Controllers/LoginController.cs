using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ASSNlearningManagementSystem.DataAccess;

namespace ASSNlearningManagementSystem.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserRepository _userRepository;

        // Injecting UserRepository through constructor
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
            if (_userRepository.ValidateUser(username, password))
            {
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("Dashboard", "Dashboard");
            }

            ViewBag.Error = "Invalid username or password";
            return View("~/Views/Login/Login.cshtml");
        }
    }
}
