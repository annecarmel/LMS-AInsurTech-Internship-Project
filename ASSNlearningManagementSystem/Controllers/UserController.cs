using Microsoft.AspNetCore.Mvc;

namespace ASSNlearningManagementSystem.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
