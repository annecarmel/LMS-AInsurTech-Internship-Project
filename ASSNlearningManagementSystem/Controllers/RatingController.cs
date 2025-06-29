using Microsoft.AspNetCore.Mvc;

namespace ASSNlearningManagementSystem.Controllers
{
    public class RatingController : Controller
    {
        public IActionResult Rating()
        {
            return View();
        }
    }
}
