using Microsoft.AspNetCore.Mvc;

namespace ASSNlearningManagementSystem.Controllers
{
    public class SessionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
