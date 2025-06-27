// File: Controllers/SessionController.cs

using Microsoft.AspNetCore.Mvc;

namespace ASSNlearningManagementSystem.Controllers
{
    public class SessionController : Controller
    {
        [HttpGet]
        public IActionResult Session()
        {
            return View();
        }
    }
}
