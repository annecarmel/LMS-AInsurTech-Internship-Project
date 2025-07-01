using Microsoft.AspNetCore.Mvc;
using ASSNlearningManagementSystem.Models;
using ASSNlearningManagementSystem.Repository;

namespace ASSNlearningManagementSystem.Controllers
{
    public class RatingController : Controller
    {
        private readonly RatingRepository _ratingRepository;

        public RatingController(IConfiguration configuration)
        {
            _ratingRepository = new RatingRepository(configuration.GetConnectionString("DefaultConnection"));
        }

        // ---------------------------
        // DISPLAY ALL RATINGS
        // ---------------------------
        public IActionResult Rating()
        {
            var ratings = _ratingRepository.GetAllRatings();
            return View(ratings);
        }

        // ---------------------------
        // DELETE A RATING
        // ---------------------------
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _ratingRepository.DeleteRating(id);
            TempData["RatingSuccess"] = "Rating deleted successfully!";

            return RedirectToAction("Rating");
        }
    }
}
