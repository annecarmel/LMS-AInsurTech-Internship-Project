using Microsoft.AspNetCore.Mvc;

namespace ASSNlearningManagementSystem.Controllers
{
    public class ExamController : Controller
    {
        public IActionResult Exam()
        {
            return View();
        }

        public IActionResult Results()
        {
            return View();
        }
    }
}
