using Microsoft.AspNetCore.Mvc;

namespace ASSNlearningManagementSystem.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
