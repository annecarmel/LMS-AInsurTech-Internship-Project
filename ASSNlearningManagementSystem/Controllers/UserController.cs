using ASSNlearningManagementSystem.Models;
using ASSNlearningManagementSystem.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ASSNlearningManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;

        public UserController(IConfiguration configuration)
        {
            _userRepository = new UserRepository(configuration);
        }

        [HttpGet]
        public IActionResult User()
        {
            var model = new UserPageViewModel
            {
                User = new UserViewModel(),
                UserList = _userRepository.GetAllUsers()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveUser(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                _userRepository.AddUser(user);

                TempData["SuccessMessage"] = "User saved successfully!";
                return RedirectToAction("User");
            }

            var model = new UserPageViewModel
            {
                User = user,
                UserList = _userRepository.GetAllUsers()
            };

            return View("User", model);
        }
    }
}
