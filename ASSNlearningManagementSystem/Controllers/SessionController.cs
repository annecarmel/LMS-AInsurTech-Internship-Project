using ASSNlearningManagementSystem.Models;
using ASSNlearningManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace ASSNlearningManagementSystem.Controllers
{
    public class SessionController : Controller
    {
        private readonly SessionRepository _sessionRepo;

        public SessionController(IConfiguration configuration)
        {
            _sessionRepo = new SessionRepository(configuration.GetConnectionString("DefaultConnection"));
        }

        // GET: /Session/Session
        [HttpGet]
        public IActionResult Session(int? id)
        {
            var model = new SessionViewModel();

            if (id.HasValue)
            {
                model.NewSession = _sessionRepo.GetSessionById(id.Value) ?? new Session();
            }
            else
            {
                model.NewSession = new Session();
            }

            model.SessionsList = _sessionRepo.GetAllSessions() ?? new List<Session>();

            // ✅ Calculate real-time status
            foreach (var session in model.SessionsList)
            {
                session.Status = CalculateStatus(session.SessionDate, session.StartTime, session.EndTime);
            }

            ViewBag.Trainers = new SelectList(_sessionRepo.GetTrainers(), "Key", "Value");
            ViewBag.Courses = new SelectList(_sessionRepo.GetCourses(), "Key", "Value");
            ViewBag.Syllabuses = new SelectList(_sessionRepo.GetSyllabuses(), "Key", "Value");
            ViewBag.Topics = new SelectList(_sessionRepo.GetTopics(), "Key", "Value");

            return View("Session", model);
        }

        // POST: /Session/SaveSession
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveSession(SessionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // ✅ LOGGING VALIDATION ERRORS — DEBUGGING AID
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation failed for '{key}': {error.ErrorMessage}");
                    }
                }

                ViewBag.Trainers = new SelectList(_sessionRepo.GetTrainers(), "Key", "Value");
                ViewBag.Courses = new SelectList(_sessionRepo.GetCourses(), "Key", "Value");
                ViewBag.Syllabuses = new SelectList(_sessionRepo.GetSyllabuses(), "Key", "Value");
                ViewBag.Topics = new SelectList(_sessionRepo.GetTopics(), "Key", "Value");

                model.SessionsList = _sessionRepo.GetAllSessions() ?? new List<Session>();
                return View("Session", model);
            }

            if (model.NewSession.SessionID == 0)
            {
                model.NewSession.CreatedBy = 1; // TODO: replace with actual user ID
                model.NewSession.CreatedOn = DateTime.Now;
            }
            else
            {
                model.NewSession.UpdatedBy = 1; // TODO: replace with actual user ID
                model.NewSession.UpdatedOn = DateTime.Now;
            }

            // ✅ Automatically calculate status based on session time
            model.NewSession.Status = CalculateStatus(model.NewSession.SessionDate, model.NewSession.StartTime, model.NewSession.EndTime);

            _sessionRepo.SaveSession(model.NewSession);

            TempData["SuccessMessage"] = model.NewSession.SessionID == 0
                ? "Session created successfully!"
                : "Session updated successfully!";

            return RedirectToAction("Session");
        }

        // POST: /Session/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            _sessionRepo.DeleteSession(id);
            TempData["SuccessMessage"] = "Session deleted successfully!";
            return RedirectToAction("Session");
        }

        // AJAX: Get syllabuses by course ID
        [HttpGet]
        public IActionResult GetSyllabusesByCourse(int courseId)
        {
            var syllabuses = _sessionRepo.GetSyllabusesByCourseId(courseId);
            return Json(syllabuses);
        }

        // AJAX: Get topics by syllabus ID
        [HttpGet]
        public IActionResult GetTopicsBySyllabus(int syllabusId)
        {
            var topics = _sessionRepo.GetTopicsBySyllabusId(syllabusId);
            return Json(topics);
        }

        // AJAX: Get enrolled learner count by course ID
        [HttpGet]
        public IActionResult GetEnrolledLearnerCount(int courseId)
        {
            int count = _sessionRepo.GetEnrolledLearnerCountByCourseId(courseId);
            return Json(new { count });
        }

        // ✅ New: Calculate status based on current date and session time
        private string CalculateStatus(DateTime sessionDate, TimeSpan startTime, TimeSpan endTime)
        {
            var now = DateTime.Now;
            var startDateTime = sessionDate.Date + startTime;
            var endDateTime = sessionDate.Date + endTime;

            if (now < startDateTime)
                return "Scheduled";
            else if (now >= startDateTime && now <= endDateTime)
                return "Ongoing";
            else
                return "Completed";
        }
    }
}
