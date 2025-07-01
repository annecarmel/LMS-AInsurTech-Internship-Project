using ASSNlearningManagementSystem.Models;
using ASSNlearningManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;

namespace ASSNlearningManagementSystem.Controllers
{
    public class ExamController : Controller
    {
        private readonly ExamRepository _examRepo;
        private readonly ExamResultRepository _resultRepo;

        public ExamController(IConfiguration configuration)
        {
            string connStr = configuration.GetConnectionString("DefaultConnection");
            _examRepo = new ExamRepository(connStr);
            _resultRepo = new ExamResultRepository(connStr);
        }

        // ✅ Exam Master - GET all exams
        public IActionResult Exam()
        {
            var exams = _examRepo.GetAllExams();
            ViewBag.Courses = _examRepo.GetCourses();
            ViewBag.Instructors = _examRepo.GetTrainers();
            ViewBag.Evaluators = _examRepo.GetTrainers();
            return View(exams);
        }

        // ✅ Exam Master - Create new exam
        [HttpPost]
        public IActionResult Create(Exam exam)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                exam.CreatedBy = userId.Value;
                exam.UpdatedBy = userId.Value;
            }

            exam.CreatedOn = DateTime.Now;
            exam.UpdatedOn = DateTime.Now;
            exam.Status = "Scheduled";

            _examRepo.InsertExam(exam);
            TempData["ExamMessage"] = "saved";
            return RedirectToAction("Exam");
        }

        // ✅ Exam Master - Edit GET
        public IActionResult Edit(int id)
        {
            var examToEdit = _examRepo.GetExamById(id);
            var exams = _examRepo.GetAllExams();

            ViewBag.ExamToEdit = examToEdit;
            ViewBag.Courses = _examRepo.GetCourses();
            ViewBag.Instructors = _examRepo.GetTrainers();
            ViewBag.Evaluators = _examRepo.GetTrainers();

            return View("Exam", exams);
        }

        // ✅ Exam Master - Edit POST
        [HttpPost]
        public IActionResult Edit(Exam exam)
        {
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId.HasValue)
            {
                exam.UpdatedBy = userId.Value;
            }

            exam.UpdatedOn = DateTime.Now;

            _examRepo.UpdateExam(exam);
            TempData["ExamMessage"] = "updated";
            return RedirectToAction("Exam");
        }

        // ✅ Exam Master - Delete
        public IActionResult Delete(int id)
        {
            try
            {
                _examRepo.DeleteExam(id);
                TempData["ExamMessage"] = "deleted";
            }
            catch (Exception)
            {
                TempData["ExamMessage"] = "Cannot delete this exam. It is already in use.";
            }

            return RedirectToAction("Exam");
        }

        // ✅ Dependent dropdown: Syllabuses by Course
        public IActionResult GetSyllabuses(int courseId)
        {
            var syllabuses = _examRepo.GetSyllabusesByCourseId(courseId);
            return Json(syllabuses);
        }

        // ✅ Dependent dropdown: Topics by Syllabus
        public IActionResult GetTopics(int syllabusId)
        {
            var topics = _examRepo.GetTopicsBySyllabusId(syllabusId);
            return Json(topics);
        }

        // ✅ Exam Results page
        public IActionResult Results()
        {
            var results = _resultRepo.GetAllExamResults();
            return View("Results", results);
        }
    }
}
