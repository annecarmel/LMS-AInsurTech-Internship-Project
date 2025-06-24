using Microsoft.AspNetCore.Mvc;

namespace ASSNlearningManagementSystem.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
